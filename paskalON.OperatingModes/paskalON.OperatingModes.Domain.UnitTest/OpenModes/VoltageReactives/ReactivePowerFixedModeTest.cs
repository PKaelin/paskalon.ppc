// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.OpenModes.VoltageReactives
{
    /// <summary>
    /// Base constructor parameters are tested in <see cref="OperatingModeBaseTest"/>.
    /// </summary>
    [TestClass]
    public class ReactivePowerFixedModeTest
    {
        private ReactivePowerFixedMode? _mode;
        private ReactivePowerFixedModeMap? _map;
        private Mock<IRampController>? _rampReactive;
        private SystemConfig? _systemConfig;
        private ReactivePowerFixedModeConfig? _config;


        [TestInitialize]
        public void Initialize()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();

            _systemConfig = new SystemConfig
            {
                ChangedBy = "Test",
                Type = OperatingModeType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerKiloWatt = double.MinValue,
                NameplateMaximumActivePowerKiloWatt = double.MaxValue,
                NameplateMinimumReactivePowerKiloVars = double.MinValue,
                NameplateMaximumReactivePowerKiloVars = double.MaxValue,
            };

            _config = new ReactivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerFixedModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = new Mock<RampBaseConfig>().Object
            };

            _map = new ReactivePowerFixedModeMap { AvailableActivePower = () => null, AvailableReactivePower = () => null };
            Mock<IRampController> rampActive = new Mock<IRampController>();
            _rampReactive = new Mock<IRampController>();
            rampActive.Setup(x => x.ShallowCopy()).Returns(_rampReactive.Object);
            _mode = new ReactivePowerFixedMode(NullLogger.Instance, TimeProvider.System, publisher.Object, _systemConfig, _config, _map, rampActive.Object);
        }


        [TestMethod]
        public void CreateOperatingModeTest()
        {
            Assert.IsNotNull(_mode!.RampControllerActive);
            Assert.IsFalse(_mode!.IsEnabled);
            Assert.AreEqual(_config!.Name, _mode!.Name);
            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
        }


        [TestMethod]
        public void CalculateStateDisabledTest()
        {
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledNoAvailableTest()
        {
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableNoSetpointTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(10000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointBeforeEnabledTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(10000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(10000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(10000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 10000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointBeforeEnabledNegativeTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(-1000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(-10000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-10000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, -1000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(101);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(101, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 100), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledNegativeTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(-100);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(-101);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-101, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, -100), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledAndDeadbandTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(100, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledAndDeadbandNegativeTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(-100);
            _mode!.Enable();
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(-100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(-100, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointRampUpReachedTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(100);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(101);
            _mode!.Enable();
            _rampReactive!.Setup(x => x.Calculate()).Returns(100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(101, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(101, _mode!.TargetReactivePower.KiloVoltAmperesReactive);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 100), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledDisabledAvailableSetpointRampDownReachedTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(100);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.Disable();
            _rampReactive!.Setup(x => x.Calculate()).Returns(10);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Exactly(2));
            _rampReactive!.Verify(x => x.Start(0, 100), Times.Once);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), 0), Times.Once);
            _rampReactive!.Verify(x => x.Stop(), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableUpperLimitTest()
        {
            _config!.MaximumReactivePowerLimitKiloVars = 500;

            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(1000);
            _mode!.Enable();
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(1000);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 500), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableLowerLimitTest()
        {
            _config!.MinimumReactivePowerLimitKiloVars = -400;

            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(-1000);
            _mode!.Enable();
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(-1000);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, -400), Times.Once);
        }



        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAvailableGettingBiggerTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(100);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.CalculateAsync();
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(500);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(100, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 100), Times.Once);
        }

    }
}
