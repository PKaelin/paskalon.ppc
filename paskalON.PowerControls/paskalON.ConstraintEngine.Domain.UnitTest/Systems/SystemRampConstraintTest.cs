
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.ConstraintEngine.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain.UnitTest.Systems
{
    [TestClass]
    public class SystemRampConstraintTest
    {
        private SystemRampConstraintConfig? _config;
        private SystemRampConstraintMap? _map;


        [TestInitialize]
        public void Initialize()
        {
            _config = new SystemRampConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemRampConstraintConfig",
                MaximumActivePowerWattRampRatePerSecond = double.MaxValue,
                MaximumReactivePowerVarsRampRatePerSecond = double.MaxValue,
            };

            _map = new SystemRampConstraintMap
            {
            };
        }


        [TestMethod]
        public void CreateWithNullConfigTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemRampConstraint(NullLogger.Instance, null, _map!, timeProvider));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullMapperTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemRampConstraint(NullLogger.Instance, _config!, null, timeProvider));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullTimeProviderTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemRampConstraint(NullLogger.Instance, _config!, _map!, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateConstraintTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            SystemRampConstraint constraint = new SystemRampConstraint(NullLogger.Instance, _config!, _map!, timeProvider);

            Assert.IsNotNull(constraint.Name);
            Assert.AreEqual(_config!.Name, constraint.Name);
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumInitialInLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(5);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(5, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumInitialOutLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(20);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(10, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumInitialOutLimitNegativeTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(-20);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(-10, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumInLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(5);
            reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(5, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumInLimitHalfSecondTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(5);
            reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(0.5));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(5, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumOutLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(20);
            reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(10, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumOutLimitHalfSecondTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(20);
            reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(0.5));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(5, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumOutLimitNegativeTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(-20);
            reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(-10, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyPowerRampUsesSignedDeltaFromPreviousTargetTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            _config.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(NullLogger.Instance, _config, _map!, timeProvider);
            ActivePower initialActivePower = ActivePower.FromKilo(5);
            ReactivePower initialReactivePower = ReactivePower.FromKilo(5);
            constraint.ApplyConstraints(ref initialActivePower, ref initialReactivePower);
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            ActivePower activePower = ActivePower.FromKilo(20);
            ReactivePower reactivePower = ReactivePower.FromKilo(-20);

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(15, activePower.KiloWatts);
            Assert.AreEqual(-5, reactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumInitialInLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 20000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(10);
            logger.Collector.Clear();

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(10, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumInitialOutLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(20);
            logger.Collector.Clear();

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(10, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumInitialOutLimitNegativeTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(-20);
            logger.Collector.Clear();

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(-10, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumInLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(0);
            reactivePower = ReactivePower.FromKilo(5);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(5, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumInLimitHalfSecondTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(0);
            reactivePower = ReactivePower.FromKilo(5);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(0.5));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(5, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumOutLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(0);
            reactivePower = ReactivePower.FromKilo(20);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(10, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumOutLimitHalfSecondTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(0);
            reactivePower = ReactivePower.FromKilo(20);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(0.5));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(5, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumOutLimitNegativeTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            constraint.ApplyConstraints(ref activePower, ref reactivePower);
            activePower = ActivePower.FromKilo(0);
            reactivePower = ReactivePower.FromKilo(-20);
            logger.Collector.Clear();
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(-10, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        [DataRow(1, 1000000, 1000000)]
        [DataRow(2, 2000000, 2000000)]
        [DataRow(60, 2000000, 2000000)]
        [DataRow(3600, 2000000, 2000000)]
        public void ApplyConstraintsGapLimitsStepToMaximumRampIntervalTest(int gapSeconds, double expectedActivePowerWatts, double expectedReactivePowerVars)
        {
            // 1 MW/s ramp rate and 1 Mvar/s with a maximum ramp interval of 2 s, the setpoints jump from 0 to 20 MW and 20 Mvar after the gap
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            _config!.MaximumActivePowerWattRampRatePerSecond = 1000000;
            _config.MaximumReactivePowerVarsRampRatePerSecond = 1000000;
            _config.MaximumRampIntervalMilliseconds = 2000;
            SystemRampConstraint constraint = new SystemRampConstraint(NullLogger.Instance, _config, _map!, timeProvider);
            ActivePower initialActivePower = ActivePower.FromMega(0);
            ReactivePower initialReactivePower = ReactivePower.FromMega(0);
            constraint.ApplyConstraints(ref initialActivePower, ref initialReactivePower);
            timeProvider.Advance(TimeSpan.FromSeconds(gapSeconds));
            ActivePower activePower = ActivePower.FromMega(20);
            ReactivePower reactivePower = ReactivePower.FromMega(20);

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(expectedActivePowerWatts, activePower.Watts);
            Assert.AreEqual(expectedReactivePowerVars, reactivePower.VoltAmperesReactive);
        }


        [TestMethod]
        [DataRow(15000000, 15005000, 15005000)]
        [DataRow(15000000, 0, 14990000)]
        [DataRow(-8000000, -8000000, -8000000)]
        public void ApplyConstraintsFirstTargetStartsFromCurrentDeviceTargetTest(double currentTargetWatts, double setpointWatts, double expectedWatts)
        {
            // A restarted power control receives the setpoint while the devices still apply their last target
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            _config.MaximumReactivePowerVarsRampRatePerSecond = 10000;

            SystemRampConstraintMap map = new SystemRampConstraintMap
            {
                CurrentActivePowerTarget = () => new ActivePower(currentTargetWatts),
                CurrentReactivePowerTarget = () => new ReactivePower(250000)
            };

            SystemRampConstraint constraint = new SystemRampConstraint(NullLogger.Instance, _config, map, timeProvider);
            ActivePower activePower = new ActivePower(setpointWatts);
            ReactivePower reactivePower = new ReactivePower(250000);

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(expectedWatts, activePower.Watts);
            Assert.AreEqual(250000, reactivePower.VoltAmperesReactive);
        }


        [TestMethod]
        public void ApplyConstraintsFirstTargetUnknownDeviceTargetStartsFromZeroTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            _config.MaximumReactivePowerVarsRampRatePerSecond = 10000;

            SystemRampConstraintMap map = new SystemRampConstraintMap
            {
                CurrentActivePowerTarget = () => null,
                CurrentReactivePowerTarget = () => null
            };

            SystemRampConstraint constraint = new SystemRampConstraint(NullLogger.Instance, _config, map, timeProvider);
            ActivePower activePower = ActivePower.FromMega(15);
            ReactivePower reactivePower = ReactivePower.FromKilo(5);

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(10000, activePower.Watts);
            Assert.AreEqual(5000, reactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms a ramp over several periodically applied setpoints is logged once when it starts and once when it reached the setpoint.
        /// </summary>
        [TestMethod]
        public void ApplyConstraintsLogsRampOnceUntilReachedTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            SystemRampConstraint constraint = new SystemRampConstraint(logger, _config!, _map!, timeProvider);
            List<double> appliedActivePowers = new List<double>();

            for (int i = 0; i < 4; i++)
            {
                ActivePower activePower = ActivePower.FromKilo(25);
                ReactivePower reactivePower = ReactivePower.FromKilo(0);
                constraint.ApplyConstraints(ref activePower, ref reactivePower);
                appliedActivePowers.Add(activePower.Watts);
                timeProvider.Advance(TimeSpan.FromSeconds(1));
            }

            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            CollectionAssert.AreEqual(new List<double> { 10000, 20000, 25000, 25000 }, appliedActivePowers);
            Assert.HasCount(1, logs.Where(l => l.Level == LogLevel.Warning && l.Message.Contains("active power ramp exceeds maximum limit")));
            Assert.HasCount(1, logs.Where(l => l.Level == LogLevel.Information && l.Message.Contains("active power ramp reached 25000 within the limit")));
        }
    }
}
