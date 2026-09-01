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

namespace paskalON.OperatingModes.Domain.IntegrationTest.OpenModes.VoltageReactives
{
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
        public void PowerFixedModePositiveTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(1000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(1000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetReactivePower.KiloVoltAmperesReactive);

            _rampReactive!.Setup(x => x.Calculate()).Returns(10);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(10, _mode!.TargetReactivePower.KiloVoltAmperesReactive);

            _rampReactive!.Setup(x => x.Calculate()).Returns(100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(100, _mode!.TargetReactivePower.KiloVoltAmperesReactive);

            _rampReactive!.Setup(x => x.Calculate()).Returns(950);
            _mode!.CalculateAsync();

            // Target is within the deadband so enabled and target = setpoint
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(1000, _mode!.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void PowerFixedModeNegativeTest()
        {
            _map!.AvailableReactivePower = () => ReactivePower.FromKilo(-1000);
            _mode!.SetpointReactivePower = ReactivePower.FromKilo(-1000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, _mode!.TargetReactivePower.KiloVoltAmperesReactive);

            _rampReactive!.Setup(x => x.Calculate()).Returns(-10);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(-10, _mode!.TargetReactivePower.KiloVoltAmperesReactive);

            _rampReactive!.Setup(x => x.Calculate()).Returns(-100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(-100, _mode!.TargetReactivePower.KiloVoltAmperesReactive);

            _rampReactive!.Setup(x => x.Calculate()).Returns(-950);
            _mode!.CalculateAsync();

            // Target is within the deadband so enabled and target = setpoint
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(-1000, _mode!.TargetReactivePower.KiloVoltAmperesReactive);
        }
    }
}
