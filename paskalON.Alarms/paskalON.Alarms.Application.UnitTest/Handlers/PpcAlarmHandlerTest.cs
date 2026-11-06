// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Stores;
using paskalON.Alarms.Application.UnitTest.SampleData;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Devices.Dto.PowerConversionSystems;
using System.Text.Json;

namespace paskalON.Alarms.Application.UnitTest.Handlers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmHandler"/> with the real store, reader and evaluator and mocked repositories.
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
        /// Mocked configuration repository.
        /// </summary>
        private readonly Mock<IPpcAlarmConfigRepository> _configRepository = new Mock<IPpcAlarmConfigRepository>();


        /// <summary>
        /// Mocked alarm repository.
        /// </summary>
        private readonly Mock<IPpcAlarmRepository> _alarmRepository = new Mock<IPpcAlarmRepository>();


        /// <summary>
        /// Mocked outbox repository.
        /// </summary>
        private readonly Mock<IAlarmActionOutboxRepository> _outboxRepository = new Mock<IAlarmActionOutboxRepository>();


        /// <summary>
        /// Alarms added to the alarm repository.
        /// </summary>
        private readonly List<PpcAlarm> _addedAlarms = new List<PpcAlarm>();


        /// <summary>
        /// Messages added to the outbox repository.
        /// </summary>
        private readonly List<AlarmActionOutboxMessage> _addedMessages = new List<AlarmActionOutboxMessage>();


        /// <summary>
        /// Sets up the repositories with the PCS fault configuration and without active alarms.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _configRepository.Setup(repository => repository.GetEnabledAsync(PpcAlarmType.PcsAlarm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<PpcAlarmConfig> { PcsAlarmConfigSampleData.CreatePcsFaultConfig() });
            _alarmRepository.Setup(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PpcAlarm>());
            _alarmRepository.Setup(repository => repository.Add(It.IsAny<IEnumerable<PpcAlarm>>()))
                .Callback<IEnumerable<PpcAlarm>>(alarms => _addedAlarms.AddRange(alarms));
            _alarmRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _outboxRepository.Setup(repository => repository.Add(It.IsAny<IEnumerable<AlarmActionOutboxMessage>>()))
                .Callback<IEnumerable<AlarmActionOutboxMessage>>(messages => _addedMessages.AddRange(messages));
        }


        /// <summary>
        /// A fault state that turns true raises an alarm and adds its raise action to the outbox in one save.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncFaultStateRaisesAlarmAndAddsRaiseActionTest()
        {
            PpcAlarmHandler handler = CreateHandler(new PpcAlarmValueStore());

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
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// A fault state that turns false normalizes the active alarm and adds its normalize action to the outbox.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncFaultStateClearedNormalizesAlarmAndAddsNormalizeActionTest()
        {
            PpcAlarmConfig faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            PpcAlarm activeAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(faultConfig, 7, "FaultStates.OverTemperature");
            _configRepository.Setup(repository => repository.GetEnabledAsync(PpcAlarmType.PcsAlarm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PpcAlarmConfig> { faultConfig });
            _alarmRepository.Setup(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PpcAlarm> { activeAlarm });
            PpcAlarmHandler handler = CreateHandler(new PpcAlarmValueStore());

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: false), ReceivedOn);

            Assert.HasCount(1, evaluation.Normalized);
            Assert.AreEqual(AlarmSequenceState.Normal, activeAlarm.AlarmSequenceState);
            Assert.IsEmpty(_addedAlarms);
            Assert.HasCount(1, _addedMessages);
            Assert.AreEqual("ResetPower", _addedMessages[0].ActionName);
            Assert.AreSame(activeAlarm, _addedMessages[0].PpcAlarm);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// Unchanged data is not evaluated again and does not access the database.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncUnchangedDataDoesNotEvaluateTest()
        {
            PpcAlarmHandler handler = CreateHandler(new PpcAlarmValueStore());
            await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn);

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn.AddSeconds(1));

            Assert.IsFalse(evaluation.HasChanges);
            _configRepository.Verify(repository => repository.GetEnabledAsync(It.IsAny<PpcAlarmType>(), It.IsAny<CancellationToken>()), Times.Once);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// Changed values without a matching configuration do not save anything.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncChangedValuesWithoutAlarmDoesNotSaveTest()
        {
            PpcAlarmHandler handler = CreateHandler(new PpcAlarmValueStore());

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
            PpcAlarmHandler handler = CreateHandler(new PpcAlarmValueStore());
            await Assert.ThrowsExactlyAsync<TimeoutException>(() => handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn));
            _addedAlarms.Clear();
            _addedMessages.Clear();

            PpcAlarmEvaluation evaluation = await handler.HandleAsync(Pcs7Key, CreatePcsDetail(overTemperature: true), ReceivedOn.AddSeconds(1));

            Assert.HasCount(1, evaluation.Raised);
            Assert.HasCount(1, _addedAlarms);
            Assert.HasCount(1, _addedMessages);
            _alarmRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        /// <summary>
        /// Creates the handler with the real evaluator and the mocked repositories.
        /// </summary>
        private PpcAlarmHandler CreateHandler(IPpcAlarmValueStore valueStore)
        {
            return new PpcAlarmHandler(NullLogger<PpcAlarmHandler>.Instance, valueStore, new PpcAlarmEvaluator(),
                _configRepository.Object, _alarmRepository.Object, _outboxRepository.Object);
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
