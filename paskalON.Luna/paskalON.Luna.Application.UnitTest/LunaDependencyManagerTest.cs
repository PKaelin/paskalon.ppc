// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Luna.Application.Dependencies;
using paskalON.Luna.Dto.Systems;
using paskalON.Messaging.Health;

namespace paskalON.Luna.Application.UnitTest
{
    /// <summary>
    /// Tests <see cref="LunaDependencyManager"/> with a mocked dependency health monitor.
    /// </summary>
    [TestClass]
    public sealed class LunaDependencyManagerTest
    {
        /// <summary>
        /// Tests that every dependency is reported with the state of the monitor.
        /// </summary>
        [TestMethod]
        [DataRow(DependencyNames.PowerControl)]
        [DataRow(DependencyNames.OperatingMode)]
        [DataRow(DependencyNames.DeviceService)]
        [DataRow(DependencyNames.Redis)]
        public void GetDependencyStatusReportsOnlyLostDependencyAsUnavailableTest(string lostDependency)
        {
            Mock<IDependencyHealthMonitor> monitor = new Mock<IDependencyHealthMonitor>();
            monitor.Setup(m => m.GetState(It.IsAny<string>())).Returns(DependencyState.Available);
            monitor.Setup(m => m.GetState(lostDependency)).Returns(DependencyState.Lost);
            LunaDependencyManager manager = new LunaDependencyManager(monitor.Object);

            IReadOnlyList<DependencyStatusDto> statuses = manager.GetDependencyStatus();

            Assert.HasCount(4, statuses);

            foreach (DependencyStatusDto status in statuses)
            {
                Assert.AreEqual(status.Name != lostDependency, status.IsAvailable, status.Name);
            }
        }
    }
}
