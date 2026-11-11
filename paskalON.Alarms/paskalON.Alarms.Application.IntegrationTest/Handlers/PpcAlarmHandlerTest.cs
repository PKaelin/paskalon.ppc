// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using paskalON.Alarms.Application.Caches;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.IntegrationTest.TestDoubles;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Application.Stores;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Devices.Dto.PowerConversionSystems;
using System.Text.Json;

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
        /// Fake time of the handlers (orphan normalization, delayed alarms and summaries), after the received data.
        /// </summary>
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(NormalizedOn);


        /// <summary>
        /// Delayed raises and normalizations shared by the handlers like the singleton in the service.
        /// </summary>
        private readonly PpcAlarmPendingTransitions _pendingTransitions = new PpcAlarmPendingTransitions();


        /// <summary>
        /// Device client with PCS 7, a singleton in the service initialized with the DER of the device service.
        /// </summary>
        private readonly InMemoryDeviceClient _deviceClient = new InMemoryDeviceClient();


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
            config.AlarmActions.Add(CreateAction("AcknowledgeAlarm", AlarmActionTrigger.Acknowledge));
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
            List<AlarmActionDto> actions = (await context.AlarmActionOutboxMessages.OrderBy(message => message.CreatedOn).ToListAsync())
                .Select(message => JsonSerializer.Deserialize<AlarmActionDto>(message.Payload)!)
                .ToList();
            CollectionAssert.AreEqual(new List<int> { 1, 0 }, actions.Select(action => action.AlarmSummary.ErrorCount).ToList());
            Assert.AreEqual(ConfiguredOn, (await context.PpcAlarmConfigs.SingleAsync()).ChangedDate);
            Assert.IsEmpty(await _activeCache.GetActiveAsync(Pcs7Key, new PpcAlarmRepository(NullLogger<PpcAlarmRepository>.Instance, context)));
        }


        /// <summary>
        /// A raised alarm is saved with the name of its device and keeps it when it is normalized in another context.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncRaisedAlarmPersistsDeviceNameTest()
        {
            _deviceClient.PowerConversionSystems.Add(new PcsDto(new PcsDefinitionDto { DeviceId = 7, TargetAddress = "10.0.0.7", TargetPort = 502, Name = "PCS Bess 7" }));
            await LoadCachesAsync();
            await HandleAsync(CreatePcsDetail(overTemperature: true, gridLoss: false), ReceivedOn);

            await HandleAsync(CreatePcsDetail(overTemperature: false, gridLoss: false), ReceivedOn.AddSeconds(5));

            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarm alarm = await context.PpcAlarms.SingleAsync();
            Assert.AreEqual(7, alarm.DeviceId);
            Assert.AreEqual("PCS Bess 7", alarm.DeviceName);
            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
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
        /// With a raise delay nothing is persisted when the fault state turns true. Once the delay elapsed the due handling
        /// persists the alarm with the time the fault state turned true and its raise action.
        /// </summary>
        [TestMethod]
        public async Task HandleDueAsyncAfterRaiseDelayPersistsAlarmAtAbnormalTimeTest()
        {
            await SetRaiseDelayAsync(3000);
            await LoadCachesAsync();
            PpcAlarmEvaluation delayedEvaluation = await HandleAsync(CreatePcsDetail(overTemperature: true, gridLoss: false), ReceivedOn);
            await using AlarmContext beforeDueContext = CreateDbContext<AlarmContext>();
            int alarmCountBeforeDue = await beforeDueContext.PpcAlarms.CountAsync();

            int changedCount = await HandleDueAsync();

            Assert.IsFalse(delayedEvaluation.HasChanges);
            Assert.AreEqual(0, alarmCountBeforeDue);
            Assert.AreEqual(1, changedCount);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarm alarm = await context.PpcAlarms.SingleAsync();
            Assert.AreEqual("FaultStates.OverTemperature", alarm.PropertyName);
            Assert.AreEqual(ReceivedOn, alarm.CreatedOn);
            Assert.AreEqual(AlarmSequenceState.Alarm, alarm.AlarmSequenceState);
            CollectionAssert.AreEqual(new List<string> { "ReducePower" }, await GetActionNamesAsync(context));
        }


        /// <summary>
        /// A raised alarm is acknowledged from the cache in another context and then normalized: the acknowledgement is persisted
        /// with its acknowledge action and kept after the normalization, the configuration is not written.
        /// </summary>
        [TestMethod]
        public async Task AcknowledgeAsyncRaisedAlarmThenNormalizePersistsAcknowledgementTest()
        {
            await LoadCachesAsync();
            await HandleAsync(CreatePcsDetail(overTemperature: true, gridLoss: false), ReceivedOn);
            await using AlarmContext raisedContext = CreateDbContext<AlarmContext>();
            int alarmId = (await raisedContext.PpcAlarms.SingleAsync()).Id;

            int acknowledgedCount = await AcknowledgeAsync(alarmId);
            await HandleAsync(CreatePcsDetail(overTemperature: false, gridLoss: false), ReceivedOn.AddSeconds(5));

            Assert.AreEqual(1, acknowledgedCount);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarm alarm = await context.PpcAlarms.SingleAsync();
            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
            Assert.AreEqual("Operator", alarm.AcknowledgedBy);
            Assert.AreEqual(NormalizedOn, alarm.AcknowledgedOn);
            CollectionAssert.AreEquivalent(new List<string> { "ReducePower", "AcknowledgeAlarm", "ResetPower" }, await GetActionNamesAsync(context));
            Assert.AreEqual(ConfiguredOn, (await context.PpcAlarmConfigs.SingleAsync()).ChangedDate);
        }


        /// <summary>
        /// Acknowledges an alarm in a new context (scope), like the web API.
        /// </summary>
        private async Task<int> AcknowledgeAsync(int alarmId)
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmHandler handler = CreateHandler(context, _configCache, _activeCache, _valueStore);

            return await handler.AcknowledgeAsync(new List<int> { alarmId }, "Operator");
        }


        /// <summary>
        /// Sets the raise delay of the PCS fault configuration in the database.
        /// </summary>
        private async Task SetRaiseDelayAsync(long raiseDelayMilliseconds)
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfig config = await context.PpcAlarmConfigs.SingleAsync();
            config.TimeBeforeRaisingAlarmMilliseconds = raiseDelayMilliseconds;
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Raises and normalizes the due delayed alarms in a new context (scope), like the delay service.
        /// </summary>
        private async Task<int> HandleDueAsync()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmHandler handler = CreateHandler(context, _configCache, _activeCache, _valueStore);

            return await handler.HandleDueAsync();
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
                _writeLock, _timeProvider, _pendingTransitions, _deviceClient);
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
