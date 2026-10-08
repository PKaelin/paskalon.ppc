// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.Configs;

namespace paskalON.Devices.Domain.Configs.UnitTest
{
    [TestClass]
    public class SystemConfigTest
    {
        private const string Metrics = nameof(SystemConfig.MetricsIntervalMilliseconds);
        private const string Polling = nameof(SystemConfig.PollingIntervalMilliseconds);
        private const string Device = nameof(SystemConfig.DeviceIntervalMilliseconds);
        private const string Heartbeat = nameof(SystemConfig.DeviceHeartbeatIntervalMilliseconds);


        [TestMethod]
        [DataRow(PowerControlType.Bess)]
        [DataRow(PowerControlType.Solar)]
        [DataRow(PowerControlType.Wind)]
        [DataRow(PowerControlType.Nuclear)]
        [DataRow(PowerControlType.Hydro)]
        [DataRow(PowerControlType.Gas)]
        public void TypeSingleTest(PowerControlType type)
        {
            SystemConfig config = CreateConfig();

            config.Type = type;

            Assert.AreEqual(type, config.Type);
        }


        [TestMethod]
        [DataRow(PowerControlType.Bess | PowerControlType.Solar)]
        [DataRow(PowerControlType.Wind | PowerControlType.Gas | PowerControlType.Hydro)]
        [DataRow((PowerControlType)0x40)]
        [DataRow((PowerControlType)(-1))]
        public void TypeInvalidTest(PowerControlType type)
        {
            SystemConfig config = CreateConfig();

            Action set = () => config.Type = type;

            Assert.ThrowsExactly<ArgumentException>(set);
            Assert.AreEqual(PowerControlType.Bess, config.Type);
        }


        [TestMethod]
        public void TypeNoneTest()
        {
            SystemConfig config = CreateConfig();

            Action set = () => config.Type = PowerControlType.None;

            Assert.ThrowsExactly<ArgumentException>(set);
        }


        [TestMethod]
        [DataRow(Metrics, 200)]
        [DataRow(Polling, 100)]
        [DataRow(Device, 100)]
        [DataRow(Heartbeat, 100)]
        [DataRow(Metrics, int.MaxValue)]
        public void IntervalAtMinimumTest(string propertyName, int interval)
        {
            SystemConfig config = CreateConfig();

            SetInterval(config, propertyName, interval);

            Assert.AreEqual(interval, GetInterval(config, propertyName));
        }


        [TestMethod]
        [DataRow(Metrics, 199)]
        [DataRow(Polling, 99)]
        [DataRow(Device, 99)]
        [DataRow(Heartbeat, 99)]
        [DataRow(Metrics, 0)]
        [DataRow(Polling, -1)]
        public void IntervalBelowMinimumTest(string propertyName, int interval)
        {
            SystemConfig config = CreateConfig();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SetInterval(config, propertyName, interval));

            Assert.AreEqual(1000, GetInterval(config, propertyName));
        }


        [TestMethod]
        [DataRow(nameof(SystemConfig.DeviceFactorCore), 1)]
        [DataRow(nameof(SystemConfig.DeviceFactorDetail), 1)]
        [DataRow(nameof(SystemConfig.StartupDelayForDevices), 0)]
        [DataRow(nameof(SystemConfig.StartupDelayForDevices), int.MaxValue)]
        public void SetAtLowerLimitTest(string propertyName, int value)
        {
            SystemConfig config = CreateConfig();

            PropertyAccessor.Set(config, propertyName, value);

            Assert.AreEqual(value, PropertyAccessor.Get(config, propertyName));
        }


        [TestMethod]
        [DataRow(nameof(SystemConfig.DeviceFactorCore), 0)]
        [DataRow(nameof(SystemConfig.DeviceFactorDetail), 0)]
        [DataRow(nameof(SystemConfig.DeviceFactorCore), -1)]
        [DataRow(nameof(SystemConfig.StartupDelayForDevices), -1)]
        public void SetBelowLowerLimitTest(string propertyName, int value)
        {
            SystemConfig config = CreateConfig();
            object? original = PropertyAccessor.Get(config, propertyName);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PropertyAccessor.Set(config, propertyName, value));

            Assert.AreEqual(original, PropertyAccessor.Get(config, propertyName));
        }


        private SystemConfig CreateConfig()
        {
            SystemConfig config = new SystemConfig
            {
                ChangedBy = TestConfigFactory.ChangedBy,
                Type = PowerControlType.Bess
            };

            return config;
        }


        private static void SetInterval(SystemConfig config, string propertyName, int interval)
        {
            switch (propertyName)
            {
                case Metrics:
                    config.MetricsIntervalMilliseconds = interval;
                    break;
                case Polling:
                    config.PollingIntervalMilliseconds = interval;
                    break;
                case Device:
                    config.DeviceIntervalMilliseconds = interval;
                    break;
                case Heartbeat:
                    config.DeviceHeartbeatIntervalMilliseconds = interval;
                    break;
                default:
                    throw new ArgumentException($"Unknown property {propertyName}.", nameof(propertyName));
            }
        }


        private static int GetInterval(SystemConfig config, string propertyName)
        {
            return propertyName switch
            {
                Metrics => config.MetricsIntervalMilliseconds,
                Polling => config.PollingIntervalMilliseconds,
                Device => config.DeviceIntervalMilliseconds,
                Heartbeat => config.DeviceHeartbeatIntervalMilliseconds,
                _ => throw new ArgumentException($"Unknown property {propertyName}.", nameof(propertyName))
            };
        }
    }
}
