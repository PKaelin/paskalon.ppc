// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Alarms.Application.Caches;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Application.Stores;
using paskalON.Alarms.Application.UnitTest.SampleData;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Devices.Client;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.PowerConversionSystems;
using System.Text.Json;

namespace paskalON.Alarms.Application.UnitTest.Handlers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmHandler"/> with the real store, reader, evaluator and active alarm cache and mocked repositories.
    /// </summary>
    [TestClass]
    public class PpcAlarmHandlerTest
    {
        /// <summary>
        /// Key of PCS 7.
        /// </summary>
        private static readonly PpcAlarmKey Pcs7Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 7 };


        /// <summary>
        /// Timestamp when the data was received.
        /// </summary>
        private static readonly DateTimeOffset ReceivedOn = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero);


        /// <summary>
        /// Mocked configuration cache.
        /// </summary>
        private readonly Mock<IPpcAlarmConfigCache> _configCache = new Mock<IPpcAlarmConfigCache>();


        /// <summary>
        /// Mocked alarm repository.
        /// </summary>
        private readonly Mock<IPpcAlarmRepository> _alarmRepository = new Mock<IPpcAlarmRepository>();


        /// <summary>
        /// Mocked outbox repository.
        /// </summary>
        private readonly Mock<IAlarmActionOutboxRepository> _outboxRepository = new Mock<IAlarmActionOutboxRepository>();


        /// <summary>
        /// Mocked outbox signal.
        /// </summary>
        private readonly Mock<IAlarmActionOutboxSignal> _outboxSignal = new Mock<IAlarmActionOutboxSignal>();


        /// <summary>
        /// Mocked device client with PCS 7 and battery bank 7 (the same device id of another device type).
        /// </summary>
        private readonly Mock<IDeviceClient> _deviceClient = new Mock<IDeviceClient>();


        /// <summary>
        /// Alarms added to the alarm repository.
        /// </summary>
        private readonly List<PpcAlarm> _addedAlarms = new List<PpcAlarm>();


        /// <summary>
        /// Messages added to the outbox repository.
        /// </summary>
        private readonly List<AlarmActionOutboxMessage> _addedMessages = new List<AlarmActionOutboxMessage>();


        /// <summary>
        /// Alarms updated in the alarm repository.
        /// </summary>
        private readonly List<PpcAlarm> _updatedAlarms = new List<PpcAlarm>();


        /// <summary>
        /// Active alarms in the database, loaded into the active alarm cache.
        /// </summary>
        private readonly List<PpcAlarm> _savedActiveAlarms = new List<PpcAlarm>();


        /// <summary>
        /// Real active alarm cache, loaded from the mocked alarm repository.
        /// </summary>
        private readonly PpcAlarmActiveCache _activeCache = new PpcAlarmActiveCache(NullLogger<PpcAlarmActiveCache>.Instance);


        /// <summary>
        /// Real lock of the alarm writers.
        /// </summary>
        private readonly PpcAlarmWriteLock _writeLock = new PpcAlarmWriteLock();


        /// <summary>
        /// Time when the orphaned alarms are normalized.
        /// </summary>
        private static readonly DateTimeOffset NormalizedOn = new DateTimeOffset(2026, 11, 6, 9, 30, 0, TimeSpan.Zero);


        /// <summary>
        /// Time the data of the delay tests was received, after the start of the fake time so that it can be advanced to the due time.
        /// </summary>
        private static readonly DateTimeOffset DelayedReceivedOn = NormalizedOn.AddMinutes(1);


        /// <summary>
        /// Fake time of the orphan normalization, the delayed alarms and the summaries.
        /// </summary>
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(NormalizedOn);


        /// <summary>
        /// Delayed raises and normalizations of the handler.
        /// </summary>
        private readonly PpcAlarmPendingTransitions _pendingTransitions = new PpcAlarmPendingTransitions();


        /// <summary>
        /// Sets up the repositories with the PCS fault configuration and without active alarms.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm))
                .Returns(() => new List<PpcAlarmConfig> { PcsAlarmConfigSampleData.CreatePcsFaultConfig() });
            _alarmRepository.Setup(repository => repository.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<PpcAlarm>(_savedActiveAlarms));
            _alarmRepository.Setup(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<PpcAlarm>(_savedActiveAlarms));
            _alarmRepository.Setup(repository => repository.Add(It.IsAny<IEnumerable<PpcAlarm>>()))
                .Callback<IEnumerable<PpcAlarm>>(alarms => _addedAlarms.AddRange(alarms));
            _alarmRepository.Setup(repository => repository.Update(It.IsAny<IEnumerable<PpcAlarm>>()))
                .Callback<IEnumerable<PpcAlarm>>(alarms => _updatedAlarms.AddRange(alarms));
            _alarmRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _outboxRepository.Setup(repository => repository.Add(It.IsAny<IEnumerable<AlarmActionOutboxMessage>>()))
                .Callback<IEnumerable<AlarmActionOutboxMessage>>(messages => _addedMessages.AddRange(messages));
            _deviceClient.Setup(client => client.PowerConversionSystems).Returns(new List<PcsDto>
            {
                new PcsDto(new PcsDefinitionDto { DeviceId = 7, TargetAddress = "10.0.0.7", TargetPort = 502, Name = "PCS Bess 7" })
            });
            _deviceClient.Setup(client => client.BatteryBanks).Returns(new List<BbDto>
            {
                new BbDto(new BbDefinitionDto { DeviceId = 7, TargetAddress = "10.0.0.70", TargetPort = 502, Name = "BB 7.1" })
            });
        }


        /// <summary>
        /// A fault state that turns true raises an alarm and adds its raise action to the outbox in one save.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncFaultStateRaisesAlarmAndAddsRaiseActionTest()
        {
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn);

            Assert.HasCount(1, evaluation.Raised);
            Assert.HasCount(1, _addedAlarms);
            Assert.AreEqual("FaultStates.OverTemperature", _addedAlarms[0].PropertyName);
            Assert.HasCount(1, _addedMessages);
            AlarmActionOutboxMessage message = _addedMessages[0];
            Assert.AreEqual("ReducePower", message.ActionName);
            Assert.AreSame(_addedAlarms[0], message.PpcAlarm);
            Assert.AreEqual(ReceivedOn, message.CreatedOn);
            Assert.IsNull(message.SentOn);
            AlarmActionDto? action = JsonSerializer.Deserialize<AlarmActionDto>(message.Payload);
            Assert.IsNotNull(action);
            Assert.AreEqual(message.MessageId, action.MessageId);
            Assert.AreEqual("7", action.AlarmActionParameters.Single(parameter => parameter.Key == "DeviceId").Value);
            Assert.AreEqual(NormalizedOn, action.AlarmSummary.CalculatedOn);
            Assert.AreEqual(1, action.AlarmSummary.ErrorCount);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _outboxSignal.Verify(signal => signal.Notify(), Times.Once);
            List<PpcAlarm> cachedAlarms = await _activeCache.GetActiveAsync(Pcs7Key, _alarmRepository.Object);
            Assert.AreSame(_addedAlarms[0], cachedAlarms.Single());
        }


        /// <summary>
        /// A raised alarm gets the name of the device of its type with its device id, the power control alarms the name of the PCS.
        /// A device that is not in the DER leaves the name empty.
        /// </summary>
        /// <param name="ppcAlarmType">PPC alarm type of the publisher.</param>
        /// <param name="deviceId">Device id of the publisher.</param>
        /// <param name="expectedDeviceName">Expected device name of the raised alarm.</param>
        [TestMethod]
        [DataRow(PpcAlarmType.PcsAlarm, 7, "PCS Bess 7")]
        [DataRow(PpcAlarmType.BatteryAlarm, 7, "BB 7.1")]
        [DataRow(PpcAlarmType.PowerControlAlarm, 7, "PCS Bess 7")]
        [DataRow(PpcAlarmType.PcsAlarm, 8, null)]
        [DataRow(PpcAlarmType.SolarPanelAlarm, 7, null)]
        public async Task HandleAsyncRaisedAlarmGetsDeviceNameOfTypeTest(PpcAlarmType ppcAlarmType, int deviceId, string? expectedDeviceName)
        {
            PpcAlarmKey key = new PpcAlarmKey { PpcAlarmType = ppcAlarmType, DeviceId = deviceId };
            // The fault configuration with the alarm type of the publisher, the evaluator only uses configurations of its type
            PpcAlarmConfig config = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            config.PpcAlarmType = ppcAlarmType;
            _configCache.Setup(cache => cache.GetEnabled(ppcAlarmType)).Returns(new List<PpcAlarmConfig> { config });
            _alarmRepository.Setup(repository => repository.GetActiveAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PpcAlarm>());
            _deviceClient.Setup(client => client.SolarPanels).Returns(new List<PvDto>());
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            await handler.HandleAsync(key, CreatePcsDetail(overTemperature: true), ReceivedOn);

            PpcAlarm alarm = _addedAlarms.Single();
            Assert.AreEqual(deviceId, alarm.DeviceId);
            Assert.AreEqual(expectedDeviceName, alarm.DeviceName);
        }


        /// <summary>
        /// A fault state that turns false normalizes the active alarm and adds its normalize action to the outbox.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncFaultStateClearedNormalizesAlarmAndAddsNormalizeActionTest()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            PpcAlarm activeAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm))
                .Returns(new List<PpcAlarmConfig> { faultConfig });
            _savedActiveAlarms.Add(activeAlarm);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: false), ReceivedOn);

            Assert.HasCount(1, evaluation.Normalized);
            Assert.AreEqual(AlarmSequenceState.Normal, activeAlarm.AlarmSequenceState);
            Assert.IsEmpty(_addedAlarms);
            Assert.AreSame(activeAlarm, _updatedAlarms.Single());
            Assert.HasCount(1, _addedMessages);
            Assert.AreEqual("ResetPower", _addedMessages[0].ActionName);
            Assert.AreSame(activeAlarm, _addedMessages[0].PpcAlarm);
            Assert.AreEqual(0, JsonSerializer.Deserialize<AlarmActionDto>(_addedMessages[0].Payload)!.AlarmSummary.ErrorCount);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _outboxSignal.Verify(signal => signal.Notify(), Times.Once);
            Assert.IsEmpty(await _activeCache.GetActiveAsync(Pcs7Key, _alarmRepository.Object));
        }


        /// <summary>
        /// The summary sent with the actions counts the alarms of all publishers after the evaluation.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncRaisedAlarmActionContainsSummaryOfAllPublishersTest()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm))
                .Returns(new List<PpcAlarmConfig> { faultConfig });
            _savedActiveAlarms.Add(PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 8, "FaultStates.GridLoss"));
            _savedActiveAlarms.Add(PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 9, "FaultStates.GridLoss"));
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn);

            AlarmActionDto action = JsonSerializer.Deserialize<AlarmActionDto>(_addedMessages.Single().Payload)!;
            Assert.AreEqual(3, action.AlarmSummary.ErrorCount);
            Assert.AreEqual(0, action.AlarmSummary.WarningCount);
        }


        /// <summary>
        /// When saving fails the evaluation was already applied to the cache for the summary, the publisher is invalidated
        /// so that the summary is calculated again from the saved alarms.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncSaveFailsSummaryCountsSavedAlarmsTest()
        {
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            _alarmRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            await Assert.ThrowsExactlyAsync<TimeoutException>(() => handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn));

            AlarmSummaryDto summary = await handler.GetSummaryAsync();

            Assert.AreEqual(0, summary.ErrorCount);
            Assert.AreEqual(NormalizedOn, summary.CalculatedOn);
            _alarmRepository.Verify(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// When saving a normalization fails the cached alarm is not used again, the next data is evaluated against
        /// the active alarm loaded again from the database.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncSaveNormalizationFailsReloadsActiveAlarmTest()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm))
                .Returns(new List<PpcAlarmConfig> { faultConfig });
            PpcAlarm cachedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            _savedActiveAlarms.Add(cachedAlarm);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            PpcAlarm savedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            _savedActiveAlarms[0] = savedAlarm;
            _alarmRepository.SetupSequence(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."))
                .ReturnsAsync(2);
            await Assert.ThrowsExactlyAsync<TimeoutException>(() => handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: false), ReceivedOn));
            _updatedAlarms.Clear();

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: false), ReceivedOn.AddSeconds(1));

            Assert.AreSame(savedAlarm, evaluation.Normalized.Single());
            Assert.AreSame(savedAlarm, _updatedAlarms.Single());
            _alarmRepository.Verify(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// A raised alarm without configured actions is saved but does not wake up the outbox publisher service.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncAlarmWithoutActionsDoesNotNotifyTest()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            faultConfig.AlarmActions.Clear();
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm))
                .Returns(new List<PpcAlarmConfig> { faultConfig });
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn);

            Assert.HasCount(1, evaluation.Raised);
            Assert.IsEmpty(_addedMessages);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _outboxSignal.Verify(signal => signal.Notify(), Times.Never);
        }


        /// <summary>
        /// Unchanged data is not evaluated again and does not access the database.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncUnchangedDataDoesNotEvaluateTest()
        {
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn);

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn.AddSeconds(1));

            Assert.IsFalse(evaluation.HasChanges);
            _configCache.Verify(cache => cache.GetEnabled(It.IsAny<PpcAlarmType>()), Times.Once);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// The configurations and active alarms come from the caches, an evaluation without changes does not query the database.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncChangedValuesUseCachesWithoutQueryTest()
        {
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: false), ReceivedOn);

            _alarmRepository.Verify(repository => repository.GetActiveAsync(It.IsAny<PpcAlarmKey>(), It.IsAny<CancellationToken>()), Times.Never);
            _alarmRepository.Verify(repository => repository.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// Changed values without a matching configuration do not save anything.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncChangedValuesWithoutAlarmDoesNotSaveTest()
        {
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: false), ReceivedOn);

            Assert.IsFalse(evaluation.HasChanges);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _outboxRepository.Verify(repository => repository.Add(It.IsAny<IEnumerable<AlarmActionOutboxMessage>>()), Times.Never);
        }


        /// <summary>
        /// When saving fails (e.g. database not available) the same data is evaluated again with the next reception, so no alarm gets lost.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncSaveFailsEvaluatesSameDataAgainTest()
        {
            _alarmRepository.SetupSequence(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."))
                .ReturnsAsync(2);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            await Assert.ThrowsExactlyAsync<TimeoutException>(() => handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn));
            _addedAlarms.Clear();
            _addedMessages.Clear();

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn.AddSeconds(1));

            Assert.HasCount(1, evaluation.Raised);
            Assert.HasCount(1, _addedAlarms);
            Assert.HasCount(1, _addedMessages);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            // Only the successful save notifies, the failed save has no committed messages
            _outboxSignal.Verify(signal => signal.Notify(), Times.Once);
            // The failed save invalidated the active alarms of the publisher, they are loaded again once
            _alarmRepository.Verify(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// An active alarm whose configuration is no longer enabled is normalized with its normalize action,
        /// the alarm of an enabled configuration stays active.
        /// </summary>
        [TestMethod]
        public async Task NormalizeOrphanedAsyncNormalizesAlarmsOfRemovedConfigsTest()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            PpcAlarmConfig removedConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            removedConfig.Id = 2;
            removedConfig.AlarmId = "PCS-REMOVED";
            PpcAlarm activeAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            PpcAlarm orphanedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(removedConfig, 7, "FaultStates.GridLoss");
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm)).Returns(new List<PpcAlarmConfig> { faultConfig });
            _savedActiveAlarms.Add(activeAlarm);
            _savedActiveAlarms.Add(orphanedAlarm);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            int normalizedCount = await handler.NormalizeOrphanedAsync();

            Assert.AreEqual(1, normalizedCount);
            Assert.AreEqual(AlarmSequenceState.Normal, orphanedAlarm.AlarmSequenceState);
            Assert.AreEqual(NormalizedOn, orphanedAlarm.ChangedDate);
            Assert.AreEqual(PpcAlarmEvaluator.SystemUser, orphanedAlarm.ChangedBy);
            Assert.AreEqual(AlarmSequenceState.Alarm, activeAlarm.AlarmSequenceState);
            Assert.AreSame(orphanedAlarm, _updatedAlarms.Single());
            Assert.AreEqual("ResetPower", _addedMessages.Single().ActionName);
            _outboxSignal.Verify(signal => signal.Notify(), Times.Once);
            Assert.AreSame(activeAlarm, (await _activeCache.GetActiveAsync(Pcs7Key, _alarmRepository.Object)).Single());
        }


        /// <summary>
        /// Without orphaned alarms nothing is saved.
        /// </summary>
        [TestMethod]
        public async Task NormalizeOrphanedAsyncWithoutOrphanedAlarmsSavesNothingTest()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm)).Returns(new List<PpcAlarmConfig> { faultConfig });
            _savedActiveAlarms.Add(PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature"));
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            int normalizedCount = await handler.NormalizeOrphanedAsync();

            Assert.AreEqual(0, normalizedCount);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// With a raise delay the received fault state saves nothing, once the delay elapsed the due handling raises and saves
        /// the alarm with its raise action, created at the time the fault state turned true.
        /// </summary>
        [TestMethod]
        public async Task HandleDueAsyncRaisesDelayedAlarmAfterDelayTest()
        {
            PpcAlarmConfig faultConfig = SetupDelayedFaultConfig(raiseDelayMilliseconds: 3000);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            PpcAlarmEvaluation delayedEvaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), DelayedReceivedOn);
            _timeProvider.SetUtcNow(DelayedReceivedOn.AddSeconds(3));

            int changedCount = await handler.HandleDueAsync();

            Assert.IsFalse(delayedEvaluation.HasChanges);
            Assert.AreEqual(1, changedCount);
            PpcAlarm raisedAlarm = _addedAlarms.Single();
            Assert.AreSame(faultConfig, raisedAlarm.PpcAlarmConfig);
            Assert.AreEqual(DelayedReceivedOn, raisedAlarm.CreatedOn);
            Assert.AreEqual("ReducePower", _addedMessages.Single().ActionName);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _outboxSignal.Verify(signal => signal.Notify(), Times.Once);
            Assert.AreEqual(0, _pendingTransitions.Count);
        }


        /// <summary>
        /// Before the delay elapsed, or when the fault state was cleared before, the due handling saves nothing.
        /// </summary>
        [TestMethod]
        [DataRow(2, false)]
        [DataRow(5, true)]
        public async Task HandleDueAsyncNothingDueSavesNothingTest(int secondsAfterReceived, bool isClearedBefore)
        {
            SetupDelayedFaultConfig(raiseDelayMilliseconds: 3000);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), DelayedReceivedOn);

            if (isClearedBefore)
            {
                await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: false), DelayedReceivedOn.AddSeconds(1));
            }

            _timeProvider.SetUtcNow(DelayedReceivedOn.AddSeconds(secondsAfterReceived));

            int changedCount = await handler.HandleDueAsync();

            Assert.AreEqual(0, changedCount);
            Assert.IsEmpty(_addedAlarms);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// When saving the delayed alarm fails, the publisher's values and delays are reset so that its next data
        /// is evaluated completely again and its delay starts again.
        /// </summary>
        [TestMethod]
        public async Task HandleDueAsyncSaveFailsResetsPublisherTest()
        {
            SetupDelayedFaultConfig(raiseDelayMilliseconds: 3000);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), DelayedReceivedOn);
            _alarmRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            _timeProvider.SetUtcNow(DelayedReceivedOn.AddSeconds(3));
            int changedCount = await handler.HandleDueAsync();
            _alarmRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), DelayedReceivedOn.AddSeconds(10));

            Assert.AreEqual(0, changedCount);
            PpcAlarmPendingTransition? pending = _pendingTransitions.Get(Pcs7Key, PcsAlarmConfigSampleData.CreatePcsFaultConfig().Id, "FaultStates.OverTemperature");
            Assert.IsNotNull(pending);
            Assert.AreEqual(DelayedReceivedOn.AddSeconds(13), pending.DueOn);
        }


        /// <summary>
        /// An active alarm is acknowledged on its cached instance (not the loaded one), so that the summary sent with its
        /// acknowledge action already counts it as acknowledged.
        /// </summary>
        [TestMethod]
        public async Task AcknowledgeAsyncActiveAlarmAcknowledgesCachedInstanceTest()
        {
            PpcAlarmConfig faultConfig = SetupAcknowledgeConfig();
            PpcAlarm cachedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            _savedActiveAlarms.Add(cachedAlarm);
            PpcAlarm storedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            SetupStoredAlarms(storedAlarm);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            int acknowledgedCount = await handler.AcknowledgeAsync(new List<int> { cachedAlarm.Id }, "Operator");

            Assert.AreEqual(1, acknowledgedCount);
            Assert.AreEqual(AlarmSequenceState.Acknowledged, cachedAlarm.AlarmSequenceState);
            Assert.AreEqual("Operator", cachedAlarm.AcknowledgedBy);
            Assert.AreEqual(NormalizedOn, cachedAlarm.AcknowledgedOn);
            Assert.AreEqual(AlarmSequenceState.Alarm, storedAlarm.AlarmSequenceState);
            Assert.AreSame(cachedAlarm, _updatedAlarms.Single());
            AlarmActionOutboxMessage message = _addedMessages.Single();
            Assert.AreEqual("AcknowledgeAlarm", message.ActionName);
            AlarmActionDto action = JsonSerializer.Deserialize<AlarmActionDto>(message.Payload)!;
            Assert.AreEqual(0, action.AlarmSummary.ErrorCount);
            Assert.AreEqual(1, action.AlarmSummary.AcknowledgedErrorCount);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _outboxSignal.Verify(signal => signal.Notify(), Times.Once);
        }


        /// <summary>
        /// An alarm that is already normal is acknowledged on the loaded instance and stays normal.
        /// </summary>
        [TestMethod]
        public async Task AcknowledgeAsyncNormalAlarmAcknowledgesLoadedInstanceTest()
        {
            PpcAlarmConfig faultConfig = SetupAcknowledgeConfig();
            PpcAlarm normalAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            normalAlarm.Normalize("System", PcsAlarmConfigSampleData.ChangedOn.AddMinutes(1));
            SetupStoredAlarms(normalAlarm);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            int acknowledgedCount = await handler.AcknowledgeAsync(new List<int> { normalAlarm.Id }, "Operator");

            Assert.AreEqual(1, acknowledgedCount);
            Assert.AreEqual(AlarmSequenceState.Normal, normalAlarm.AlarmSequenceState);
            Assert.AreEqual("Operator", normalAlarm.AcknowledgedBy);
            Assert.AreSame(normalAlarm, _updatedAlarms.Single());
            Assert.AreEqual("AcknowledgeAlarm", _addedMessages.Single().ActionName);
        }


        /// <summary>
        /// Alarms that cannot be acknowledged (not acknowledgeable, already acknowledged) and unknown ids are skipped,
        /// nothing is saved when no alarm was acknowledged.
        /// </summary>
        [TestMethod]
        public async Task AcknowledgeAsyncNothingToAcknowledgeSavesNothingTest()
        {
            PpcAlarmConfig faultConfig = SetupAcknowledgeConfig();
            PpcAlarmConfig notAcknowledgeableConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            notAcknowledgeableConfig.Id = 2;
            notAcknowledgeableConfig.IsAcknowledgeable = false;
            PpcAlarm notAcknowledgeableAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(notAcknowledgeableConfig, 7, "FaultStates.GridLoss");
            notAcknowledgeableAlarm.Normalize("System", PcsAlarmConfigSampleData.ChangedOn.AddMinutes(1));
            PpcAlarm acknowledgedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            acknowledgedAlarm.Id = 101;
            acknowledgedAlarm.Acknowledge("Homer", PcsAlarmConfigSampleData.ChangedOn.AddMinutes(1));
            acknowledgedAlarm.Normalize("System", PcsAlarmConfigSampleData.ChangedOn.AddMinutes(2));
            SetupStoredAlarms(notAcknowledgeableAlarm, acknowledgedAlarm);
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());

            int acknowledgedCount = await handler.AcknowledgeAsync(new List<int> { 100, 101, 999 }, "Operator");

            Assert.AreEqual(0, acknowledgedCount);
            Assert.AreEqual("Homer", acknowledgedAlarm.AcknowledgedBy);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _outboxSignal.Verify(signal => signal.Notify(), Times.Never);
        }


        /// <summary>
        /// When saving fails the publisher of the acknowledged active alarm is loaded again, the unsaved acknowledgement is lost.
        /// </summary>
        [TestMethod]
        public async Task AcknowledgeAsyncSaveFailsReloadsPublisherTest()
        {
            PpcAlarmConfig faultConfig = SetupAcknowledgeConfig();
            PpcAlarm cachedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            _savedActiveAlarms.Add(cachedAlarm);
            SetupStoredAlarms(PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature"));
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            _alarmRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            await Assert.ThrowsExactlyAsync<TimeoutException>(() => handler.AcknowledgeAsync(new List<int> { cachedAlarm.Id }, "Operator"));
            PpcAlarm savedAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            _savedActiveAlarms[0] = savedAlarm;

            List<PpcAlarm> activeAlarms = await _activeCache.GetActiveAsync(Pcs7Key, _alarmRepository.Object);

            Assert.AreSame(savedAlarm, activeAlarms.Single());
            Assert.AreEqual(AlarmSequenceState.Alarm, activeAlarms.Single().AlarmSequenceState);
        }


        /// <summary>
        /// Sets up the configuration cache with the PCS fault configuration with an additional acknowledge action.
        /// </summary>
        private PpcAlarmConfig SetupAcknowledgeConfig()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            faultConfig.AlarmActions.Add(new AlarmActionConfig
            {
                Id = 30,
                ChangedBy = "Homer",
                ChangedDate = PcsAlarmConfigSampleData.ChangedOn,
                Name = "AcknowledgeAlarm",
                Trigger = AlarmActionTrigger.Acknowledge
            });
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm))
                .Returns(new List<PpcAlarmConfig> { faultConfig });

            return faultConfig;
        }


        /// <summary>
        /// Sets up the repository to return the stored alarms by their ids.
        /// </summary>
        private void SetupStoredAlarms(params PpcAlarm[] storedAlarms)
        {
            _alarmRepository.Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyCollection<int> ids, CancellationToken _) => storedAlarms.Where(alarm => ids.Contains(alarm.Id)).ToList());
        }


        /// <summary>
        /// An evaluation waits while another writer (e.g. the web API) holds the write lock.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncWaitsForWriteLockTest()
        {
            PpcAlarmHandler handler = await CreateHandlerAsync(new PpcAlarmValueStore());
            IDisposable otherWriter = await _writeLock.AcquireAsync();
            Task<PpcAlarmEvaluation> evaluation = handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn);
            bool isEvaluatedWhileLocked = evaluation.IsCompleted;

            otherWriter.Dispose();

            Assert.IsFalse(isEvaluatedWhileLocked);
            Assert.HasCount(1, (await evaluation.WaitAsync(TimeSpan.FromSeconds(10))).Raised);
        }


        /// <summary>
        /// Sets up the configuration cache with the PCS fault configuration with a raise delay.
        /// </summary>
        private PpcAlarmConfig SetupDelayedFaultConfig(long raiseDelayMilliseconds)
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            faultConfig.TimeBeforeRaisingAlarmMilliseconds = raiseDelayMilliseconds;
            _configCache.Setup(cache => cache.GetEnabled(PpcAlarmType.PcsAlarm))
                .Returns(new List<PpcAlarmConfig> { faultConfig });

            return faultConfig;
        }


        /// <summary>
        /// Creates the handler with the real evaluator, the real active alarm cache loaded from the mocked repository
        /// and the mocked repositories.
        /// </summary>
        private async Task<PpcAlarmHandler> CreateHandlerAsync(IPpcAlarmValueStore valueStore)
        {
            await _activeCache.LoadAsync(_alarmRepository.Object);

            return new PpcAlarmHandler(NullLogger<PpcAlarmHandler>.Instance, valueStore, new PpcAlarmEvaluator(),
                _configCache.Object, _activeCache, _alarmRepository.Object, _outboxRepository.Object, _outboxSignal.Object, _writeLock,
                _timeProvider, _pendingTransitions, _deviceClient.Object);
        }


        /// <summary>
        /// Creates a PCS detail of PCS 7 with an over temperature fault state and a fan failure warning.
        /// </summary>
        private PcsDetailDto CreatePcsDetail(bool overTemperature)
        {
            return new PcsDetailDto
            {
                DeviceId = 7,
                IsInMaintenanceMode = false,
                IsACBreakerClosed = true,
                FaultStates = new Dictionary<string, bool> { { "OverTemperature", overTemperature }, { "GridLoss", false } },
                WarningStates = new Dictionary<string, bool> { { "FanFailure", true } },
                VendorEvents = new Dictionary<string, bool> { { "FirmwareUpdate", false } }
            };
        }
    }
}
