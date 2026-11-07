// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using paskalON.Alarms.Application.Caches;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Application.Stores;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Alarms.Application.IntegrationTest.Handlers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmHandler"/> with the configuration and active alarm caches against PostgreSQL,
    /// every received data is evaluated in a new database context like in the service.
    /// </summary>
    [TestClass]
    public class PpcAlarmHandlerTest : DatabaseTestBase
    {
        /// <summary>
        /// Key of PCS 7.
        /// </summary>
        private static readonly PpcAlarmKey Pcs7Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 7 };


        /// <summary>
        /// Time when the configuration was created.
        /// </summary>
        private static readonly DateTimeOffset ConfiguredOn = new DateTimeOffset(2026, 11, 6, 8, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// Time when the first data was received.
        /// </summary>
        private static readonly DateTimeOffset ReceivedOn = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero);


        /// <summary>
        /// Configuration cache, a singleton in the service.
        /// </summary>
        private readonly PpcAlarmConfigCache _configCache = new PpcAlarmConfigCache(NullLogger<PpcAlarmConfigCache>.Instance);


        /// <summary>
        /// Active alarm cache, a singleton in the service.
        /// </summary>
        private readonly PpcAlarmActiveCache _activeCache = new PpcAlarmActiveCache(NullLogger<PpcAlarmActiveCache>.Instance);


        /// <summary>
        /// Stateless store, a singleton in the service.
        /// </summary>
        private readonly PpcAlarmValueStore _valueStore = new PpcAlarmValueStore();


        /// <summary>
        /// Outbox signal, a singleton in the service.
        /// </summary>
        private readonly AlarmActionOutboxSignal _outboxSignal = new AlarmActionOutboxSignal(new FakeTimeProvider(ReceivedOn));


        /// <summary>
        /// Lock of the alarm writers, a singleton in the service.
        /// </summary>
        private readonly PpcAlarmWriteLock _writeLock = new PpcAlarmWriteLock();


        /// <summary>
        /// Time when orphaned alarms are normalized.
        /// </summary>
        private static readonly DateTimeOffset NormalizedOn = new DateTimeOffset(2026, 11, 6, 9, 30, 0, TimeSpan.Zero);


        /// <summary>
        /// Seeds the PCS fault configuration with the raise action ReducePower and the normalize action ResetPower.
        /// </summary>
        protected override async Task SeedAsync(AlarmContext db)
        {
            PpcAlarmConfig config = new PpcAlarmConfig
            {
                ChangedBy = "Homer",
                ChangedDate = ConfiguredOn,
                IsEnabled = true,
                AlarmId = "PCS-FAULT",
                Name = "{{PCS.Fault.Name}}",
                SeverityLevel = AlarmSeverity.Error,
                Description = "{{PCS.Fault.Desc}}",
                PpcAlarmType = PpcAlarmType.PcsAlarm,
                PropertyName = "FaultStates",
                PropertyOperator = PropertyOperator.Equal,
                PropertyValue = 1
            };
            config.AlarmActions.Add(CreateAction("ReducePower", AlarmActionTrigger.Raise));
            config.AlarmActions.Add(CreateAction("ResetPower", AlarmActionTrigger.Normalize));
            db.PpcAlarmConfigs.Add(config);
            await db.SaveChangesAsync();
        }


        /// <summary>
        /// An alarm raised with one data is normalized from the cache with the next data in another context,
        /// both changes and their actions are saved and the configuration is not written.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncRaiseThenNormalizeFromCachePersistsTest()
        {
            await LoadCachesAsync();
            await HandleAsync(CreatePcsDetail(overTemperature: true, gridLoss: false), ReceivedOn);

            PpcAlarmEvaluation evaluation = await HandleAsync(CreatePcsDetail(overTemperature: false, gridLoss: false), ReceivedOn.AddSeconds(5));

            Assert.HasCount(1, evaluation.Normalized);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarm alarm = await context.PpcAlarms.SingleAsync();
            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
            Assert.AreEqual(ReceivedOn.AddSeconds(5), alarm.ChangedDate);
            CollectionAssert.AreEqual(new List<string> { "ReducePower", "ResetPower" }, await GetActionNamesAsync(context));
            Assert.AreEqual(ConfiguredOn, (await context.PpcAlarmConfigs.SingleAsync()).ChangedDate);
            Assert.IsEmpty(await _activeCache.GetActiveAsync(Pcs7Key, new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context)));
        }


        /// <summary>
        /// After a restart the cached active alarm and the cached configuration are different instances of the same
        /// configuration: one data normalizes the active alarm and raises another alarm of the configuration in one save.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncNormalizeAndRaiseOfSameConfigAfterRestartPersistsTest()
        {
            await LoadCachesAsync();
            await HandleAsync(CreatePcsDetail(overTemperature: true, gridLoss: false), ReceivedOn);
            PpcAlarmConfigCache restartedConfigCache = new PpcAlarmConfigCache(NullLogger<PpcAlarmConfigCache>.Instance);
            PpcAlarmActiveCache restartedActiveCache = new PpcAlarmActiveCache(NullLogger<PpcAlarmActiveCache>.Instance);
            await LoadCachesAsync(restartedConfigCache, restartedActiveCache);

            PpcAlarmEvaluation evaluation = await HandleAsync(CreatePcsDetail(overTemperature: false, gridLoss: true), ReceivedOn.AddSeconds(5),
                restartedConfigCache, restartedActiveCache, new PpcAlarmValueStore());

            Assert.HasCount(1, evaluation.Normalized);
            Assert.HasCount(1, evaluation.Raised);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarm activeAlarm = (await new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context).GetActiveAsync(Pcs7Key)).Single();
            Assert.AreEqual("FaultStates.GridLoss", activeAlarm.PropertyName);
            Assert.AreEqual(2, await context.PpcAlarms.CountAsync());
            CollectionAssert.AreEquivalent(new List<string> { "ReducePower", "ResetPower", "ReducePower" }, await GetActionNamesAsync(context));
            Assert.AreEqual(1, await context.PpcAlarmConfigs.CountAsync());
        }


        /// <summary>
        /// After the configuration of an active alarm was disabled and the configurations reloaded, the orphaned alarm is
        /// normalized with its normalize action. The stale cached configuration is not written back.
        /// </summary>
        [TestMethod]
        public async Task NormalizeOrphanedAsyncAfterConfigDisabledNormalizesAlarmTest()
        {
            await LoadCachesAsync();
            await HandleAsync(CreatePcsDetail(overTemperature: true, gridLoss: false), ReceivedOn);
            await DisableConfigAsync();
            await using (AlarmContext reloadContext = CreateDbContext<AlarmContext>())
            {
                await _configCache.LoadAsync(new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, reloadContext));
            }

            await using AlarmContext context = CreateDbContext<AlarmContext>();
            int normalizedCount = await CreateHandler(context, _configCache, _activeCache, _valueStore).NormalizeOrphanedAsync();

            Assert.AreEqual(1, normalizedCount);
            await using AlarmContext verifyContext = CreateDbContext<AlarmContext>();
            PpcAlarm alarm = await verifyContext.PpcAlarms.SingleAsync();
            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
            Assert.AreEqual(NormalizedOn, alarm.ChangedDate);
            CollectionAssert.AreEqual(new List<string> { "ReducePower", "ResetPower" }, await GetActionNamesAsync(verifyContext));
            Assert.IsFalse((await verifyContext.PpcAlarmConfigs.SingleAsync()).IsEnabled);
        }


        /// <summary>
        /// Disables the PCS fault configuration in the database, like an operator in Luna.
        /// </summary>
        private async Task DisableConfigAsync()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfig config = await context.PpcAlarmConfigs.SingleAsync();
            config.IsEnabled = false;
            config.ChangedBy = "Marge";
            config.ChangedDate = ReceivedOn.AddMinutes(10);
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Loads the caches of this test from the database.
        /// </summary>
        private async Task LoadCachesAsync()
        {
            await LoadCachesAsync(_configCache, _activeCache);
        }


        /// <summary>
        /// Loads caches from the database like at startup.
        /// </summary>
        private async Task LoadCachesAsync(PpcAlarmConfigCache configCache, PpcAlarmActiveCache activeCache)
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            await configCache.LoadAsync(new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, context));
            await activeCache.LoadAsync(new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context));
        }


        /// <summary>
        /// Evaluates received data with the caches of this test in a new context.
        /// </summary>
        private async Task<PpcAlarmEvaluation> HandleAsync(PcsDetailDto detail, DateTimeOffset receivedOn)
        {
            return await HandleAsync(detail, receivedOn, _configCache, _activeCache, _valueStore);
        }


        /// <summary>
        /// Evaluates received data in a new context (scope), like the alarm handler service.
        /// </summary>
        private async Task<PpcAlarmEvaluation> HandleAsync(PcsDetailDto detail, DateTimeOffset receivedOn,
            PpcAlarmConfigCache configCache, PpcAlarmActiveCache activeCache, PpcAlarmValueStore valueStore)
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmHandler handler = CreateHandler(context, configCache, activeCache, valueStore);

            return await handler.HandleAsync(Pcs7Key, detail, receivedOn);
        }


        /// <summary>
        /// Creates the handler of a context (scope) with the singletons.
        /// </summary>
        private PpcAlarmHandler CreateHandler(AlarmContext context, PpcAlarmConfigCache configCache, PpcAlarmActiveCache activeCache,
            PpcAlarmValueStore valueStore)
        {
            return new PpcAlarmHandler(NullLogger<PpcAlarmHandler>.Instance, valueStore, new PpcAlarmEvaluator(),
                configCache, activeCache, new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context),
                new AlarmActionOutboxRepository(NullLogger<AlarmActionOutboxRepository>.Instance, context), _outboxSignal,
                _writeLock, new FakeTimeProvider(NormalizedOn));
        }


        /// <summary>
        /// Gets the action names of the outbox messages in creation order.
        /// </summary>
        private static async Task<List<string>> GetActionNamesAsync(AlarmContext context)
        {
            List<AlarmActionOutboxMessage> messages = await context.AlarmActionOutboxMessages.OrderBy(message => message.CreatedOn).ThenBy(message => message.Id).ToListAsync();

            return messages.Select(message => message.ActionName).ToList();
        }


        /// <summary>
        /// Creates an action with the device id parameter.
        /// </summary>
        private static AlarmActionConfig CreateAction(string name, AlarmActionTrigger trigger)
        {
            AlarmActionConfig action = new AlarmActionConfig
            {
                ChangedBy = "Homer",
                ChangedDate = ConfiguredOn,
                Name = name,
                Trigger = trigger
            };
            action.AlarmActionParameters.Add(new AlarmActionParameterConfig
            {
                ChangedBy = "Homer",
                ChangedDate = ConfiguredOn,
                AlarmActionConfig = action,
                Key = "DeviceId",
                Value = "{{DeviceId}}"
            });

            return action;
        }


        /// <summary>
        /// Creates a PCS detail of PCS 7 with the over temperature and grid loss fault states.
        /// </summary>
        private static PcsDetailDto CreatePcsDetail(bool overTemperature, bool gridLoss)
        {
            return new PcsDetailDto
            {
                DeviceId = 7,
                IsInMaintenanceMode = false,
                IsACBreakerClosed = true,
                FaultStates = new Dictionary<string, bool> { { "OverTemperature", overTemperature }, { "GridLoss", gridLoss } },
                WarningStates = new Dictionary<string, bool> { { "FanFailure", false } },
                VendorEvents = new Dictionary<string, bool> { { "FirmwareUpdate", false } }
            };
        }
    }
}
