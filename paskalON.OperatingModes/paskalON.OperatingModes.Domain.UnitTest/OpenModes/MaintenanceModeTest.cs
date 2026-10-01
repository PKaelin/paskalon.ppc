// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.Ramps;
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
                DerUnitName = "DerBatteryStorageUnitConfig",
                IsActive = true,
                Type = PowerControlType.Bess,
                MaximumActivePowerLimitWatt = double.MaxValue,
                MinimumActivePowerLimitWatt = double.MinValue,
                MaximumReactivePowerLimitVars = double.MaxValue,
                MinimumReactivePowerLimitVars = double.MinValue,
                RampConfig = new Mock<RampBaseConfig>().Object
            };

            _map = new MaintenanceModeMap { AvailableActivePower = () => null, AvailableReactivePower = () => null, DerUnitName = _config.DerUnitName };
            _rampActive = new Mock<IRampController>();
            _rampReactive = new Mock<IRampController>();
            _rampActive.Setup(x => x.ShallowCopy()).Returns(_rampReactive.Object);
            _mode = new MaintenanceMode(NullLogger.Instance, TimeProvider.System, publisher.Object, _systemConfig, _config, _map, _rampActive.Object);
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
