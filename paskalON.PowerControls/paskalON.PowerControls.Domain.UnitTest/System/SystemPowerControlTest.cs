// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Moq;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
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


        private Mock<IDerUnitPowerControl> CreateUnit(DerState state)
        {
            Mock<IDerUnitPowerControl> unit = new Mock<IDerUnitPowerControl>();

            unit.SetupGet(x => x.State).Returns(state);
            unit.SetupGet(x => x.DistributionStrategyType).Returns(DistributionStrategyType.Equal);

            return unit;
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