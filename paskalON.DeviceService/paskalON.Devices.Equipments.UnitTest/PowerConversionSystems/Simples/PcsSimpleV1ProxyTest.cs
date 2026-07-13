// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Communication.Protocols.Modbus;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.PowerConversionSystems.Simples
{
    [TestClass]
    public class PcsSimpleV1ProxyTest
    {
        private Mock<DerBatteryStorageUnit>? _unit;
        private Mock<PowerConversionSystemConfig>? _pcsConfig;


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
            _pcsConfig = new Mock<PowerConversionSystemConfig>();
            _pcsConfig.SetupGet(x => x.Name).Returns("PowerConversionSystemConfig");
        }



        [TestMethod]
        public void CreatePcsWithNullClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreatePcsWithMockedClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_pcsConfig.Object.Name, pcs.Name);
        }


        [TestMethod]
        public async Task PcsStartTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            ushort? stateTarget = 1;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? state = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, CancellationToken>((adr, val, type, token) => { address = adr; state = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.StartAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.SelectorState, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            Assert.AreEqual(stateTarget, state);
            Assert.AreEqual(PcsState.Starting, pcs.State);
        }


        [TestMethod]
        public async Task PcsStopTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            ushort? stateTarget = 0;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? state = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, CancellationToken>((adr, val, type, token) => { address = adr; state = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.StopAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.SelectorState, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            Assert.AreEqual(stateTarget, state);
            Assert.AreEqual(PcsState.Stopping, pcs.State);
        }


        [TestMethod]
        public async Task PcsSetActivePowerTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            double? activePowerTarget = 11000;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? activePower = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, double, ModbusDataType, CancellationToken>((adr, val, type, token) => { address = adr; activePower = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.SetActivePowerTargetAsync(activePowerTarget);

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.PReference, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            // Active power is written in kilo watts
            Assert.AreEqual(activePowerTarget / 1000, activePower);
        }


        [TestMethod]
        public async Task PcsSetReactivePowerTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            double? reactivePowerTarget = 12000;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? reactivePower = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, double, ModbusDataType, CancellationToken>((adr, val, type, token) => { address = adr; reactivePower = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!.Object, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.SetReactivePowerTargetAsync(reactivePowerTarget);

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.QReference, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            // Reactive power is written in kilo vars
            Assert.AreEqual(reactivePowerTarget / 1000, reactivePower);
        }

    }
}
