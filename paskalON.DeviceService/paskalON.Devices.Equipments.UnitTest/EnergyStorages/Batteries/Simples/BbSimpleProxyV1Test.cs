// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Communication.Protocols.Modbus;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.EnergyStorages.Batteries.Simples
{
    [TestClass]
    public class BbSimpleProxyV1Test
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
        public void CreateBatteryBankWithNullClient()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateBatteryBankWithMockedClient()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_bbConfig.Object.Name, bb.Name);
        }

    }
}
