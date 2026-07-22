// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.IntegrationTest.OpenModes.FrequencyActives
{
    [TestClass]
    public class ActivePowerFixedModeTest
    {
        [TestMethod]
        public void CreateWithNullLoggerTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<ActivePowerFixedModeConfig> config = new Mock<ActivePowerFixedModeConfig>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new ActivePowerFixedMode(null, TimeProvider.System, systemConfig.Object, config.Object, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullSystemConfigTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<ActivePowerFixedModeConfig> config = new Mock<ActivePowerFixedModeConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, null, config.Object, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullModeConfigTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, systemConfig.Object, null, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullRampControllerTest()
        {
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<ActivePowerFixedModeConfig> config = new Mock<ActivePowerFixedModeConfig>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, systemConfig.Object, config.Object, null, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateOperatingModeTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
            Mock<ActivePowerFixedModeConfig> config = new Mock<ActivePowerFixedModeConfig>();
            config.SetupGet(x => x.Name).Returns("ActivePowerFixedModeConfig");

            ActivePowerFixedMode mode = new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, systemConfig.Object, config.Object, ramp.Object, curve.Object);

            Assert.IsNotNull(mode.RampController);
            Assert.IsNotNull(mode.CurveController);
            Assert.IsFalse(mode.IsEnabled);
            Assert.AreEqual(config.Object.Name, mode.Name);
            Assert.AreEqual(OperatingModeState.Disabled, mode.State);
            Assert.AreEqual(0, mode.Setpoint.ActivePower.Watts);
            Assert.AreEqual(0, mode.Setpoint.ReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, mode.Target.ActivePower.Watts);
            Assert.AreEqual(0, mode.Target.ReactivePower.VoltAmperesReactive);
        }


        [TestMethod]
        public void CallCalculateAsyncWithSimpleObjectTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
            Mock<ActivePowerFixedModeConfig> config = new Mock<ActivePowerFixedModeConfig>();
            ActivePowerFixedMode mode = new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, systemConfig.Object, config.Object, ramp.Object, curve.Object);

            Assert.ThrowsExactly<ArgumentException>(() => _ = mode.CalculateAsync(new object()));
        }

    }
}
