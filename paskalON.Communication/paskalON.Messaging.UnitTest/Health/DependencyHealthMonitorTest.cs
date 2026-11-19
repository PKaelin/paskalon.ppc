// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;
using paskalON.Messaging.Health;

namespace paskalON.Messaging.UnitTest.Health
{
    [TestClass]
    public sealed class DependencyHealthMonitorTest
    {
        private const string Redis = "Redis";
        private const string PowerControl = "PowerControl";
        private const string DeviceService = "DeviceService";
        private static readonly TimeSpan LossTimeout = TimeSpan.FromMilliseconds(1500);


        [TestMethod]
        public void ConstructorNullLoggerThrowsTest()
        {
            Func<DependencyHealthMonitor> act = () => new DependencyHealthMonitor(null!, TimeProvider.System);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void ConstructorNullTimeProviderThrowsTest()
        {
            Func<DependencyHealthMonitor> act = () => new DependencyHealthMonitor(NullLogger<DependencyHealthMonitor>.Instance, null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1500)]
        public void RegisterSignalInvalidLossTimeoutThrowsTest(int lossTimeoutMilliseconds)
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());

            Action act = () => monitor.RegisterSignal(PowerControl, TimeSpan.FromMilliseconds(lossTimeoutMilliseconds));

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        [TestMethod]
        public void RegisterConditionNullConditionThrowsTest()
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());

