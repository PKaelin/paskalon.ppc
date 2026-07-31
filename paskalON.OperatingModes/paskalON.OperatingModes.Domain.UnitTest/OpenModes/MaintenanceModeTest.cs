// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.Ramps;

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
        private SystemConfig? _systemConfig;
        private MaintenanceModeConfig? _config;


        [TestInitialize]
        public void Initialize()
        {
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
                Type = OperatingModeType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerKiloWatt = double.MinValue,
                NameplateMaximumActivePowerKiloWatt = double.MaxValue,
                NameplateMinimumReactivePowerKiloVars = double.MinValue,
                NameplateMaximumReactivePowerKiloVars = double.MaxValue,
            };

            _config = new MaintenanceModeConfig
            {
                ChangedBy = "Test",
                Name = "MaintenanceModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = new Mock<RampBaseConfig>().Object
            };

            _map = new MaintenanceModeMap { AvailableActivePower = () => null, AvailableReactivePower = () => null, DerUnit = () => derUnit.Object };
            _rampActive = new Mock<IRampController>();
            _rampActive.Setup(x => x.ShallowCopy()).Returns(new Mock<IRampController>().Object);
            _mode = new MaintenanceMode(NullLogger.Instance, TimeProvider.System, _systemConfig, _config, derUnit.Object, _map, _rampActive.Object);
        }


        [TestMethod]
        public void CreateWithNullDerUnitTest()
        {
            Mock<IRampController> ramp = new Mock<IRampController>();
            Mock<ICurveController> curve = new Mock<ICurveController>();
            Mock<MaintenanceModeConfig> config = new Mock<MaintenanceModeConfig>();
            Mock<MaintenanceModeMap> map = new Mock<MaintenanceModeMap>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new MaintenanceMode(NullLogger.Instance, TimeProvider.System, systemConfig.Object, config.Object, null, map.Object, ramp.Object, curve.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}
