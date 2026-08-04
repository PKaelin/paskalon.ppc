// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.OpenModes.FrequencyActives
{
    [TestClass]
    public class ActivePowerFixedModeTest
    {
        private ActivePowerFixedMode? _mode;
        private ActivePowerFixedModeMap? _map;
        private Mock<IRampController>? _rampActive;
        private SystemConfig? _systemConfig;
        private ActivePowerFixedModeConfig? _config;


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

            _config = new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = new Mock<RampBaseConfig>().Object
            };

            _map = new ActivePowerFixedModeMap { AvailableActivePower = () => null, AvailableReactivePower = () => null };
            _rampActive = new Mock<IRampController>();
            _rampActive.Setup(x => x.ShallowCopy()).Returns(new Mock<IRampController>().Object);
            _mode = new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, publisher.Object, _systemConfig, _config, _map, _rampActive.Object);
        }



        [TestMethod]
        public void PowerFixedModePositiveTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(1000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.TargetActivePower.KiloWatts);

            _rampActive!.Setup(x => x.Calculate()).Returns(10);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(10, _mode!.TargetActivePower.KiloWatts);

            _rampActive!.Setup(x => x.Calculate()).Returns(100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(100, _mode!.TargetActivePower.KiloWatts);

            _rampActive!.Setup(x => x.Calculate()).Returns(950);
            _mode!.CalculateAsync();

            // Target is within the deadband so enabled and target = setpoint
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(1000, _mode!.TargetActivePower.KiloWatts);
        }


        [TestMethod]
        public void PowerFixedModeNegativeTest()
        {
            _map!.AvailableActivePower = () => ActivePower.FromKilo(-1000);
            _mode!.SetpointActivePower = ActivePower.FromKilo(-1000);
            _mode!.Enable();
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(0, _mode!.TargetActivePower.KiloWatts);

            _rampActive!.Setup(x => x.Calculate()).Returns(-10);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(-10, _mode!.TargetActivePower.KiloWatts);

            _rampActive!.Setup(x => x.Calculate()).Returns(-100);
            _mode!.CalculateAsync();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(-100, _mode!.TargetActivePower.KiloWatts);

            _rampActive!.Setup(x => x.Calculate()).Returns(-950);
            _mode!.CalculateAsync();

            // Target is within the deadband so enabled and target = setpoint
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(-1000, _mode!.SetpointActivePower.KiloWatts);
            Assert.AreEqual(-1000, _mode!.TargetActivePower.KiloWatts);
        }
    }
}
