// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.OpenModes
{
    /// <summary>
    /// Base constructor parameters are tested in <see cref="OperatingModeBaseTest"/>.
    /// </summary>
    [TestClass]
    public class MaintenanceModeTest
    {
        private MaintenanceMode? _mode;
        private MaintenanceModeMap? _map;
        private Mock<IRampController>? _rampActive;
        private Mock<IRampController>? _rampReactive;
        private SystemConfig? _systemConfig;
        private MaintenanceModeConfig? _config;


        [TestInitialize]
        public void Initialize()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            // Der
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<Der> der = new Mock<Der>(NullLogger.Instance, derConfig.Object);
            // Group
            Mock<DerGroupConfig> groupConfig = new Mock<DerGroupConfig>();
            groupConfig.SetupGet(x => x.Name).Returns("DerGroupConfig");
            Mock<DerGroup> group = new Mock<DerGroup>(NullLogger.Instance, groupConfig.Object, der.Object);
            // Circuit
            Mock<DerCircuitConfig> circuitConfig = new Mock<DerCircuitConfig>();
            circuitConfig.SetupGet(x => x.Name).Returns("DerCircuitConfig");
            Mock<DerCircuit> circuit = new Mock<DerCircuit>(NullLogger.Instance, circuitConfig.Object, group.Object);

            Mock<DerBatteryStorageUnitConfig> unitConfig = new Mock<DerBatteryStorageUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerBatteryStorageUnitConfig");
            Mock<DerBatteryStorageUnit> derUnit = new Mock<DerBatteryStorageUnit>(NullLogger.Instance, unitConfig.Object, circuit.Object);

            _systemConfig = new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerWatt = double.MinValue,
                NameplateMaximumActivePowerWatt = double.MaxValue,
                NameplateMinimumReactivePowerVars = double.MinValue,
                NameplateMaximumReactivePowerVars = double.MaxValue,
            };

            _config = new MaintenanceModeConfig
            {
                ChangedBy = "Test",
                Name = "MaintenanceModeConfig",
                IsActive = true,
                Type = PowerControlType.Bess,
                MaximumActivePowerLimitWatt = double.MaxValue,
                MinimumActivePowerLimitWatt = double.MinValue,
                MaximumReactivePowerLimitVars = double.MaxValue,
                MinimumReactivePowerLimitVars = double.MinValue,
                RampConfig = new Mock<RampBaseConfig>().Object
            };

            _map = new MaintenanceModeMap { AvailableActivePower = () => null, AvailableReactivePower = () => null, DerUnit = () => derUnit.Object };
            _rampActive = new Mock<IRampController>();
            _rampReactive = new Mock<IRampController>();
            _rampActive.Setup(x => x.ShallowCopy()).Returns(_rampReactive.Object);
            _mode = new MaintenanceMode(NullLogger.Instance, TimeProvider.System, publisher.Object, _systemConfig, _config, derUnit.Object, _map, _rampActive.Object);
        }


        [TestMethod]
        public void CreateWithNullDerUnitTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<MaintenanceModeConfig> config = new Mock<MaintenanceModeConfig>();
            Mock<MaintenanceModeMap> map = new Mock<MaintenanceModeMap>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
            Assert.ThrowsExactly<ArgumentNullException>(() => new MaintenanceMode(NullLogger.Instance, TimeProvider.System, publisher.Object,
                systemConfig.Object, config.Object, null!, map.Object, ramp.Object, curve.Object));
        }


        [TestMethod]
        public void CreateOperatingModeTest()
        {
            Assert.IsNotNull(_mode!.RampControllerActive);
            Assert.IsFalse(_mode!.IsEnabled);
            Assert.AreEqual(_config!.Name, _mode!.Name);
            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
        }


        [TestMethod]
        public void CalculateStateDisabledTest()
        {
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledNoAvailableTest()
        {
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableNoSetpointTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(10000);
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(10000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointBeforeEnabledTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(10000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(10000);
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(5000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(5000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(10000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(5000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 10000000), Times.Once);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 5000000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointBeforeEnabledNegativeTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(-10000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(-10000);
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(-5000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(-5000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-10000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(-5000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, -10000000), Times.Once);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, -5000000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableUpperLimitTest()
        {
            _config!.MaximumActivePowerLimitWatt = 700;
            _config!.MaximumReactivePowerLimitVars = 600;
            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(1000);
            _mode!.Enable();
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(1000);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 700), Times.Once);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 600), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableLowerLimitTest()
        {
            _config!.MinimumActivePowerLimitWatt = -500;
            _config!.MinimumReactivePowerLimitVars = -400;

            _map!.AvailableActivePower = () => ActivePower.FromKilo(-1000);
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(-1000);
            _mode!.Enable();
            _mode!.SetpointActivePower = ActivePower.FromKilo(-1000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(-1000);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(-1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, -500), Times.Once);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, -400), Times.Once);
        }

    }
}
