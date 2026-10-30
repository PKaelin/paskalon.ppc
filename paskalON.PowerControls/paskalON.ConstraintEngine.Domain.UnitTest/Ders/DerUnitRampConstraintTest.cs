// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Ders;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerUnitRampConstraintTest
    {
        private DerUnitRampConstraintConfig? _config;
        private DerUnitRampConstraintMap? _map;


        [TestInitialize]
        public void Initialize()
        {
            _config = new DerUnitRampConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitRampConstraintConfig",
                MaximumActivePowerWattRampRatePerSecond = double.MaxValue,
                MaximumReactivePowerVarsRampRatePerSecond = double.MaxValue,
            };

            _map = new DerUnitRampConstraintMap
            {
            };
        }


        [TestMethod]
        public void CreateWithNullConfigTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitRampConstraint(NullLogger.Instance, null, _map!, timeProvider));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullMapperTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitRampConstraint(NullLogger.Instance, _config!, null, timeProvider));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullTimeProviderTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitRampConstraint(NullLogger.Instance, _config!, _map!, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateConstraintTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(NullLogger.Instance, _config!, _map!, timeProvider);

            Assert.IsNotNull(constraint.Name);
            Assert.AreEqual(_config!.Name, constraint.Name);
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumInitialInLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerWattRampRatePerSecond = 10000;
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
        public void CreateApplyReactivePowerMaximumInitialInLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerVarsRampRatePerSecond = 20000;
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(logger, _config!, _map!, timeProvider);
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
        [DataRow(1, 500000)]
        [DataRow(60, 1500000)]
        public void ApplyConstraintsGapLimitsStepToMaximumRampIntervalTest(int gapSeconds, double expectedActivePowerWatts)
        {
            // 500 kW/s ramp rate with a maximum ramp interval of 3 s, the setpoint jumps from 0 to 3 MW after the gap
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            _config!.MaximumActivePowerWattRampRatePerSecond = 500000;
            _config.MaximumRampIntervalMilliseconds = 3000;
            DerUnitRampConstraint constraint = new DerUnitRampConstraint(NullLogger.Instance, _config, _map!, timeProvider);
            ActivePower initialActivePower = ActivePower.FromMega(0);
            ReactivePower initialReactivePower = ReactivePower.FromMega(0);
            constraint.ApplyConstraints(ref initialActivePower, ref initialReactivePower);
            timeProvider.Advance(TimeSpan.FromSeconds(gapSeconds));
            ActivePower activePower = ActivePower.FromMega(3);
            ReactivePower reactivePower = ReactivePower.FromMega(0);

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(expectedActivePowerWatts, activePower.Watts);
        }


        [TestMethod]
        public void ApplyConstraintsFirstTargetStartsFromCurrentPcsTargetTest()
        {
            // A restarted power control receives the target while the PCS still applies its last target of 2.4 MW
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            _config!.MaximumActivePowerWattRampRatePerSecond = 100000;
            _config.MaximumReactivePowerVarsRampRatePerSecond = 100000;

            DerUnitRampConstraintMap map = new DerUnitRampConstraintMap
            {
                CurrentActivePowerTarget = () => ActivePower.FromMega(2.4),
                CurrentReactivePowerTarget = () => ReactivePower.FromKilo(-300)
            };

            DerUnitRampConstraint constraint = new DerUnitRampConstraint(NullLogger.Instance, _config, map, timeProvider);
            ActivePower activePower = ActivePower.FromMega(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(-300);

            constraint.ApplyConstraints(ref activePower, ref reactivePower);

            Assert.AreEqual(2300000, activePower.Watts);
            Assert.AreEqual(-300000, reactivePower.VoltAmperesReactive);
        }
    }
}
