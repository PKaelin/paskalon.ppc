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

        public ActivePower? LastAvailableActive { get => _lastAvailableActive; }
        public ActivePower? LastSetpointActive { get => _lastSetpointActive; }
        public ReactivePower? LastAvailableReactive { get => _lastAvailableReactive; }
        public ReactivePower? LastSetpointReactive { get => _lastSetpointReactive; }
        public double TestGetActiveSetpoint() { return GetActiveSetpoint(); }
        public double TestGetReactiveSetpoint() { return GetReactiveSetpoint(); }
    }


    [TestClass]
    public class OperatingModeBaseTest
    {
        private OperatingModeTest? _test;
        private OperatingModeBaseMap? _map;


        [TestInitialize]
        public void Initialize()
        {
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
            Mock<OperatingModeBaseConfig> config = new Mock<OperatingModeBaseConfig>();
            _map = new OperatingModeBaseMap { AvailableActivePower = () => null, AvailableReactivePower = () => null };
            Mock<IRampController> ramp = new Mock<IRampController>();
            _test = new OperatingModeTest(NullLogger.Instance, TimeProvider.System, systemConfig.Object, config.Object, _map, ramp.Object);
        }


        [TestMethod]
        public void GetActiveSetpointNoAvailableNoSetpointTest()
        {
            double setpoint = _test!.TestGetActiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNull(_test!.LastAvailableActive);
            Assert.IsNull(_test!.LastSetpointActive);
        }


        [TestMethod]
        public void GetActiveSetpointAvailableButNoSetpointTest()
        {
            _map!.AvailableActivePower = () => new ActivePower(10);
            double setpoint = _test!.TestGetActiveSetpoint();

            Assert.AreEqual(0, setpoint);
            Assert.IsNotNull(_test!.LastAvailableActive);
            Assert.IsNotNull(_test!.LastSetpointActive);
        }


    }
}
