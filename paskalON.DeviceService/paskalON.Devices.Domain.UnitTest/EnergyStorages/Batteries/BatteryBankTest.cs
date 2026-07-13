// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.PowerConversionSystems
{
    [TestClass]
    public class BatteryBankTest
    {
        private DerBatteryStorageUnit? _unit;
        private BatteryBankConfig? _bbConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            Der der = new Der(NullLogger.Instance, derConfig);
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
            DerGroup group = new DerGroup(NullLogger.Instance, groupConfig, der);
            DerCircuitConfig circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = groupConfig };
            DerCircuit circuit = new DerCircuit(NullLogger.Instance, circuitConfig, group);
            DerBatteryStorageUnitConfig unitConfig = new DerBatteryStorageUnitConfig { ChangedBy = "Test", Name = "DerUnitConfig", DerCircuitConfig = circuitConfig };
            _unit = new DerBatteryStorageUnit(NullLogger.Instance, unitConfig, circuit);
            Mock<ModbusConfig> modbusConfig = new Mock<ModbusConfig>();
            modbusConfig.SetupGet(x => x.Name).Returns("ModbusConfig");
            Mock<BatteryBankDeviceConfig> bbDeviceConfig = new Mock<BatteryBankDeviceConfig>();
            bbDeviceConfig.SetupGet(x => x.Name).Returns("BatteryBankDeviceConfig");

            _bbConfig = new BatteryBankConfig
            {
                DeviceId = 1,
                IsActive = true,
                ChangedBy = "Test",
                Name = "BatteryBankConfig",
                DerUnitConfig = unitConfig,
                ModbusConfig = modbusConfig.Object,
                BatteryBankDeviceConfig = bbDeviceConfig.Object
            };
        }


        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new BatteryBank(NullLogger.Instance, null, _unit!, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new BatteryBank(NullLogger.Instance, _bbConfig!, null, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            BatteryBank batteryBank = new BatteryBank(NullLogger.Instance, _bbConfig!, _unit!, publisher.Object, dataface);

            Assert.IsNotNull(batteryBank.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(4, dataface.Registers);
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.StateOfCharge)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.StateOfHealth)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.TotalDCVoltage)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.TotalDCCurrent)));
        }

    }
}
