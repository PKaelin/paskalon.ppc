// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.ConstraintEngine.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain.UnitTest.Systems
{
    [TestClass]
    public class SystemPowerConstraintTest
    {
        private SystemPowerConstraintConfig? _config;
        private SystemPowerConstraintMap? _map;


        [TestInitialize]
        public void Initialize()
        {
            _config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                IsActive = true,
                IsEnabled = true,
                MaximumActivePowerKiloWatt = double.MaxValue,
                MinimumActivePowerKiloWatt = double.MinValue,
                MaximumReactivePowerKiloVars = double.MaxValue,
                MinimumReactivePowerKiloVars = double.MinValue,
                DeratePerUnitInMaintenance = true,
                DeratePerUnitStopped = true,
            };

            _map = new SystemPowerConstraintMap
            {
            };
        }


        [TestMethod]
        public void CreateWithNullConfigTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerConstraint(NullLogger.Instance, null, _map!));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullMapperTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerConstraint(NullLogger.Instance, _config!, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestMethod]
        public void CreateConstraintTest()
        {
            SystemPowerConstraint constraint = new SystemPowerConstraint(NullLogger.Instance, _config!, _map!);

            Assert.IsNotNull(constraint.Name);
            Assert.AreEqual(_config!.Name, constraint.Name);
            Assert.AreEqual(_config!.IsEnabled, constraint.IsEnabled);
        }


        [TestMethod]
        public void CreateApplyActivePowerMaximumLimitTest()
        {
            FakeLogger logger = new FakeLogger();
            _config!.MaximumActivePowerKiloWatt = 10;
            SystemPowerConstraint constraint = new SystemPowerConstraint(logger, _config!, _map!);
            ActivePower activePower = ActivePower.FromKilo(20);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();

            constraint.ApplyLimits(ref activePower, ref reactivePower);

            Assert.AreEqual(10, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyActivePowerMinimumLimitTest()
        {
            FakeLogger logger = new FakeLogger();
            _config!.MinimumActivePowerKiloWatt = -20;
            SystemPowerConstraint constraint = new SystemPowerConstraint(logger, _config!, _map!);
            ActivePower activePower = ActivePower.FromKilo(-40);
            ReactivePower reactivePower = ReactivePower.FromKilo(0);
            logger.Collector.Clear();

            constraint.ApplyLimits(ref activePower, ref reactivePower);

            Assert.AreEqual(-20, activePower.KiloWatts);
            Assert.AreEqual(0, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("below minimum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyReactivePowerMaximumLimitTest()
        {
            FakeLogger logger = new FakeLogger();
            _config!.MaximumReactivePowerKiloVars = 10;
            SystemPowerConstraint constraint = new SystemPowerConstraint(logger, _config!, _map!);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(20);
            logger.Collector.Clear();

            constraint.ApplyLimits(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(10, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("exceeds maximum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyReactivePowerMinimumLimitTest()
        {
            FakeLogger logger = new FakeLogger();
            _config!.MinimumReactivePowerKiloVars = -20;
            SystemPowerConstraint constraint = new SystemPowerConstraint(logger, _config!, _map!);
            ActivePower activePower = ActivePower.FromKilo(0);
            ReactivePower reactivePower = ReactivePower.FromKilo(-40);
            logger.Collector.Clear();

            constraint.ApplyLimits(ref activePower, ref reactivePower);

            Assert.AreEqual(0, activePower.KiloWatts);
            Assert.AreEqual(-20, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("below minimum limit", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public void CreateApplyIsNotEnabledMaximumTest()
        {
            FakeLogger logger = new FakeLogger();
            _config!.IsEnabled = false;
            _config!.MaximumActivePowerKiloWatt = 10;
            _config!.MaximumReactivePowerKiloVars = 10;
            SystemPowerConstraint constraint = new SystemPowerConstraint(logger, _config!, _map!);
            ActivePower activePower = ActivePower.FromKilo(40);
            ReactivePower reactivePower = ReactivePower.FromKilo(40);
            logger.Collector.Clear();

            constraint.ApplyLimits(ref activePower, ref reactivePower);

            Assert.AreEqual(40, activePower.KiloWatts);
            Assert.AreEqual(40, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }


        [TestMethod]
        public void CreateApplyIsNotEnabledMinimumTest()
        {
            FakeLogger logger = new FakeLogger();
            _config!.IsEnabled = false;
            _config!.MinimumActivePowerKiloWatt = -20;
            _config!.MinimumReactivePowerKiloVars = -20;
            SystemPowerConstraint constraint = new SystemPowerConstraint(logger, _config!, _map!);
            ActivePower activePower = ActivePower.FromKilo(-40);
            ReactivePower reactivePower = ReactivePower.FromKilo(-40);
            logger.Collector.Clear();

            constraint.ApplyLimits(ref activePower, ref reactivePower);

            Assert.AreEqual(-40, activePower.KiloWatts);
            Assert.AreEqual(-40, reactivePower.KiloVoltAmperesReactive);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Warning);
            Assert.HasCount(0, logs);
        }

    }
}
