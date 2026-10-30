// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Messaging.Health;
using paskalON.OperatingModes.Application.Dependencies;

namespace paskalON.OperatingModes.Application.UnitTest.Dependencies
{
    /// <summary>
    /// Tests the hold reason of <see cref="DependencyGate"/>.
    /// </summary>
    [TestClass]
    public class DependencyGateTest
    {
        /// <summary>
        /// The resume settle time of the tests.
        /// </summary>
        private static readonly TimeSpan ResumeSettleTime = TimeSpan.FromMilliseconds(2000);


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void DependencyGateConstructorInvalidArgumentsTest()
        {
            IDependencyHealthMonitor monitor = new Mock<IDependencyHealthMonitor>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new DependencyGate(null!, ResumeSettleTime, TimeProvider.System));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DependencyGate(monitor, TimeSpan.FromMilliseconds(-1), TimeProvider.System));
            Assert.ThrowsExactly<ArgumentNullException>(() => new DependencyGate(monitor, ResumeSettleTime, null!));
        }


        /// <summary>
        /// Confirms lost dependencies are the hold reason.
        /// </summary>
        [TestMethod]
        public void DependencyGateGetHoldReasonLostDependenciesTest()
        {
            Mock<IDependencyHealthMonitor> monitor = CreateMonitor(DependencyNames.Redis, DependencyNames.PowerControl);
            DependencyGate gate = new DependencyGate(monitor.Object, ResumeSettleTime, new FakeTimeProvider());

            string? holdReason = gate.GetHoldReason();

            Assert.AreEqual("Lost dependencies: Redis, PowerControl.", holdReason);
        }


        /// <summary>
        /// Confirms the control holds until all dependencies are available for the settle time.
        /// </summary>
        /// <param name="elapsedMilliseconds">The time since all dependencies are available.</param>
        /// <param name="isHolding">Whether the control is expected to hold.</param>
        [TestMethod]
        [DataRow(0, true)]
        [DataRow(1999, true)]
        [DataRow(2000, false)]
        [DataRow(60000, false)]
        public void DependencyGateGetHoldReasonSettlesAfterLossTest(int elapsedMilliseconds, bool isHolding)
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            List<string> lostDependencies = new List<string> { DependencyNames.DeviceService };
            Mock<IDependencyHealthMonitor> monitor = new Mock<IDependencyHealthMonitor>();
            monitor.Setup(x => x.GetLostDependencies()).Returns(() => lostDependencies.ToArray());
            DependencyGate gate = new DependencyGate(monitor.Object, ResumeSettleTime, timeProvider);
            gate.GetHoldReason();
            lostDependencies.Clear();
            gate.GetHoldReason();
            timeProvider.Advance(TimeSpan.FromMilliseconds(elapsedMilliseconds));

            string? holdReason = gate.GetHoldReason();

            Assert.AreEqual(isHolding, holdReason is not null);
        }


        /// <summary>
        /// Confirms a loss during the settle time restarts the settle time.
        /// </summary>
        [TestMethod]
        public void DependencyGateGetHoldReasonLossWhileSettlingRestartsSettleTimeTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            List<string> lostDependencies = new List<string>();
            Mock<IDependencyHealthMonitor> monitor = new Mock<IDependencyHealthMonitor>();
            monitor.Setup(x => x.GetLostDependencies()).Returns(() => lostDependencies.ToArray());
            DependencyGate gate = new DependencyGate(monitor.Object, ResumeSettleTime, timeProvider);
            gate.GetHoldReason();
            timeProvider.Advance(TimeSpan.FromMilliseconds(1500));
            lostDependencies.Add(DependencyNames.PowerControl);
            string? lostReason = gate.GetHoldReason();
            lostDependencies.Clear();
            gate.GetHoldReason();
            timeProvider.Advance(TimeSpan.FromMilliseconds(1500));

            string? holdReason = gate.GetHoldReason();

            Assert.AreEqual("Lost dependencies: PowerControl.", lostReason);
            Assert.AreEqual("Dependencies are available, settling for 2000 ms.", holdReason);
        }


        /// <summary>
        /// Confirms the control does not hold without settle time when all dependencies are available.
        /// </summary>
        [TestMethod]
        public void DependencyGateGetHoldReasonWithoutSettleTimeTest()
        {
            Mock<IDependencyHealthMonitor> monitor = CreateMonitor();
            DependencyGate gate = new DependencyGate(monitor.Object, TimeSpan.Zero, new FakeTimeProvider());

            string? holdReason = gate.GetHoldReason();

            Assert.IsNull(holdReason);
        }


        /// <summary>
        /// Creates a monitor reporting the given lost dependencies.
        /// </summary>
        /// <param name="lostDependencies">The names of the lost dependencies.</param>
        /// <returns>The mocked monitor.</returns>
        private Mock<IDependencyHealthMonitor> CreateMonitor(params string[] lostDependencies)
        {
            Mock<IDependencyHealthMonitor> monitor = new Mock<IDependencyHealthMonitor>();
            monitor.Setup(x => x.GetLostDependencies()).Returns(lostDependencies);

            return monitor;
        }
    }
}
