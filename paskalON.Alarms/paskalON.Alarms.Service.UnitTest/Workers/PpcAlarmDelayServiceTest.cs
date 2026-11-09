// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Service.Workers;

namespace paskalON.Alarms.Service.UnitTest.Workers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmDelayService"/> with the real pending transitions and a mocked handler in a real service provider.
    /// </summary>
    [TestClass]
    public class PpcAlarmDelayServiceTest
    {
        /// <summary>
        /// Interval between two checks.
        /// </summary>
        private const int CheckIntervalMilliseconds = 500;


        /// <summary>
        /// Start of the fake time, the communication error changed at this time.
        /// </summary>
        private static readonly DateTimeOffset ChangedOn = new DateTimeOffset(2026, 11, 9, 10, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// Maximum real time to wait for a completion in the tests.
        /// </summary>
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        /// <summary>
        /// Key of PCS 7.
        /// </summary>
        private static readonly PpcAlarmKey Pcs7Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 7 };


        /// <summary>
        /// Mocked alarm handler.
        /// </summary>
        private readonly Mock<IPpcAlarmHandler> _handler = new Mock<IPpcAlarmHandler>();


        /// <summary>
        /// Delayed raises and normalizations.
        /// </summary>
        private readonly PpcAlarmPendingTransitions _pendingTransitions = new PpcAlarmPendingTransitions();


        /// <summary>
        /// Fake time of the check interval and the due times.
        /// </summary>
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(ChangedOn);


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
        /// <returns>Task.</returns>
        [TestCleanup]
        public async Task Cleanup()
        {
            await _serviceProvider.DisposeAsync();
        }


        /// <summary>
        /// Without a due transition no scope is created and the handler is not called.
        /// </summary>
        [TestMethod]
        [DataRow(false, 0)]
        [DataRow(true, 2999)]
        public async Task CheckDueAsyncNothingDueDoesNotCallHandlerTest(bool hasPending, int millisecondsAfterChange)
        {
            if (hasPending)
            {
                StartRaise(delayMilliseconds: 3000);
            }

            PpcAlarmDelayService service = CreateService();
            _timeProvider.Advance(TimeSpan.FromMilliseconds(millisecondsAfterChange));

            bool isHandled = await service.CheckDueAsync();

            Assert.IsFalse(isHandled);
            Assert.AreEqual(0, _createdHandlers);
            _handler.Verify(handler => handler.HandleDueAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// A due transition is handled by the handler of a new scope.
        /// </summary>
        [TestMethod]
        public async Task CheckDueAsyncDueTransitionCallsHandlerTest()
        {
            StartRaise(delayMilliseconds: 3000);
            _handler.Setup(handler => handler.HandleDueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            PpcAlarmDelayService service = CreateService();
            _timeProvider.Advance(TimeSpan.FromSeconds(3));

            bool isHandled = await service.CheckDueAsync();

            Assert.IsTrue(isHandled);
            Assert.AreEqual(1, _createdHandlers);
            _handler.Verify(handler => handler.HandleDueAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// A failed handling (e.g. database not available) is logged, the service keeps running.
        /// </summary>
        [TestMethod]
        public async Task CheckDueAsyncHandlerFailsReturnsFalseTest()
        {
            StartRaise(delayMilliseconds: 3000);
            _handler.Setup(handler => handler.HandleDueAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            PpcAlarmDelayService service = CreateService();
            _timeProvider.Advance(TimeSpan.FromSeconds(3));

            bool isHandled = await service.CheckDueAsync();

            Assert.IsFalse(isHandled);
        }


        /// <summary>
        /// The running service checks every interval and handles the transition once it is due.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncHandlesDueTransitionAfterIntervalTest()
        {
            TaskCompletionSource handled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            StartRaise(delayMilliseconds: CheckIntervalMilliseconds);
            _handler.Setup(handler => handler.HandleDueAsync(It.IsAny<CancellationToken>()))
                .Callback(() => handled.TrySetResult())
                .ReturnsAsync(1);
            PpcAlarmDelayService service = CreateService();
            service.Initialize(CheckIntervalMilliseconds);
            await service.StartAsync(CancellationToken.None);

            await AdvanceUntilAsync(handled.Task);
            await service.StopAsync(CancellationToken.None);

            _handler.Verify(handler => handler.HandleDueAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// The service cannot start without the check interval of the system configuration.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncNotInitializedThrowsTest()
        {
            PpcAlarmDelayService service = CreateService();

            await service.StartAsync(CancellationToken.None);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => service.ExecuteTask!);
        }


        /// <summary>
        /// Starts a delayed raise of the communication error of PCS 7 that changed at the start of the fake time.
        /// </summary>
        private void StartRaise(int delayMilliseconds)
        {
            _pendingTransitions.Start(new PpcAlarmPendingTransition
            {
                Key = Pcs7Key,
                PpcAlarmConfigId = 1,
                PropertyName = "CommunicationError",
                Transition = PpcAlarmTransition.Raise,
                Value = new PpcAlarmKeyValue { Name = "CommunicationError", Value = 1, ValueChanged = true, ValueChangedOn = ChangedOn },
                DueOn = ChangedOn.AddMilliseconds(delayMilliseconds)
            });
        }


        /// <summary>
        /// Advances the fake time by the check interval until the task completed, the check delay might not be registered yet.
        /// </summary>
        private async Task AdvanceUntilAsync(Task task)
        {
            using CancellationTokenSource timeout = new CancellationTokenSource(TestTimeout);

            while (task.IsCompleted is false)
            {
                timeout.Token.ThrowIfCancellationRequested();
                _timeProvider.Advance(TimeSpan.FromMilliseconds(CheckIntervalMilliseconds));
                await Task.WhenAny(task, Task.Delay(TimeSpan.FromMilliseconds(10), TimeProvider.System));
            }

            await task;
        }


        /// <summary>
        /// Creates the service.
        /// </summary>
        private PpcAlarmDelayService CreateService()
        {
            return new PpcAlarmDelayService(NullLogger<PpcAlarmDelayService>.Instance, _serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                _pendingTransitions, _timeProvider);
        }
    }
}
