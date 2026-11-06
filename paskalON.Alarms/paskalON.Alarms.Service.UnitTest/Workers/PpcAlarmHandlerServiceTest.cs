// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Subscribers;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Service.Workers;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Alarms.Service.UnitTest.Workers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmHandlerService"/> with the real queue and a mocked handler in a real service provider.
    /// </summary>
    [TestClass]
    public class PpcAlarmHandlerServiceTest
    {
        /// <summary>
        /// Time when the first data was received.
        /// </summary>
        private static readonly DateTimeOffset ReceivedOn = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero);


        /// <summary>
        /// Maximum real time to wait for a completion in the tests.
        /// </summary>
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        /// <summary>
        /// Mocked alarm handler.
        /// </summary>
        private readonly Mock<IPpcAlarmHandler> _handler = new Mock<IPpcAlarmHandler>();


        /// <summary>
        /// Queue of the received data.
        /// </summary>
        private readonly PpcAlarmMessageQueue _queue = new PpcAlarmMessageQueue(NullLogger<PpcAlarmMessageQueue>.Instance);


        /// <summary>
        /// Number of created handlers, one per scope.
        /// </summary>
        private int _createdHandlers;


        /// <summary>
        /// Service provider with the scoped handler.
        /// </summary>
        private ServiceProvider _serviceProvider = null!;


        /// <summary>
        /// Builds the service provider with the mocked handler registered scoped.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<IPpcAlarmHandler>(_ =>
            {
                Interlocked.Increment(ref _createdHandlers);

                return _handler.Object;
            });
            _serviceProvider = services.BuildServiceProvider();
        }


        /// <summary>
        /// Disposes the service provider.
        /// </summary>
        [TestCleanup]
        public async Task Cleanup()
        {
            await _serviceProvider.DisposeAsync();
        }


        /// <summary>
        /// The received data is passed with its publisher and receive time to the handler in a new scope.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncPassesDataToHandlerTest()
        {
            PpcAlarmMessage message = CreateMessage(7, 0);
            SetupHandler();
            PpcAlarmHandlerService service = CreateService();

            bool isHandled = await service.HandleAsync(message);

            Assert.IsTrue(isHandled);
            Assert.AreEqual(1, _createdHandlers);
            _handler.Verify(handler => handler.HandleAsync(message.Key, message.Data, message.ReceivedOn, It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// A failed evaluation (e.g. database not available) is logged and does not throw.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncHandlerThrowsReturnsFalseTest()
        {
            _handler.Setup(handler => handler.HandleAsync(It.IsAny<PpcAlarmKey>(), It.IsAny<object>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            PpcAlarmHandlerService service = CreateService();

            bool isHandled = await service.HandleAsync(CreateMessage(7, 0));

            Assert.IsFalse(isHandled);
        }


        /// <summary>
        /// The running service evaluates the queued data in receive order and continues after a failed evaluation.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncHandlesQueuedDataInOrderAfterFailureTest()
        {
            List<DateTimeOffset> handled = new List<DateTimeOffset>();
            TaskCompletionSource allHandled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _handler.Setup(handler => handler.HandleAsync(It.IsAny<PpcAlarmKey>(), It.IsAny<object>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .Returns<PpcAlarmKey, object, DateTimeOffset, CancellationToken>((_, _, receivedOn, _) =>
                {
                    handled.Add(receivedOn);

                    if (handled.Count == 3)
                    {
                        allHandled.TrySetResult();
                    }

                    // The first evaluation fails, e.g. database not available
                    return handled.Count == 1
                        ? Task.FromException<PpcAlarmEvaluation>(new TimeoutException("Database not available."))
                        : Task.FromResult(new PpcAlarmEvaluation());
                });
            _queue.Enqueue(CreateMessage(7, 0));
            _queue.Enqueue(CreateMessage(8, 1));
            _queue.Enqueue(CreateMessage(7, 2));
            PpcAlarmHandlerService service = CreateService();

            await service.StartAsync(CancellationToken.None);

            await allHandled.Task.WaitAsync(TestTimeout);
            await service.StopAsync(CancellationToken.None);
            CollectionAssert.AreEqual(new List<DateTimeOffset> { ReceivedOn, ReceivedOn.AddSeconds(1), ReceivedOn.AddSeconds(2) }, handled);
        }


        /// <summary>
        /// Sets up the handler to evaluate without changes.
        /// </summary>
        private void SetupHandler()
        {
            _handler.Setup(handler => handler.HandleAsync(It.IsAny<PpcAlarmKey>(), It.IsAny<object>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PpcAlarmEvaluation());
        }


        /// <summary>
        /// Creates the service with the queue and the service provider.
        /// </summary>
        private PpcAlarmHandlerService CreateService()
        {
            return new PpcAlarmHandlerService(NullLogger<PpcAlarmHandlerService>.Instance, _queue,
                _serviceProvider.GetRequiredService<IServiceScopeFactory>());
        }


        /// <summary>
        /// Creates the received PCS detail of a PCS with an over temperature fault state.
        /// </summary>
        private static PpcAlarmMessage CreateMessage(int deviceId, int receivedAfterSeconds)
        {
            PcsDetailDto detail = new PcsDetailDto
            {
                DeviceId = deviceId,
                IsACBreakerClosed = true,
                FaultStates = new Dictionary<string, bool> { { "OverTemperature", true }, { "GridLoss", false } }
            };

            return new PpcAlarmMessage
            {
                Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = deviceId },
                Data = detail,
                ReceivedOn = ReceivedOn.AddSeconds(receivedAfterSeconds)
            };
        }
    }
}
