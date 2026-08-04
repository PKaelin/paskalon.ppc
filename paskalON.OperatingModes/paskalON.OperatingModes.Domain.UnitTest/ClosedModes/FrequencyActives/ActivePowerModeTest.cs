// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.UnitTest.ClosedModes.FrequencyActives
{
    [TestClass]
    public class ActivePowerModeTest
    {
        private ActivePowerMode? _mode;
        private ActivePowerModeMap? _map;
        private Mock<IRampController>? _rampActive;
        private SystemConfig? _systemConfig;
        private ActivePowerModeConfig? _config;


        [TestInitialize]
        public void Initialize()
        {
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

            _config = new ActivePowerModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = new Mock<RampBaseConfig>().Object,
                ProportionalGain = 1.0,
            };

            _map = new ActivePowerModeMap { AvailableActivePower = () => null, AvailableReactivePower = () => null, ActivePowerAtPoi = () => null };
            _rampActive = new Mock<IRampController>();
            _rampActive.Setup(x => x.ShallowCopy()).Returns(new Mock<IRampController>().Object);
            _mode = new ActivePowerMode(NullLogger.Instance, TimeProvider.System, _systemConfig, _config, _map, _rampActive.Object);
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
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(10000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 10000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointBeforeEnabledNegativeTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(-1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(-10000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-10000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, -1000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.SetpointActivePower = ActivePower.FromKilo(101);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(101, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 100), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledNegativeTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(-100);
            _mode!.SetpointActivePower = ActivePower.FromKilo(-101);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-101, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, -100), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledAndDeadbandTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.SetpointActivePower = ActivePower.FromKilo(100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(100, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAfterEnabledAndDeadbandNegativeTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(-100);
            _mode!.Enable();
            _mode!.SetpointActivePower = ActivePower.FromKilo(-100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
            Assert.AreEqual(-100, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            Assert.AreEqual(0, _mode!.TargetReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointRampUpReachedTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(100);
            _mode!.SetpointActivePower = ActivePower.FromKilo(101);
            _mode!.Enable();
            _rampActive!.Setup(x => x.Calculate()).Returns(100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(101, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(101, _mode!.TargetActivePower.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 100), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledDisabledAvailableSetpointRampDownReachedTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(100);
            _mode!.SetpointActivePower = ActivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.Disable();
            _rampActive!.Setup(x => x.Calculate()).Returns(10);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointActivePower.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Exactly(2));
            _rampActive!.Verify(x => x.Start(0, 100), Times.Once);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), 0), Times.Once);
            _rampActive!.Verify(x => x.Stop(), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableUpperLimitTest()
        {
            _config!.MaximumActivePowerLimitKiloWatt = 500;

            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _mode!.Enable();
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 500), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableLowerLimitTest()
        {
            _config!.MinimumActivePowerLimitKiloWatt = -400;

            _map!.AvailableActivePower = () => ActivePower.FromKilo(-1000);
            _mode!.Enable();
            _mode!.SetpointActivePower = ActivePower.FromKilo(-1000);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointActivePower.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, -400), Times.Once);
        }



        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointAvailableGettingBiggerTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(100);
            _mode!.SetpointActivePower = ActivePower.FromKilo(100);
            _mode!.Enable();
            _mode!.CalculateAsync();
            _map!.AvailableActivePower = () => ActivePower.FromKilo(500);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(100, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.TargetActivePower.Watts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 100), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointErrorInDeadbandTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _config!.DeadbandErrorKilo = 100;
            _mode!.Enable();
            _rampActive!.Setup(x => x.Calculate()).Returns(1000);
            _mode!.CalculateAsync();
            _map!.ActivePowerAtPoi = () => ActivePower.FromKilo(950);
            _mode!.CalculateAsync();

            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(1000, _mode!.TargetActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.ErrorAdjustmentActive.Watts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 1000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointErrorOutsideDeadbandTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _config!.DeadbandErrorKilo = 20;
            _mode!.Enable();
            _rampActive!.Setup(x => x.Calculate()).Returns(1000);
            _mode!.CalculateAsync();
            _map!.ActivePowerAtPoi = () => ActivePower.FromKilo(950);
            _mode!.CalculateAsync();

            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(1050, _mode!.TargetActivePower.KiloWatts);
            Assert.AreEqual(50, _mode!.ErrorAdjustmentActive.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 1000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointErrorOutsideDeadbandCalculateMultipleTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _config!.DeadbandErrorKilo = 20;
            _mode!.Enable();
            _rampActive!.Setup(x => x.Calculate()).Returns(1000);
            _mode!.CalculateAsync();
            _map!.ActivePowerAtPoi = () => ActivePower.FromKilo(950);
            _mode!.CalculateAsync();
            _mode!.CalculateAsync();
            _mode!.CalculateAsync();
            _mode!.CalculateAsync();

            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(1050, _mode!.TargetActivePower.KiloWatts);
            Assert.AreEqual(50, _mode!.ErrorAdjustmentActive.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 1000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointErrorOutsideDeadbandCalculateMultipleChangePoiTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _config!.DeadbandErrorKilo = 20;
            _mode!.Enable();
            _rampActive!.Setup(x => x.Calculate()).Returns(1000);
            _mode!.CalculateAsync();
            _map!.ActivePowerAtPoi = () => ActivePower.FromKilo(950);
            _mode!.CalculateAsync();
            _mode!.CalculateAsync();
            _map!.ActivePowerAtPoi = () => ActivePower.FromKilo(951);
            _mode!.CalculateAsync();
            _mode!.CalculateAsync();

            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(1099, _mode!.TargetActivePower.KiloWatts);
            Assert.AreEqual(99, _mode!.ErrorAdjustmentActive.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 1000), Times.Once);
        }


        [TestMethod]
        public void CalculateModeEnabledAvailableSetpointRampGainTest()
        {
            _config!.ProportionalGain = 0.5;
            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _mode!.Enable();
            _rampActive!.Setup(x => x.Calculate()).Returns(800);
            _mode!.CalculateAsync();
            _map!.ActivePowerAtPoi = () => ActivePower.FromKilo(700);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(850, _mode!.TargetActivePower.KiloWatts);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 1000), Times.Once);
        }


    }
}
