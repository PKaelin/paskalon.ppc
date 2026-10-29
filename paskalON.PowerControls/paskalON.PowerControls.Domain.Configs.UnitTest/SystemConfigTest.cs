// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.Configs;

namespace paskalON.PowerControls.Domain.Configs.UnitTest
{
    [TestClass]
    public class SystemConfigTest
    {
        [TestMethod]
        [DataRow(100)]
        [DataRow(1000)]
        public void SystemConfigDependencyHeartbeatIntervalAcceptedTest(int intervalMilliseconds)
        {
            SystemConfig config = CreateConfig();

            config.DependencyHeartbeatIntervalMilliseconds = intervalMilliseconds;
            config.PublisherTopicHeartbeat = "ppc:pc:heartbeat";

            Assert.AreEqual(intervalMilliseconds, config.DependencyHeartbeatIntervalMilliseconds);
            Assert.AreEqual("ppc:pc:heartbeat", config.PublisherTopicHeartbeat);
        }


        [TestMethod]
        [DataRow(99)]
        [DataRow(0)]
        [DataRow(-200)]
        public void SystemConfigDependencyHeartbeatIntervalBelowMinimumThrowsTest(int intervalMilliseconds)
        {
            SystemConfig config = CreateConfig();

            Action act = () => config.DependencyHeartbeatIntervalMilliseconds = intervalMilliseconds;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        private SystemConfig CreateConfig()
        {
            return new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess
            };
        }
    }
}