            Action act = () => monitor.RegisterCondition(Redis, null!, LossTimeout);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void RegisterDuplicateNameThrowsTest()
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());
            monitor.RegisterSignal(PowerControl, LossTimeout);

            Action act = () => monitor.RegisterCondition(PowerControl, () => true, LossTimeout);

            Assert.ThrowsExactly<ArgumentException>(act);
        }


        [TestMethod]
        public void GetStateUnregisteredThrowsTest()
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());
            monitor.RegisterSignal(PowerControl, LossTimeout);

            Action act = () => monitor.GetState(DeviceService);

            Assert.ThrowsExactly<ArgumentException>(act);
        }


        [TestMethod]
        public void SignalUnregisteredThrowsTest()
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());
            monitor.RegisterSignal(PowerControl, LossTimeout);

            Action act = () => monitor.Signal(DeviceService);

            Assert.ThrowsExactly<ArgumentException>(act);
        }


        [TestMethod]
        public void GetStateNeverSignaledIsLostTest()
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());
            monitor.RegisterSignal(PowerControl, LossTimeout);

            DependencyState state = monitor.GetState(PowerControl);

            Assert.AreEqual(DependencyState.Lost, state);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(500)]
        [DataRow(1499)]
        public void GetStateSignaledWithinLossTimeoutIsAvailableTest(int elapsedMilliseconds)
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            DependencyHealthMonitor monitor = CreateMonitor(timeProvider);
            monitor.RegisterSignal(PowerControl, LossTimeout);
            monitor.Signal(PowerControl);
            timeProvider.Advance(TimeSpan.FromMilliseconds(elapsedMilliseconds));

            DependencyState state = monitor.GetState(PowerControl);

            Assert.AreEqual(DependencyState.Available, state);
        }


        [TestMethod]
        [DataRow(1500)]
        [DataRow(60000)]
        public void GetStateSignaledLossTimeoutElapsedIsLostTest(int elapsedMilliseconds)
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            DependencyHealthMonitor monitor = CreateMonitor(timeProvider);
            monitor.RegisterSignal(PowerControl, LossTimeout);
            monitor.Signal(PowerControl);
            timeProvider.Advance(TimeSpan.FromMilliseconds(elapsedMilliseconds));

            DependencyState state = monitor.GetState(PowerControl);

            Assert.AreEqual(DependencyState.Lost, state);
        }


        [TestMethod]
        public void GetStateSignalAfterLossIsAvailableAgainTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            DependencyHealthMonitor monitor = CreateMonitor(timeProvider);
            monitor.RegisterSignal(PowerControl, LossTimeout);
            monitor.Signal(PowerControl);
            timeProvider.Advance(TimeSpan.FromSeconds(10));
            DependencyState stateBefore = monitor.GetState(PowerControl);

            monitor.Signal(PowerControl);
            DependencyState stateAfter = monitor.GetState(PowerControl);

            Assert.AreEqual(DependencyState.Lost, stateBefore);
            Assert.AreEqual(DependencyState.Available, stateAfter);
        }


        [TestMethod]
        public void GetStateConditionNeverTrueIsLostTest()
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());
            monitor.RegisterCondition(Redis, () => false, LossTimeout);

            DependencyState state = monitor.GetState(Redis);

            Assert.AreEqual(DependencyState.Lost, state);
        }


        [TestMethod]
        [DataRow(0, DependencyState.Available)]
        [DataRow(1499, DependencyState.Available)]
        [DataRow(1500, DependencyState.Lost)]
        public void GetStateConditionFalseIsLostAfterLossTimeoutTest(int elapsedMilliseconds, DependencyState expectedState)
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            bool isConnected = true;
            DependencyHealthMonitor monitor = CreateMonitor(timeProvider);
            monitor.RegisterCondition(Redis, () => isConnected, LossTimeout);
            monitor.GetState(Redis);
            isConnected = false;
            timeProvider.Advance(TimeSpan.FromMilliseconds(elapsedMilliseconds));

            DependencyState state = monitor.GetState(Redis);

            Assert.AreEqual(expectedState, state);
        }


        [TestMethod]
        public void GetStateConditionTrueAgainIsAvailableTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            bool isConnected = false;
            DependencyHealthMonitor monitor = CreateMonitor(timeProvider);
            monitor.RegisterCondition(Redis, () => isConnected, LossTimeout);
            timeProvider.Advance(TimeSpan.FromSeconds(10));
            DependencyState stateBefore = monitor.GetState(Redis);

            isConnected = true;
            DependencyState stateAfter = monitor.GetState(Redis);

            Assert.AreEqual(DependencyState.Lost, stateBefore);
            Assert.AreEqual(DependencyState.Available, stateAfter);
        }


        [TestMethod]
        public void GetLostDependenciesReturnsOnlyLostTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            DependencyHealthMonitor monitor = CreateMonitor(timeProvider);
            monitor.RegisterCondition(Redis, () => true, LossTimeout);
            monitor.RegisterSignal(PowerControl, LossTimeout);
            monitor.RegisterSignal(DeviceService, TimeSpan.FromMilliseconds(3000));
            monitor.Signal(PowerControl);
            monitor.Signal(DeviceService);
            timeProvider.Advance(TimeSpan.FromMilliseconds(2000));

            IReadOnlyList<string> lostDependencies = monitor.GetLostDependencies();

            Assert.HasCount(1, lostDependencies);
            Assert.AreEqual(PowerControl, lostDependencies[0]);
        }


        [TestMethod]
        public void GetLostDependenciesAllAvailableIsEmptyTest()
        {
            DependencyHealthMonitor monitor = CreateMonitor(new FakeTimeProvider());
            monitor.RegisterCondition(Redis, () => true, LossTimeout);
            monitor.RegisterSignal(PowerControl, LossTimeout);
            monitor.Signal(PowerControl);

            IReadOnlyList<string> lostDependencies = monitor.GetLostDependencies();

            Assert.IsEmpty(lostDependencies);
        }


        [TestMethod]
        public void GetStateLogsEveryStateChangeOnceTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger<DependencyHealthMonitor> logger = new FakeLogger<DependencyHealthMonitor>();
            DependencyHealthMonitor monitor = new DependencyHealthMonitor(logger, timeProvider);
            monitor.RegisterSignal(PowerControl, LossTimeout);
            monitor.Signal(PowerControl);

            monitor.GetState(PowerControl);
            monitor.GetState(PowerControl);
            timeProvider.Advance(LossTimeout);
            monitor.GetState(PowerControl);
            monitor.GetState(PowerControl);

            IReadOnlyList<FakeLogRecord> records = logger.Collector.GetSnapshot();
            Assert.HasCount(2, records);
            Assert.AreEqual(LogLevel.Information, records[0].Level);
            Assert.AreEqual(LogLevel.Warning, records[1].Level);
        }


        private DependencyHealthMonitor CreateMonitor(TimeProvider timeProvider)
        {
            return new DependencyHealthMonitor(NullLogger<DependencyHealthMonitor>.Instance, timeProvider);
        }
    }
}
