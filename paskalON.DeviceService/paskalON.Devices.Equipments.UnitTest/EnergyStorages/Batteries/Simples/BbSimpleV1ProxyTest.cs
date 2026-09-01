// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.EnergyStorages.Batteries.Simples
{
    [TestClass]
    public class BbSimpleV1ProxyTest
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
        public void CreateBatteryBankWithNullClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, null!));
        }


        [TestMethod]
        public void CreateBatteryBankWithNullDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, null!, client.Object));
        }


        [TestMethod]
        public void CreateBatteryBankWithMockedClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_bbConfig!.Object.Name, bb.Name);
        }


        [TestMethod]
        public void BatteryBankWithMockedClientComErrorTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            FakeLogger<BbSimpleV1Proxy> logger = new FakeLogger<BbSimpleV1Proxy>();

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(logger, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);
            EventArgs expectedEvent = new EventArgs();
            client.Raise(x => x.OnCommunicationError += null, this, expectedEvent);

            Assert.AreEqual(_bbConfig!.Object.Name, bb.Name);
            Assert.IsTrue(bb.CommunicationError);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsTrue(logs.First().Message.Contains("CommunicationError state", StringComparison.OrdinalIgnoreCase));
        }



        [TestMethod]
        public async Task BatteryBankConnectTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            ushort? stateTarget = 1;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? state = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, short, CancellationToken>((adr, val, type, priority, token) => { address = adr; state = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await bb.ConnectAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual((ushort)BbSimpleV1Description.Register.SelectorState, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            Assert.AreEqual(stateTarget, state);
            Assert.AreEqual(BatteryBankState.Connecting, bb.State);
        }


        [TestMethod]
        public async Task BatteryBankDisconnectTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            ushort? stateTarget = 0;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? state = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, short, CancellationToken>((adr, val, type, priority, token) => { address = adr; state = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await bb.DisconnectAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual((ushort)BbSimpleV1Description.Register.SelectorState, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            Assert.AreEqual(stateTarget, state);
            Assert.AreEqual(BatteryBankState.Disconnecting, bb.State);
        }
    }
}
