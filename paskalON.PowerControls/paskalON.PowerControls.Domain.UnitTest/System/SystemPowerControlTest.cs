// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.ConstraintEngine.Domain.Ders;
using paskalON.ConstraintEngine.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Strategies;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.PowerControls.Domain.Systems;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain.UnitTest.System
{
    [TestClass]
    public sealed class SystemPowerControlTest
    {
        private Mock<ILogger> _logger = new Mock<ILogger>();
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private SystemPowerControlConfig _config = null!;
        private Mock<SystemPowerControlMap> _map = new Mock<SystemPowerControlMap>();
        private Mock<DistributionStrategyProfile> _distributions = new Mock<DistributionStrategyProfile>();
        private IEnumerable<ISystemConstraint> _constraints = new List<ISystemConstraint>();
        private IEnumerable<DerUnitPowerControl> _units = new List<DerUnitPowerControl>();
        private Mock<IDerUnitPowerControl> _unit1 = null!;
        private Mock<IDerUnitPowerControl> _unit2 = null!;
        private Mock<IDerUnitPowerControl> _unit3 = null!;
        private Mock<IDerUnitPowerControl> _unit4 = null!;

        private Mock<IDistributionStrategy> _priority = new Mock<IDistributionStrategy>();
        private Mock<IDistributionStrategy> _equal = new Mock<IDistributionStrategy>();
        private Mock<IDistributionStrategy> _weighted = new Mock<IDistributionStrategy>();
        private Mock<IDistributionStrategy> _proportional = new Mock<IDistributionStrategy>();
        private Mock<IDistributionStrategy> _waterFilling = new Mock<IDistributionStrategy>();


        [TestInitialize]
        public void Initialize()
        {
            _unit1 = CreateUnit(DerState.Started);
            _unit2 = CreateUnit(DerState.Started);
            _unit3 = CreateUnit(DerState.Started);
            _unit4 = CreateUnit(DerState.Started);

            _config = new SystemPowerControlConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerControlConfig",
                IsActive = true,
                IsEnabled = true
            };
        }



        [TestMethod]
        public void SystemPowerControlNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(null!, _config, _map.Object,
                _publisher.Object, _constraints, _units, _distributions.Object));
        }



        [TestMethod]
        public void SystemPowerControlNullConfigTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(_logger.Object, null!, _map.Object,
                _publisher.Object, _constraints, _units, _distributions.Object));
        }



        [TestMethod]
        public void SystemPowerControlNullMapTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(_logger.Object, _config, null!,
                _publisher.Object, _constraints, _units, _distributions.Object));
        }


        [TestMethod]
        public void SystemPowerControlNullPublisherTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(_logger.Object, _config, _map.Object,
                null!, _constraints, _units, _distributions.Object));
        }


        [TestMethod]
        public void SystemPowerControlNullConstraintsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(_logger.Object, _config, _map.Object,
                _publisher.Object, null!, _units, _distributions.Object));
        }


        [TestMethod]
        public void SystemPowerControlNullUnitsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(_logger.Object, _config, _map.Object,
                _publisher.Object, _constraints, null!, _distributions.Object));
        }


        [TestMethod]
        public void SystemPowerControlNullDistributionsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(_logger.Object, _config, _map.Object,
                _publisher.Object, _constraints, _units, null!));
        }


        [TestMethod]
        public void SystemPowerControlConstructorTest()
        {
            SystemPowerControl control = new SystemPowerControl(_logger.Object, _config, _map.Object,
                _publisher.Object, _constraints, _units, _distributions.Object);

            Assert.IsNotNull(control.MetricsPublisher);
        }


        [TestMethod]
        public void SystemPowerControlDistributionStrategyValuesAreUniqueTest()
        {
            DistributionStrategyType[] values = Enum.GetValues<DistributionStrategyType>();

            Assert.AreEqual(values.Length, values.Select(value => (int)value).Distinct().Count());
        }


        [TestMethod]
        public void UpdatePowerSetsActiveAndReactiveSetpointsTest()
        {
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object]);
            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));

            Assert.AreEqual(5000, system.SetpointActivePower.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(5000, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerAllowsNegativeSetpointsTest()
        {
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object]);
            system.UpdatePower(new ActivePower(-5000), new ReactivePower(-2000));

            Assert.AreEqual(-5000, system.SetpointActivePower.Watts);
            Assert.AreEqual(-2000, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(-5000, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(-2000, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerZeroSetpointsTest()
        {
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object]);
            system.UpdatePower(new ActivePower(0), new ReactivePower(0));

            Assert.AreEqual(0, system.SetpointActivePower.Watts);
            Assert.AreEqual(0, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(0, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerWhenDisabledDoesNotUpdateSetpointsTest()
        {
            _config.IsEnabled = false;
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object]);
            system.UpdatePower(new ActivePower(1000), new ReactivePower(100));

            Assert.AreEqual(0, system.SetpointActivePower.Watts);
            Assert.AreEqual(0, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(0, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerNoUnitsNoDerateTest()
        {
            SystemPowerControl system = CreateSystemPowerControl(units: []);
            system.UpdatePower(new ActivePower(1000), new ReactivePower(100));

            Assert.AreEqual(1000, system.SetpointActivePower.Watts);
            Assert.AreEqual(100, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(1000, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(100, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerNoUnitsWithDerateTest()
        {
            SystemPowerConstraintConfig config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                DeratePerUnitStopped = true,
                DeratePerUnitInMaintenance = true
            };

            SystemPowerConstraint constraint = new SystemPowerConstraint(_logger.Object, config, new SystemPowerConstraintMap());

            SystemPowerControl system = CreateSystemPowerControl(units: [], constraints: [constraint]);
            system.UpdatePower(new ActivePower(1000), new ReactivePower(100));

            Assert.AreEqual(1000, system.SetpointActivePower.Watts);
            Assert.AreEqual(100, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(1000, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(100, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerOneUnitWithDerateTest()
        {
            SystemPowerConstraintConfig config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                DeratePerUnitStopped = true,
                DeratePerUnitInMaintenance = true
            };

            SystemPowerConstraint constraint = new SystemPowerConstraint(_logger.Object, config, new SystemPowerConstraintMap());
            _unit1.SetupGet(x => x.State).Returns(DerState.Stopped);
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object], constraints: [constraint]);
            system.UpdatePower(new ActivePower(1000), new ReactivePower(100));

            Assert.AreEqual(1000, system.SetpointActivePower.Watts);
            Assert.AreEqual(100, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(0, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }



        [TestMethod]
        public void UpdatePowerOneOfFourUnitsStoppedDeratesTo75PercentTest()
        {
            SystemPowerConstraintConfig config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                DeratePerUnitStopped = true,
                DeratePerUnitInMaintenance = true
            };

            SystemPowerConstraint constraint = new SystemPowerConstraint(_logger.Object, config, new SystemPowerConstraintMap());
            _unit1.SetupGet(x => x.State).Returns(DerState.Stopped);
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object, _unit2.Object, _unit3.Object, _unit4.Object], constraints: [constraint]);
            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));

            Assert.AreEqual(5000, system.SetpointActivePower.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(3750, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(1500, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerOneOfFourUnitsMaintenanceDeratesTo75PercentTest()
        {
            SystemPowerConstraintConfig config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                DeratePerUnitStopped = true,
                DeratePerUnitInMaintenance = true
            };

            SystemPowerConstraint constraint = new SystemPowerConstraint(_logger.Object, config, new SystemPowerConstraintMap());
            _unit1.SetupGet(x => x.State).Returns(DerState.Maintenance);
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object, _unit2.Object, _unit3.Object, _unit4.Object], constraints: [constraint]);
            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));

            Assert.AreEqual(5000, system.SetpointActivePower.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(3750, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(1500, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }



        [TestMethod]
        public void UpdatePowerOneOfFourUnitsOneStoppedOneMaintenanceDeratesTo75PercentTest()
        {
            SystemPowerConstraintConfig config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                DeratePerUnitStopped = true,
                DeratePerUnitInMaintenance = true
            };

            SystemPowerConstraint constraint = new SystemPowerConstraint(_logger.Object, config, new SystemPowerConstraintMap());
            _unit1.SetupGet(x => x.State).Returns(DerState.Maintenance);
            _unit2.SetupGet(x => x.State).Returns(DerState.Stopped);
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object, _unit2.Object, _unit3.Object, _unit4.Object], constraints: [constraint]);
            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));

            Assert.AreEqual(5000, system.SetpointActivePower.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(2500, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(1000, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdatePowerOneOfFourUnitsDerateStopSetToFalseTest()
        {
            SystemPowerConstraintConfig config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                DeratePerUnitStopped = false,
                DeratePerUnitInMaintenance = true
            };

            SystemPowerConstraint constraint = new SystemPowerConstraint(_logger.Object, config, new SystemPowerConstraintMap());
            _unit1.SetupGet(x => x.State).Returns(DerState.Stopped);
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object, _unit2.Object, _unit3.Object, _unit4.Object], constraints: [constraint]);
            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));

            Assert.AreEqual(5000, system.SetpointActivePower.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(5000, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }

        [TestMethod]
        public void UpdatePowerOneOfFourUnitsDerateMaintenanceSetToFalseTest()
        {
            SystemPowerConstraintConfig config = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                DeratePerUnitStopped = true,
                DeratePerUnitInMaintenance = false
            };

            SystemPowerConstraint constraint = new SystemPowerConstraint(_logger.Object, config, new SystemPowerConstraintMap());
            _unit1.SetupGet(x => x.State).Returns(DerState.Maintenance);
            SystemPowerControl system = CreateSystemPowerControl(units: [_unit1.Object, _unit2.Object, _unit3.Object, _unit4.Object], constraints: [constraint]);
            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));

            Assert.AreEqual(5000, system.SetpointActivePower.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(5000, system.SetpointActivePowerActual.Watts);
            Assert.AreEqual(2000, system.SetpointReactivePowerActual.VoltAmperesReactive);
        }


        [TestMethod]
        public void UpdateDerUnitPowerUnknownUnitThrowsTest()
        {
            Mock<IDerUnitPowerControl> unit = CreateNamedUnit("BESS-Unit 1", DerState.Maintenance);
            SystemPowerControl control = CreateSystemPowerControl(units: [unit.Object]);

            Assert.ThrowsExactly<KeyNotFoundException>(() => control.UpdateDerUnitPower("BESS-Unit 9", new ActivePower(1000), new ReactivePower(0)));

            unit.Verify(u => u.SetMaintenanceTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        [DataRow(DerState.Started)]
        [DataRow(DerState.Stopped)]
        [DataRow(DerState.Standby)]
        public void UpdateDerUnitPowerUnitNotInMaintenanceThrowsTest(DerState state)
        {
            Mock<IDerUnitPowerControl> unit = CreateNamedUnit("BESS-Unit 1", state);
            SystemPowerControl control = CreateSystemPowerControl(units: [unit.Object]);

            Assert.ThrowsExactly<InvalidOperationException>(() => control.UpdateDerUnitPower("BESS-Unit 1", new ActivePower(1000), new ReactivePower(0)));

            unit.Verify(u => u.SetMaintenanceTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        public void UpdateDerUnitPowerDisabledUnitThrowsTest()
        {
            Mock<IDerUnitPowerControl> unit = CreateNamedUnit("BESS-Unit 1", DerState.Maintenance, isEnabled: false);
            SystemPowerControl control = CreateSystemPowerControl(units: [unit.Object]);

            Assert.ThrowsExactly<InvalidOperationException>(() => control.UpdateDerUnitPower("BESS-Unit 1", new ActivePower(1000), new ReactivePower(0)));

            unit.Verify(u => u.SetMaintenanceTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void UpdateDerUnitPowerInvalidUnitNameThrowsTest(string? unitName)
        {
            SystemPowerControl control = CreateSystemPowerControl(units: [CreateNamedUnit("BESS-Unit 1", DerState.Maintenance).Object]);

            Assert.Throws<ArgumentException>(() => control.UpdateDerUnitPower(unitName!, new ActivePower(1000), new ReactivePower(0)));
        }


        [TestMethod]
        [DataRow(80000, -20000, 50000, -10000)]
        [DataRow(-80000, 20000, -50000, 10000)]
        [DataRow(25000, 4000, 25000, 4000)]
        public void UpdateDerUnitPowerAppliesUnitConstraintsAndSetsMaintenanceTargetTest(double activePowerWatt, double reactivePowerVar,
            double expectedActivePowerWatt, double expectedReactivePowerVar)
        {
            DerUnitPowerConstraintConfig constraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerLimits",
                MaximumActivePowerWatt = 50000,
                MinimumActivePowerWatt = -50000,
                MaximumReactivePowerVars = 10000,
                MinimumReactivePowerVars = -10000
            };
            DerUnitPowerConstraint constraint = new DerUnitPowerConstraint(_logger.Object, constraintConfig, new DerUnitPowerConstraintMap());
            Mock<IDerUnitPowerControl> maintenanceUnit = CreateNamedUnit("BESS-Unit 1", DerState.Maintenance, [constraint]);
            Mock<IDerUnitPowerControl> systemUnit = CreateNamedUnit("BESS-Unit 2", DerState.Started);
            SystemPowerControl control = CreateSystemPowerControl(units: [maintenanceUnit.Object, systemUnit.Object]);

            control.UpdateDerUnitPower("BESS-Unit 1", new ActivePower(activePowerWatt), new ReactivePower(reactivePowerVar));

            maintenanceUnit.Verify(u => u.SetMaintenanceTarget(new ActivePower(expectedActivePowerWatt), new ReactivePower(expectedReactivePowerVar)), Times.Once);
            maintenanceUnit.Verify(u => u.UpdatePower(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
            systemUnit.Verify(u => u.SetMaintenanceTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
            systemUnit.Verify(u => u.UpdatePower(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        public void UpdateDerUnitPowerDoesNotChangeSystemSetpointsTest()
        {
            SystemPowerControl control = CreateSystemPowerControl(units: [CreateNamedUnit("BESS-Unit 1", DerState.Maintenance).Object]);
            control.UpdatePower(new ActivePower(100000), new ReactivePower(20000));

            control.UpdateDerUnitPower("BESS-Unit 1", new ActivePower(5000), new ReactivePower(1000));

            Assert.AreEqual(new ActivePower(100000), control.SetpointActivePower);
            Assert.AreEqual(new ReactivePower(20000), control.SetpointReactivePower);
        }


        private static Mock<IDerUnitPowerControl> CreateNamedUnit(string unitName, DerState state, IEnumerable<IDerUnitConstraint>? constraints = null,
            bool isEnabled = true)
        {
            Mock<IDerUnitPowerControl> unit = new Mock<IDerUnitPowerControl>();

            unit.SetupGet(x => x.DerUnitName).Returns(unitName);
            unit.SetupGet(x => x.State).Returns(state);
            unit.SetupGet(x => x.IsEnabled).Returns(isEnabled);
            unit.SetupGet(x => x.Constraints).Returns(constraints ?? new List<IDerUnitConstraint>());
            unit.SetupGet(x => x.DistributionStrategyType).Returns(DistributionStrategyType.Equal);

            return unit;
        }


        private Mock<IDerUnitPowerControl> CreateUnit(DerState state)
        {
            Mock<IDerUnitPowerControl> unit = new Mock<IDerUnitPowerControl>();

            unit.SetupGet(x => x.State).Returns(state);
            unit.SetupGet(x => x.DistributionStrategyType).Returns(DistributionStrategyType.Equal);

            return unit;
        }


        [TestMethod]
        public void UpdatePowerLogsInformationOnlyForChangedTargetTest()
        {
            FakeLogger logger = new FakeLogger();
            SystemPowerControl system = new SystemPowerControl(logger, _config, _map.Object, _publisher.Object, _constraints, [_unit1.Object],
                _distributions.Object);

            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));
            system.UpdatePower(new ActivePower(5000), new ReactivePower(2000));
            system.UpdatePower(new ActivePower(6000), new ReactivePower(2000));

            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            List<FakeLogRecord> informationLogs = logs.Where(log => log.Level == LogLevel.Information && log.Message.StartsWith("Update system power control")).ToList();
            List<FakeLogRecord> debugLogs = logs.Where(log => log.Level == LogLevel.Debug && log.Message.StartsWith("Update system power control unchanged")).ToList();
            Assert.HasCount(2, informationLogs);
            Assert.AreEqual("Update system power control. Active Power 5000 W, Reactive Power 2000 VAR", informationLogs[0].Message);
            Assert.AreEqual("Update system power control. Active Power 6000 W, Reactive Power 2000 VAR", informationLogs[1].Message);
            Assert.HasCount(1, debugLogs);
        }


        private SystemPowerControl CreateSystemPowerControl(SystemPowerControlConfig? config = null, SystemPowerControlMap? map = null,
            IEnumerable<ISystemConstraint>? constraints = null, IEnumerable<IDerUnitPowerControl>? units = null,
            DistributionStrategyProfile? distribution = null)
        {
            SystemPowerControl control = new SystemPowerControl(_logger.Object, config ?? _config, map ?? _map.Object,
                _publisher.Object, constraints ?? _constraints, units ?? _units, distribution ?? _distributions.Object);

            return control;
        }
    }
}