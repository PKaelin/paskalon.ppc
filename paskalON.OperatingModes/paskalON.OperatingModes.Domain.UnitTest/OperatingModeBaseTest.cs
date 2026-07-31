// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.UnitTest
{
    /// <summary>
    /// Test class to test hidden members.
    /// </summary>
    internal class OperatingModeTest : OperatingModeBase
    {
        public OperatingModeTest(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeBaseConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, systemConfig, config, map, rampController, curveController)
        {
        }
        public ActivePower ActivePowerTarget { set => _targetActivePower = value; }
        public ReactivePower ReactivePowerTarget { set => _targetReactivePower = value; }
        public OperatingModeState OmState { set => State = value; }
        public ActivePower? LastAvailableActive { get => _lastAvailableActive; }
        public ActivePower? LastSetpointActive { get => _lastSetpointActive; }
        public ReactivePower? LastAvailableReactive { get => _lastAvailableReactive; }
        public ReactivePower? LastSetpointReactive { get => _lastSetpointReactive; }
        public double TestGetActiveSetpoint() { return GetActivePowerTargetSetpoint(); }
        public double TestGetReactiveSetpoint() { return GetReactivePowerTargetSetpoint(); }
    }


    [TestClass]
    public class OperatingModeBaseTest
    {
        private OperatingModeTest? _mode;
        private OperatingModeBaseMap? _map;
        private Mock<IRampController>? _rampActive;
        private Mock<IRampController>? _rampReactive;
        private SystemConfig? _systemConfig;


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

            Mock<OperatingModeBaseConfig> config = new Mock<OperatingModeBaseConfig>();
            _map = new OperatingModeBaseMap { AvailableActivePower = () => null, AvailableReactivePower = () => null };
            _rampActive = new Mock<IRampController>();
            _rampReactive = new Mock<IRampController>();
            _rampActive.Setup(x => x.ShallowCopy()).Returns(_rampReactive.Object);
            _mode = new OperatingModeTest(NullLogger.Instance, TimeProvider.System, _systemConfig, config.Object, _map, _rampActive.Object);
        }


        [TestMethod]
        public void CreateWithNullLoggerTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<OperatingModeBaseConfig> config = new Mock<OperatingModeBaseConfig>();
            Mock<OperatingModeBaseMap> map = new Mock<OperatingModeBaseMap>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new OperatingModeTest(null, TimeProvider.System, systemConfig.Object, config.Object, map.Object, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullSystemConfigTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<OperatingModeBaseConfig> config = new Mock<OperatingModeBaseConfig>();
            Mock<OperatingModeBaseMap> map = new Mock<OperatingModeBaseMap>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new OperatingModeTest(NullLogger.Instance, TimeProvider.System, null, config.Object, map.Object, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullModeConfigTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<OperatingModeBaseMap> map = new Mock<OperatingModeBaseMap>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new OperatingModeTest(NullLogger.Instance, TimeProvider.System, systemConfig.Object, null, map.Object, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullRampControllerTest()
        {
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<OperatingModeBaseConfig> config = new Mock<OperatingModeBaseConfig>();
            Mock<OperatingModeBaseMap> map = new Mock<OperatingModeBaseMap>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new OperatingModeTest(NullLogger.Instance, TimeProvider.System, systemConfig.Object, config.Object, map.Object, null, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullMapControllerTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<OperatingModeBaseConfig> config = new Mock<OperatingModeBaseConfig>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new OperatingModeTest(NullLogger.Instance, TimeProvider.System, systemConfig.Object, config.Object, null, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }



        [TestMethod]
        public void SetActivePowerSetpointBiggerThanNameplateTest()
        {
            _systemConfig!.NameplateMaximumActivePowerKiloWatt = 100;
            Assert.ThrowsExactly<InvalidOperationException>(() => _mode!.SetpointActivePower = ActivePower.FromKilo(200));
        }


        [TestMethod]
        public void SetActivePowerSetpointSmallerThanNameplateTest()
        {
            _systemConfig!.NameplateMinimumActivePowerKiloWatt = -100;
            Assert.ThrowsExactly<InvalidOperationException>(() => _mode!.SetpointActivePower = ActivePower.FromKilo(-200));
        }


        [TestMethod]
        public void SetReactivePowerSetpointBiggerThanNameplateTest()
        {
            _systemConfig!.NameplateMaximumReactivePowerKiloVars = 100;
            Assert.ThrowsExactly<InvalidOperationException>(() => _mode!.SetpointReactivePower = ReactivePower.FromKilo(200));
        }


        [TestMethod]
        public void SetReactivePowerSetpointSmallerThanNameplateTest()
        {
            _systemConfig!.NameplateMinimumReactivePowerKiloVars = -100;
            Assert.ThrowsExactly<InvalidOperationException>(() => _mode!.SetpointReactivePower = ReactivePower.FromKilo(-200));
        }


        [TestMethod]
        public void GetActiveSetpointNoAvailableNoSetpointTest()
        {
            double setpoint = _mode!.TestGetActiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNull(_mode!.LastAvailableActive);
            Assert.IsNull(_mode!.LastSetpointActive);
        }


        [TestMethod]
        public void GetActiveSetpointAvailableButNoSetpointTest()
        {
            _map!.AvailableActivePower = () => new ActivePower(10000);
            double setpoint = _mode!.TestGetActiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableActive);
            Assert.IsNotNull(_mode!.LastSetpointActive);
        }


        [TestMethod]
        public void GetActiveSetpointNoAvailableButSetpointTest()
        {
            _map!.AvailableActivePower = () => null;
            _mode!.SetpointActivePower = new ActivePower(10000);
            double setpoint = _mode!.TestGetActiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNull(_mode!.LastAvailableActive);
            Assert.IsNull(_mode!.LastSetpointActive);
        }


        [TestMethod]
        public void GetActiveSetpointZeroAvailableButSetpointTest()
        {
            _map!.AvailableActivePower = () => new ActivePower(0);
            _mode!.SetpointActivePower = new ActivePower(10000);
            double setpoint = _mode!.TestGetActiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableActive);
            Assert.IsNotNull(_mode!.LastSetpointActive);
        }


        [TestMethod]
        public void GetActiveSetpointSmallerAvailableBiggerSetpointTest()
        {
            _map!.AvailableActivePower = () => new ActivePower(10000);
            _mode!.SetpointActivePower = new ActivePower(20000);
            double setpoint = _mode!.TestGetActiveSetpoint();

            Assert.AreEqual(10, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableActive);
            Assert.IsNotNull(_mode!.LastSetpointActive);
        }


        [TestMethod]
        public void GetActiveSetpointBiggerAvailableSmallerSetpointTest()
        {
            _map!.AvailableActivePower = () => new ActivePower(100000);
            _mode!.SetpointActivePower = new ActivePower(50000);
            double setpoint = _mode!.TestGetActiveSetpoint();

            Assert.AreEqual(50, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableActive);
            Assert.IsNotNull(_mode!.LastSetpointActive);
        }


        [TestMethod]
        public void GetReactiveSetpointNoAvailableNoSetpointTest()
        {
            double setpoint = _mode!.TestGetReactiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNull(_mode!.LastAvailableReactive);
            Assert.IsNull(_mode!.LastSetpointReactive);
        }


        [TestMethod]
        public void GetReactiveSetpointAvailableButNoSetpointTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(10000);
            double setpoint = _mode!.TestGetReactiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableReactive);
            Assert.IsNotNull(_mode!.LastSetpointReactive);
        }


        [TestMethod]
        public void GetReactiveSetpointNoAvailableButSetpointTest()
        {
            _map!.AvailableReactivePower = () => null;
            _mode!.SetpointReactivePower = new ReactivePower(10000);
            double setpoint = _mode!.TestGetReactiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNull(_mode!.LastAvailableReactive);
            Assert.IsNull(_mode!.LastSetpointReactive);
        }


        [TestMethod]
        public void GetReactiveSetpointZeroAvailableButSetpointTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(0);
            _mode!.SetpointReactivePower = new ReactivePower(10000);
            double setpoint = _mode!.TestGetReactiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableReactive);
            Assert.IsNotNull(_mode!.LastSetpointReactive);
        }


        [TestMethod]
        public void GetReactiveSetpointSmallerAvailableBiggerSetpointTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(10000);
            _mode!.SetpointReactivePower = new ReactivePower(20000);
            double setpoint = _mode!.TestGetReactiveSetpoint();

            Assert.AreEqual(10, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableReactive);
            Assert.IsNotNull(_mode!.LastSetpointReactive);
        }


        [TestMethod]
        public void GetReactiveSetpointBiggerAvailableSmallerSetpointTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(100000);
            _mode!.SetpointReactivePower = new ReactivePower(50000);
            double setpoint = _mode!.TestGetReactiveSetpoint();

            Assert.AreEqual(50, setpoint);
            Assert.IsNotNull(_mode!.LastAvailableReactive);
            Assert.IsNotNull(_mode!.LastSetpointReactive);
        }


        [TestMethod]
        public void EnableDisableWhenCheckIsEnabledTest()
        {
            _mode!.Enable();

            Assert.IsTrue(_mode!.IsEnabled);

            _mode!.Disable();

            Assert.IsFalse(_mode!.IsEnabled);
        }


        [TestMethod]
        public void EnableWhenAlreadyEnabledTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(100000);
            _mode!.SetpointReactivePower = new ReactivePower(50000);
            _mode!.OmState = OperatingModeState.Enabled;
            _mode!.Enable();

            // As supposed to RampingToEnabled
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
        }


        [TestMethod]
        public void EnableWhenAvailable0Setpoint0Test()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(0);
            _mode!.SetpointReactivePower = new ReactivePower(0);
            _mode!.Enable();

            // Operating mode is enabling but not ramping yet.
            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
        }


        [TestMethod]
        public void EnableWhenAvailable10Setpoint10ActiveTest()
        {
            _map!.AvailableActivePower = () => new ActivePower(20000);
            _mode!.SetpointActivePower = new ActivePower(20000);
            _mode!.Enable();

            // Operating mode is enabling but not ramping yet.
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            _rampActive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampActive!.Verify(x => x.Start(0, 20), Times.Once);
        }


        [TestMethod]
        public void EnableWhenAvailable10Setpoint10ReactiveTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(10000);
            _mode!.SetpointReactivePower = new ReactivePower(10000);
            _mode!.Enable();

            // Operating mode is enabling but not ramping yet.
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            _rampReactive!.Verify(x => x.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            _rampReactive!.Verify(x => x.Start(0, 10), Times.Once);
        }



        [TestMethod]
        public void DisableWhenAlreadyDisabledTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(100000);
            _mode!.SetpointReactivePower = new ReactivePower(50000);
            _mode!.OmState = OperatingModeState.Disabled;
            _mode!.Disable();

            // As supposed to RampingToDisabled
            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
        }


        [TestMethod]
        public void DisableTest()
        {
            _map!.AvailableReactivePower = () => new ReactivePower(100000);
            _mode!.SetpointReactivePower = new ReactivePower(50000);
            _mode!.ActivePowerTarget = new ActivePower(20000);
            _mode!.ReactivePowerTarget = new ReactivePower(10000);

            _mode!.OmState = OperatingModeState.Enabled;
            _mode!.Disable();

            Assert.AreEqual(OperatingModeState.RampingToDisabled, _mode!.State);
            Assert.AreEqual(0, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(20, _mode!.TargetActivePower.KiloWatts);
            Assert.AreEqual(10, _mode!.TargetReactivePower.KiloVoltAmperesReactive);
        }

    }
}
