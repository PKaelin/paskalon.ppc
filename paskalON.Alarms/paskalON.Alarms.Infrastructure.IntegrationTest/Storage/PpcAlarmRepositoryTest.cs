// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Infrastructure.IntegrationTest.Storage
{
    /// <summary>
    /// Tests <see cref="PpcAlarmRepository"/> against a migrated PostgreSQL database.
    /// </summary>
    [TestClass]
    public class PpcAlarmRepositoryTest : DatabaseTestBase
    {
        /// <summary>
        /// Key of PCS 7.
        /// </summary>
        private static readonly PpcAlarmKey Pcs7Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 7 };


        /// <summary>
        /// Key of the system.
        /// </summary>
        private static readonly PpcAlarmKey SystemKey = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.SystemAlarm };


        /// <summary>
        /// Timestamp when the alarms changed in the tests.
        /// </summary>
        private static readonly DateTimeOffset ChangedOn = PcsAlarmSampleData.ChangedOn.AddMinutes(5);


        /// <summary>
        /// Seeds the PCS fault, grid loss and the system configurations.
        /// </summary>
        protected override async Task SeedAsync(AlarmContext db)
        {
            db.PpcAlarmConfigs.Add(PcsAlarmSampleData.CreatePcsFaultConfig());
            db.PpcAlarmConfigs.Add(PcsAlarmSampleData.CreatePcsGridLossConfig());
            db.PpcAlarmConfigs.Add(PcsAlarmSampleData.CreateSystemRtuConfig());
            await db.SaveChangesAsync();
        }


        /// <summary>
        /// A raised alarm with a configuration loaded in another context is persisted without inserting the configuration again.
        /// </summary>
        [TestMethod]
        public async Task AddRaisedAlarmWithDetachedConfigPersistsAlarmTest()
        {
            PpcAlarmConfig faultConfig = await GetConfigAsync("PCS-FAULT");
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);

            repository.Add(new List<PpcAlarm> { PcsAlarmSampleData.CreateAlarm(faultConfig, 7, "FaultStates.OverTemperature") });
            await repository.SaveChangesAsync();

            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            PpcAlarm alarm = await verifyContext.PpcAlarms.Include(a => a.PpcAlarmConfig).SingleAsync();
            Assert.AreEqual(7, alarm.DeviceId);
            Assert.AreEqual("FaultStates.OverTemperature", alarm.PropertyName);
            Assert.AreEqual(AlarmSequenceState.Alarm, alarm.AlarmSequenceState);
            Assert.AreEqual(AlarmProcessCondition.Abnormal, alarm.AlarmProcessCondition);
            Assert.AreEqual(PcsAlarmSampleData.ChangedOn, alarm.CreatedOn);
            Assert.AreEqual(AlarmSeverity.Error, alarm.SeverityLevel);
            Assert.AreEqual(3, await verifyContext.PpcAlarmConfigs.CountAsync());
        }


        /// <summary>
        /// A raised alarm with a cached (detached) configuration is persisted although the context already tracks
        /// the same configuration with an active alarm.
        /// </summary>
        [TestMethod]
        public async Task AddRaisedAlarmWithCachedConfigTrackedByActiveAlarmPersistsAlarmTest()
        {
            await SeedAlarmsAsync();
            PpcAlarmConfig cachedFaultConfig = await GetConfigAsync("PCS-FAULT");
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
            List<PpcAlarm> activeAlarms = await repository.GetActiveAsync(Pcs7Key);
            PpcAlarm raisedAlarm = PcsAlarmSampleData.CreateAlarm(cachedFaultConfig, 7, "FaultStates.OverCurrent");

            repository.Add(new List<PpcAlarm> { raisedAlarm });
            await repository.SaveChangesAsync();

            Assert.AreSame(activeAlarms[0].PpcAlarmConfig, raisedAlarm.PpcAlarmConfig);
            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            Assert.IsTrue(await verifyContext.PpcAlarms.AnyAsync(alarm => alarm.DeviceId == 7 && alarm.PropertyName == "FaultStates.OverCurrent"));
            Assert.AreEqual(3, await verifyContext.PpcAlarmConfigs.CountAsync());
        }


        /// <summary>
        /// Only the active alarms of the publisher are returned, including the configuration with its actions.
        /// </summary>
        [TestMethod]
        public async Task GetActiveAsyncReturnsActiveAlarmsOfPublisherTest()
        {
            await SeedAlarmsAsync();
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);

            List<PpcAlarm> alarms = await repository.GetActiveAsync(Pcs7Key);

            Assert.HasCount(1, alarms);
            Assert.AreEqual("FaultStates.OverTemperature", alarms[0].PropertyName);
            Assert.AreEqual(7, alarms[0].DeviceId);
            Assert.AreEqual("PCS-FAULT", alarms[0].PpcAlarmConfig.AlarmId);
            Assert.HasCount(2, alarms[0].PpcAlarmConfig.AlarmActions);
        }


        /// <summary>
        /// The active alarms of a publisher without a device (system) are returned.
        /// </summary>
        [TestMethod]
        public async Task GetActiveAsyncReturnsSystemAlarmsWithoutDeviceTest()
        {
            await SeedAlarmsAsync();
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);

            List<PpcAlarm> alarms = await repository.GetActiveAsync(SystemKey);

            Assert.HasCount(1, alarms);
            Assert.IsNull(alarms[0].DeviceId);
            Assert.AreEqual("IsRtuCommunicationLost", alarms[0].PropertyName);
        }


        /// <summary>
        /// The acknowledged state of an alarm is persisted and the alarm stays active.
        /// </summary>
        [TestMethod]
        public async Task SaveChangesAsyncAcknowledgedAlarmStaysActiveTest()
        {
            await SeedAlarmsAsync();
            await using (AlarmContext context = CreateDbContext<AlarmContext>())
            {
                PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
                PpcAlarm alarm = (await repository.GetActiveAsync(Pcs7Key)).Single();
                alarm.Acknowledge("Homer", ChangedOn);

                await repository.SaveChangesAsync();
            }

            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            PpcAlarmRepository verifyRepository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, verifyContext);
            PpcAlarm acknowledgedAlarm = (await verifyRepository.GetActiveAsync(Pcs7Key)).Single();
            Assert.AreEqual(AlarmSequenceState.Acknowledged, acknowledgedAlarm.AlarmSequenceState);
            Assert.AreEqual("Homer", acknowledgedAlarm.ChangedBy);
            Assert.AreEqual(ChangedOn, acknowledgedAlarm.ChangedDate);
        }


        /// <summary>
        /// A normalized alarm is persisted and no longer active.
        /// </summary>
        [TestMethod]
        public async Task SaveChangesAsyncNormalizedAlarmIsNotActiveTest()
        {
            await SeedAlarmsAsync();
            await using (AlarmContext context = CreateDbContext<AlarmContext>())
            {
                PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
                PpcAlarm alarm = (await repository.GetActiveAsync(Pcs7Key)).Single();
                alarm.Normalize("System", ChangedOn);

                await repository.SaveChangesAsync();
            }

            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            PpcAlarmRepository verifyRepository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, verifyContext);
            Assert.IsEmpty(await verifyRepository.GetActiveAsync(Pcs7Key));
            PpcAlarm normalizedAlarm = await verifyContext.PpcAlarms
                .SingleAsync(alarm => alarm.DeviceId == 7 && alarm.PropertyName == "FaultStates.OverTemperature");
            Assert.AreEqual(AlarmSequenceState.Normal, normalizedAlarm.AlarmSequenceState);
            Assert.AreEqual(AlarmProcessCondition.Normal, normalizedAlarm.AlarmProcessCondition);
        }


        /// <summary>
        /// The database rejects a second active alarm of the same device, property and configuration, also without a device.
        /// </summary>
        [TestMethod]
        [DataRow("PCS-FAULT", 7, "FaultStates.OverTemperature")]
        [DataRow("SYS-RTU", null, "IsRtuCommunicationLost")]
        public async Task SaveChangesAsyncSecondActiveAlarmThrowsTest(string alarmId, int? deviceId, string propertyName)
        {
            await SeedAlarmsAsync();
            PpcAlarmConfig config = await GetConfigAsync(alarmId);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
            repository.Add(new List<PpcAlarm> { PcsAlarmSampleData.CreateAlarm(config, deviceId, propertyName) });

            Func<Task> save = async () => await repository.SaveChangesAsync();

            await Assert.ThrowsExactlyAsync<DbUpdateException>(save);
        }


        /// <summary>
        /// A new alarm can be raised for the same device, property and configuration after the previous alarm got normalized.
        /// </summary>
        [TestMethod]
        public async Task SaveChangesAsyncNewAlarmAfterNormalizedAlarmPersistsTest()
        {
            await SeedAlarmsAsync();
            PpcAlarmConfig gridLossConfig = await GetConfigAsync("PCS-GRIDLOSS");
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmRepository repository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
            repository.Add(new List<PpcAlarm> { PcsAlarmSampleData.CreateAlarm(gridLossConfig, 7, "FaultStates.GridLoss") });

            int written = await repository.SaveChangesAsync();

            Assert.AreEqual(1, written);
            Assert.AreEqual(2, await context.PpcAlarms.CountAsync(alarm => alarm.DeviceId == 7 && alarm.PropertyName == "FaultStates.GridLoss"));
        }


        /// <summary>
        /// The evaluation of loaded configurations and active alarms is persisted in one save:
        /// the over temperature fault is normalized and the critical grid loss override raises a new alarm.
        /// </summary>
        [TestMethod]
        public async Task SaveChangesAsyncEvaluationRaisesAndNormalizesTest()
        {
            await SeedAlarmsAsync();
            List<PpcAlarmKeyValue> changedValues = new List<PpcAlarmKeyValue>
            {
                new PpcAlarmKeyValue { Name = "FaultStates.OverTemperature", Value = 0, ValueChanged = true, ValueChangedOn = ChangedOn },
                new PpcAlarmKeyValue { Name = "FaultStates.GridLoss", Value = 1, ValueChanged = true, ValueChangedOn = ChangedOn }
            };

            await using (AlarmContext context = CreateDbContext<AlarmContext>())
            {
                PpcAlarmConfigRepository configRepository = new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, context);
                PpcAlarmRepository alarmRepository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context);
                List<PpcAlarmConfig> configs = await configRepository.GetEnabledAsync(Pcs7Key.PpcAlarmType);
                List<PpcAlarm> activeAlarms = await alarmRepository.GetActiveAsync(Pcs7Key);
                PpcAlarmEvaluation evaluation = new PpcAlarmEvaluator().Evaluate(Pcs7Key, changedValues, configs, activeAlarms);
                alarmRepository.Add(evaluation.Raised);

                await alarmRepository.SaveChangesAsync();
            }

            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            PpcAlarmRepository verifyRepository = new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, verifyContext);
            PpcAlarm activeAlarm = (await verifyRepository.GetActiveAsync(Pcs7Key)).Single();
            Assert.AreEqual("FaultStates.GridLoss", activeAlarm.PropertyName);
            Assert.AreEqual(AlarmSeverity.Critical, activeAlarm.SeverityLevel);
            Assert.AreEqual(ChangedOn, activeAlarm.CreatedOn);
        }


        /// <summary>
        /// Seeds an active over temperature alarm and a normalized grid loss alarm of PCS 7,
        /// an active over temperature alarm of PCS 8 and an active RTU system alarm.
        /// </summary>
        private async Task SeedAlarmsAsync()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfig faultConfig = await context.PpcAlarmConfigs.SingleAsync(config => config.AlarmId == "PCS-FAULT");
            PpcAlarmConfig gridLossConfig = await context.PpcAlarmConfigs.SingleAsync(config => config.AlarmId == "PCS-GRIDLOSS");
            PpcAlarmConfig rtuConfig = await context.PpcAlarmConfigs.SingleAsync(config => config.AlarmId == "SYS-RTU");
            PpcAlarm normalizedAlarm = PcsAlarmSampleData.CreateAlarm(gridLossConfig, 7, "FaultStates.GridLoss");
            normalizedAlarm.Normalize("System", PcsAlarmSampleData.ChangedOn.AddMinutes(1));
            context.PpcAlarms.Add(PcsAlarmSampleData.CreateAlarm(faultConfig, 7, "FaultStates.OverTemperature"));
            context.PpcAlarms.Add(normalizedAlarm);
            context.PpcAlarms.Add(PcsAlarmSampleData.CreateAlarm(faultConfig, 8, "FaultStates.OverTemperature"));
            context.PpcAlarms.Add(PcsAlarmSampleData.CreateAlarm(rtuConfig, null, "IsRtuCommunicationLost"));
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Gets a configuration with its actions from a disposed context (detached).
        /// </summary>
        private async Task<PpcAlarmConfig> GetConfigAsync(string alarmId)
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();

            return await context.PpcAlarmConfigs
                .Include(config => config.AlarmActions)
                .ThenInclude(action => action.AlarmActionParameters)
                .AsNoTracking()
                .SingleAsync(config => config.AlarmId == alarmId);
        }
    }
}
