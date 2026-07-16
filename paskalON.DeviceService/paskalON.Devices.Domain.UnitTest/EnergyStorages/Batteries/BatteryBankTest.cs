// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.PowerConversionSystems
{
    [TestClass]
    public class BatteryBankTest
    {
        private Mock<DerBatteryStorageUnit>? _unit;
        private Mock<BatteryBankConfig>? _bbConfig;


        [TestInitialize]
        public void TestInitialize()
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
            // Unit
            Mock<DerBatteryStorageUnitConfig> unitConfig = new Mock<DerBatteryStorageUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerBatteryStorageUnitConfig");
            _unit = new Mock<DerBatteryStorageUnit>(NullLogger.Instance, unitConfig.Object, circuit.Object);
            // Device
            _bbConfig = new Mock<BatteryBankConfig>();
            _bbConfig.SetupGet(x => x.Name).Returns("BatteryBankConfig");
        }


        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new BatteryBank(NullLogger.Instance, null, _unit!.Object, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new BatteryBank(NullLogger.Instance, _bbConfig!.Object, null, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            BatteryBank batteryBank = new BatteryBank(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface);

            Assert.IsNotNull(batteryBank.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(4, dataface.Registers);
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.StateOfCharge)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.StateOfHealth)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.TotalDCVoltage)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.TotalDCCurrent)));
        }


        [TestMethod]
        public async Task BatteryBankConnectTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            BatteryBank batteryBank = new BatteryBank(logger, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface);

            await batteryBank.ConnectAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(BatteryBankState.Connecting, batteryBank.State);
            Assert.HasCount(2, logs);
            Assert.IsTrue(logs.First().Message.Contains("connect requested", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(logs.Last().Message.Contains("state changed", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public async Task BatteryBankDisconnectTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            BatteryBank batteryBank = new BatteryBank(logger, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface);

            await batteryBank.DisconnectAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(BatteryBankState.Disconnecting, batteryBank.State);
            Assert.HasCount(2, logs);
            Assert.IsTrue(logs.First().Message.Contains("disconnect requested", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(logs.Last().Message.Contains("state changed", StringComparison.OrdinalIgnoreCase));
        }

    }
}
