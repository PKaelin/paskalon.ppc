// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Infrastructure.IntegrationTest.Storage
{
    /// <summary>
    /// Tests <see cref="AlarmActionOutboxRepository"/> against a migrated PostgreSQL database.
    /// </summary>
    [TestClass]
    public class AlarmActionOutboxRepositoryTest : DatabaseTestBase
    {
        /// <summary>
        /// Seeds the PCS fault configuration.
        /// </summary>
        protected override async Task SeedAsync(AlarmContext db)
        {
            db.PpcAlarmConfigs.Add(PcsAlarmSampleData.CreatePcsFaultConfig());
            await db.SaveChangesAsync();
        }


        /// <summary>
        /// The outbox message of a raised alarm is saved in the same save as the alarm and references the new alarm.
        /// </summary>
        [TestMethod]
        public async Task AddWithRaisedAlarmSavesBothInOneSaveTest()
        {
            Guid messageId = Guid.CreateVersion7();

            await using (AlarmContext context = CreateDbContext<AlarmContext>())
            {
                PpcAlarmRepository alarmRepository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
                AlarmActionOutboxRepository outboxRepository = new AlarmActionOutboxRepository(NullLogger<AlarmActionOutboxRepository>.Instance, context);
                PpcAlarmConfig config = await context.PpcAlarmConfigs.SingleAsync();
                PpcAlarm alarm = PcsAlarmSampleData.CreateAlarm(config, 7, "FaultStates.OverTemperature");
                alarmRepository.Add(new List<PpcAlarm> { alarm });
                outboxRepository.Add(new List<AlarmActionOutboxMessage> { CreateMessage(messageId, alarm) });

                await alarmRepository.SaveChangesAsync();
            }

            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            AlarmActionOutboxMessage message = await verifyContext.AlarmActionOutboxMessages.Include(m => m.PpcAlarm).SingleAsync();
            Assert.AreEqual(messageId, message.MessageId);
            Assert.AreEqual("FaultStates.OverTemperature", message.PpcAlarm.PropertyName);
            Assert.AreEqual(message.PpcAlarm.Id, message.PpcAlarmId);
            Assert.IsNull(message.SentOn);
            Assert.AreEqual(0, message.FailedAttempts);
            using JsonDocument payload = JsonDocument.Parse(message.Payload);
            Assert.AreEqual("ReducePower", payload.RootElement.GetProperty("Name").GetString());
        }


        /// <summary>
        /// When saving the alarm fails (second active alarm) the outbox message is not saved either.
        /// </summary>
        [TestMethod]
        public async Task AddWithFailingAlarmSaveSavesNothingTest()
        {
            await using (AlarmContext seedContext = CreateDbContext<AlarmContext>())
            {
                PpcAlarmConfig seedConfig = await seedContext.PpcAlarmConfigs.SingleAsync();
                seedContext.PpcAlarms.Add(PcsAlarmSampleData.CreateAlarm(seedConfig, 7, "FaultStates.OverTemperature"));
                await seedContext.SaveChangesAsync();
            }

            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmRepository alarmRepository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
            AlarmActionOutboxRepository outboxRepository = new AlarmActionOutboxRepository(NullLogger<AlarmActionOutboxRepository>.Instance, context);
            PpcAlarmConfig config = await context.PpcAlarmConfigs.SingleAsync();
            PpcAlarm duplicatedAlarm = PcsAlarmSampleData.CreateAlarm(config, 7, "FaultStates.OverTemperature");
            alarmRepository.Add(new List<PpcAlarm> { duplicatedAlarm });
            outboxRepository.Add(new List<AlarmActionOutboxMessage> { CreateMessage(Guid.CreateVersion7(), duplicatedAlarm) });

            await Assert.ThrowsExactlyAsync<DbUpdateException>(() => alarmRepository.SaveChangesAsync());

            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            Assert.AreEqual(0, await verifyContext.AlarmActionOutboxMessages.CountAsync());
            Assert.AreEqual(1, await verifyContext.PpcAlarms.CountAsync());
        }


        /// <summary>
        /// Deleting an archived (normalized) alarm deletes its outbox messages, but keeps its configuration.
        /// </summary>
        [TestMethod]
        public async Task AddMessagesAreDeletedWithTheirAlarmTest()
        {
            await using (AlarmContext seedContext = CreateDbContext<AlarmContext>())
            {
                AlarmActionOutboxRepository outboxRepository = new AlarmActionOutboxRepository(NullLogger<AlarmActionOutboxRepository>.Instance, seedContext);
                PpcAlarmConfig config = await seedContext.PpcAlarmConfigs.SingleAsync();
                PpcAlarm archivedAlarm = PcsAlarmSampleData.CreateAlarm(config, 7, "FaultStates.OverTemperature");
                archivedAlarm.Normalize("System", PcsAlarmSampleData.ChangedOn.AddMinutes(10));
                PpcAlarm activeAlarm = PcsAlarmSampleData.CreateAlarm(config, 8, "FaultStates.OverTemperature");
                seedContext.PpcAlarms.AddRange(archivedAlarm, activeAlarm);
                outboxRepository.Add(new List<AlarmActionOutboxMessage>
                {
                    CreateMessage(Guid.CreateVersion7(), archivedAlarm),
                    CreateMessage(Guid.CreateVersion7(), archivedAlarm),
                    CreateMessage(Guid.CreateVersion7(), activeAlarm)
                });
                await seedContext.SaveChangesAsync();
            }

            await using (AlarmContext context = CreateDbContext<AlarmContext>())
            {
                await context.PpcAlarms.Where(alarm => alarm.DeviceId == 7).ExecuteDeleteAsync();
            }

            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            AlarmActionOutboxMessage remainingMessage = await verifyContext.AlarmActionOutboxMessages.Include(m => m.PpcAlarm).SingleAsync();
            Assert.AreEqual(8, remainingMessage.PpcAlarm.DeviceId);
            Assert.AreEqual(1, await verifyContext.PpcAlarms.CountAsync());
            Assert.AreEqual(1, await verifyContext.PpcAlarmConfigs.CountAsync());
        }


        /// <summary>
        /// Creates a pending reduce power message of an alarm.
        /// </summary>
        private static AlarmActionOutboxMessage CreateMessage(Guid messageId, PpcAlarm alarm)
        {
            return new AlarmActionOutboxMessage
            {
                ChangedBy = "System",
                ChangedDate = PcsAlarmSampleData.ChangedOn,
                MessageId = messageId,
                PpcAlarm = alarm,
                ActionName = "ReducePower",
                Payload = $"{{\"MessageId\":\"{messageId}\",\"Name\":\"ReducePower\",\"AlarmActionParameters\":[{{\"Key\":\"DeviceId\",\"Value\":\"7\"}}]}}",
                CreatedOn = PcsAlarmSampleData.ChangedOn
            };
        }
    }
}
