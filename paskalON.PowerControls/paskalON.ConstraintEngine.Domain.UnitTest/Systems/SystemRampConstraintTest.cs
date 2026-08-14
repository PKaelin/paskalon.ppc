
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
                MaximumActivePowerKiloWattRampRatePerSecond = double.MaxValue,
                MaximumReactivePowerKiloVarsRampRatePerSecond = double.MaxValue,
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
            _config!.MaximumActivePowerKiloWattRampRatePerSecond = 10;
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
        public void CreateApplyReactivePowerMaximumInitialInLimitTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 20;
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
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 10;
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
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 10;
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
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 10;
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
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 10;
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
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 10;
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
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 10;
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
            _config!.MaximumReactivePowerKiloVarsRampRatePerSecond = 10;
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
    }
}
