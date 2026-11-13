// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs.UnitTest
{
    /// <summary>
    /// Tests <see cref="SystemConfig"/>.
    /// </summary>
    [TestClass]
    public class SystemConfigTest
    {
        /// <summary>
        /// Name of the send interval property.
        /// </summary>
        private const string SendInterval = nameof(SystemConfig.AlarmActionSendIntervalMilliseconds);


        /// <summary>
        /// Name of the retry interval property.
        /// </summary>
        private const string RetryInterval = nameof(SystemConfig.AlarmActionRetryIntervalMilliseconds);


        /// <summary>
        /// Name of the delay check interval property.
        /// </summary>
        private const string DelayCheckInterval = nameof(SystemConfig.AlarmDelayCheckIntervalMilliseconds);


        /// <summary>
        /// Name of the heartbeat interval property.
        /// </summary>
        private const string HeartbeatInterval = nameof(SystemConfig.DependencyHeartbeatIntervalMilliseconds);


        /// <summary>
        /// A new configuration has the default intervals, no exchange and no heartbeat topic.
        /// </summary>
        [TestMethod]
        public void SystemConfigDefaultsTest()
        {
            SystemConfig config = CreateConfig();

            int sendInterval = config.AlarmActionSendIntervalMilliseconds;
            int retryInterval = config.AlarmActionRetryIntervalMilliseconds;
            int delayCheckInterval = config.AlarmDelayCheckIntervalMilliseconds;
            int heartbeatInterval = config.DependencyHeartbeatIntervalMilliseconds;

            Assert.AreEqual(1000, sendInterval);
            Assert.AreEqual(5000, retryInterval);
            Assert.AreEqual(500, delayCheckInterval);
            Assert.AreEqual(500, heartbeatInterval);
            Assert.IsNull(config.PublisherExchangeAlarmActions);
            Assert.IsNull(config.PublisherTopicHeartbeat);
        }


        /// <summary>
        /// Intervals at or above the minimum are accepted.
        /// </summary>
        [TestMethod]
        [DataRow(SendInterval, 100)]
        [DataRow(SendInterval, int.MaxValue)]
        [DataRow(RetryInterval, 1000)]
        [DataRow(RetryInterval, int.MaxValue)]
        [DataRow(DelayCheckInterval, 100)]
        [DataRow(DelayCheckInterval, int.MaxValue)]
        [DataRow(HeartbeatInterval, 100)]
        [DataRow(HeartbeatInterval, int.MaxValue)]
        public void IntervalAtOrAboveMinimumTest(string propertyName, int interval)
        {
            SystemConfig config = CreateConfig();

            SetInterval(config, propertyName, interval);

            Assert.AreEqual(interval, GetInterval(config, propertyName));
        }


        /// <summary>
        /// Intervals below the minimum are rejected and the previous value is kept.
        /// </summary>
        [TestMethod]
        [DataRow(SendInterval, 99, 1000)]
        [DataRow(SendInterval, 0, 1000)]
        [DataRow(SendInterval, -1, 1000)]
        [DataRow(RetryInterval, 999, 5000)]
        [DataRow(RetryInterval, 0, 5000)]
        [DataRow(DelayCheckInterval, 99, 500)]
        [DataRow(DelayCheckInterval, 0, 500)]
        [DataRow(HeartbeatInterval, 99, 500)]
        [DataRow(HeartbeatInterval, 0, 500)]
        public void IntervalBelowMinimumThrowsTest(string propertyName, int interval, int expectedInterval)
        {
            SystemConfig config = CreateConfig();

            Action set = () => SetInterval(config, propertyName, interval);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
            Assert.AreEqual(expectedInterval, GetInterval(config, propertyName));
        }


        /// <summary>
        /// Creates a system configuration with the default values.
        /// </summary>
        private SystemConfig CreateConfig()
        {
            return new SystemConfig
            {
                ChangedBy = "Homer",
                ChangedDate = new DateTimeOffset(2026, 11, 6, 8, 0, 0, TimeSpan.Zero)
            };
        }


        /// <summary>
        /// Sets the interval of the property.
        /// </summary>
        private void SetInterval(SystemConfig config, string propertyName, int interval)
        {
            switch (propertyName)
            {
                case SendInterval:
                    config.AlarmActionSendIntervalMilliseconds = interval;
                    break;
                case RetryInterval:
                    config.AlarmActionRetryIntervalMilliseconds = interval;
                    break;
                case DelayCheckInterval:
                    config.AlarmDelayCheckIntervalMilliseconds = interval;
                    break;
                case HeartbeatInterval:
                    config.DependencyHeartbeatIntervalMilliseconds = interval;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName));
            }
        }


        /// <summary>
        /// Gets the interval of the property.
        /// </summary>
        private int GetInterval(SystemConfig config, string propertyName)
        {
            return propertyName switch
            {
                SendInterval => config.AlarmActionSendIntervalMilliseconds,
                RetryInterval => config.AlarmActionRetryIntervalMilliseconds,
                DelayCheckInterval => config.AlarmDelayCheckIntervalMilliseconds,
                HeartbeatInterval => config.DependencyHeartbeatIntervalMilliseconds,
                _ => throw new ArgumentOutOfRangeException(nameof(propertyName))
            };
        }
    }
}
