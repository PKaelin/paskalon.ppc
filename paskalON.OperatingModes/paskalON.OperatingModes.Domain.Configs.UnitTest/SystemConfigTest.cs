// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.Configs;

namespace paskalON.OperatingModes.Domain.Configs.UnitTest
{
    [TestClass]
    public class SystemConfigTest
    {
        [TestMethod]
        public void SystemConfigDependencyValidValuesAcceptedTest()
        {
            SystemConfig config = CreateConfig();

            config.DependencyHeartbeatIntervalMilliseconds = 100;
            config.RedisLossTimeoutMilliseconds = 1000;
            config.PowerControlLossTimeoutMilliseconds = 1000;
            config.DeviceServiceLossTimeoutMilliseconds = 1000;
            config.ResumeSettleMilliseconds = 0;
            config.SetpointRepublishIntervalMilliseconds = 200;
            config.PublisherTopicHeartbeat = "ppc:om:heartbeat";
            config.SubscriberTopicPowerControlHeartbeat = "ppc:pc:heartbeat";

            Assert.AreEqual(100, config.DependencyHeartbeatIntervalMilliseconds);
            Assert.AreEqual(1000, config.RedisLossTimeoutMilliseconds);
            Assert.AreEqual(1000, config.PowerControlLossTimeoutMilliseconds);
            Assert.AreEqual(1000, config.DeviceServiceLossTimeoutMilliseconds);
            Assert.AreEqual(0, config.ResumeSettleMilliseconds);
            Assert.AreEqual(200, config.SetpointRepublishIntervalMilliseconds);
            Assert.AreEqual("ppc:om:heartbeat", config.PublisherTopicHeartbeat);
            Assert.AreEqual("ppc:pc:heartbeat", config.SubscriberTopicPowerControlHeartbeat);
        }


        [TestMethod]
        [DataRow(nameof(SystemConfig.DependencyHeartbeatIntervalMilliseconds), 99)]
        [DataRow(nameof(SystemConfig.DependencyHeartbeatIntervalMilliseconds), 0)]
        [DataRow(nameof(SystemConfig.RedisLossTimeoutMilliseconds), 999)]
        [DataRow(nameof(SystemConfig.RedisLossTimeoutMilliseconds), 500)]
        [DataRow(nameof(SystemConfig.PowerControlLossTimeoutMilliseconds), 999)]
        [DataRow(nameof(SystemConfig.PowerControlLossTimeoutMilliseconds), 500)]
        [DataRow(nameof(SystemConfig.DeviceServiceLossTimeoutMilliseconds), 999)]
        [DataRow(nameof(SystemConfig.DeviceServiceLossTimeoutMilliseconds), 500)]
        [DataRow(nameof(SystemConfig.ResumeSettleMilliseconds), -1)]
        [DataRow(nameof(SystemConfig.SetpointRepublishIntervalMilliseconds), 199)]
        [DataRow(nameof(SystemConfig.SetpointRepublishIntervalMilliseconds), 0)]
        public void SystemConfigDependencyValueBelowMinimumThrowsTest(string propertyName, int value)
        {
            SystemConfig config = CreateConfig();

            Action act = () => SetProperty(config, propertyName, value);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        private SystemConfig CreateConfig()
        {
            return new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess,
                ReferenceFrequency = 60.0
            };
        }


        private void SetProperty(SystemConfig config, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(SystemConfig.DependencyHeartbeatIntervalMilliseconds):
                    config.DependencyHeartbeatIntervalMilliseconds = value;
                    break;
                case nameof(SystemConfig.RedisLossTimeoutMilliseconds):
                    config.RedisLossTimeoutMilliseconds = value;
                    break;
                case nameof(SystemConfig.PowerControlLossTimeoutMilliseconds):
                    config.PowerControlLossTimeoutMilliseconds = value;
                    break;
                case nameof(SystemConfig.DeviceServiceLossTimeoutMilliseconds):
                    config.DeviceServiceLossTimeoutMilliseconds = value;
                    break;
                case nameof(SystemConfig.ResumeSettleMilliseconds):
                    config.ResumeSettleMilliseconds = value;
                    break;
                case nameof(SystemConfig.SetpointRepublishIntervalMilliseconds):
                    config.SetpointRepublishIntervalMilliseconds = value;
                    break;
                default:
                    throw new ArgumentException($"Unknown property {propertyName}.", nameof(propertyName));
            }
        }
    }
}
