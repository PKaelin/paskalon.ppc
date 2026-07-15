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
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.Devices.Equipments.UnitTest.EnergyStorages.Batteries.Simples
{
    [TestClass]
    public class BbSimpleProxyV1Test
    {
        private Mock<DerBatteryStorageUnit>? _unit;
        private BatteryBankConfig? _bbConfig;

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
            Mock<BatteryBankDeviceConfig> deviceConfig = new Mock<BatteryBankDeviceConfig>();
            deviceConfig.SetupGet(x => x.Name).Returns("BatteryBankDeviceConfig");

            ModbusConnectionConfig modbusConnection = new ModbusConnectionConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConnectionConfig",
            };

            ModbusConfig modbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartContainer,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };

            _bbConfig = new BatteryBankConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankConfig",
                IsActive = true,
                DeviceId = 1,
                BatteryBankDeviceConfig = deviceConfig.Object,
                ModbusConfig = modbusConfig,
                DerUnitConfig = unitConfig.Object,
            };

        }


        [TestMethod]
        public void CreateBatteryBankWithNullClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, publisher.Object, dataface.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateBatteryBankWithMockedClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_bbConfig!.Name, bb.Name);
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
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, CancellationToken>((adr, val, type, token) => { address = adr; state = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await bb.ConnectAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>()), Times.Once);
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
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, CancellationToken>((adr, val, type, token) => { address = adr; state = val; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await bb.DisconnectAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>()), Times.Once);
            Assert.AreEqual((ushort)BbSimpleV1Description.Register.SelectorState, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            Assert.AreEqual(stateTarget, state);
            Assert.AreEqual(BatteryBankState.Disconnecting, bb.State);
        }


        [TestMethod]
        public async Task BbPollTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            ModbusDataConverter converter = new ModbusDataConverter();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.ConvertRawData(It.IsAny<bool[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((bool[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });
            client.Setup(x => x.ConvertRawData(It.IsAny<ushort[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((ushort[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });

            double totalStateOfCharge = 11;
            double totalStateOfHealth = 12;
            double totalDCVoltage = 13;
            double totalDCCurrent = 14;

            client
                .Setup(x => x.ReadHoldingRegistersAsync((ushort)BbSimpleV1Description.Register.TotalStateOfCharge, (ushort)BbSimpleV1Description.Register.TotalDCCurrent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    List<ushort> list = new List<ushort>();
                    list.AddRange(converter.RegisterArrayFromValue(totalStateOfCharge, ModbusDataType.MbUint16, ModbusScale.Upscale100));   // TotalStateOfCharge
                    list.AddRange(converter.RegisterArrayFromValue(totalStateOfHealth, ModbusDataType.MbUint16, ModbusScale.Upscale100));   // TotalStateOfHealth
                    list.AddRange(converter.RegisterArrayFromValue(totalDCVoltage, ModbusDataType.MbInt16, ModbusScale.NoScale));           // TotalDCVoltage
                    list.AddRange(converter.RegisterArrayFromValue(totalDCCurrent, ModbusDataType.MbInt16, ModbusScale.NoScale));           // TotalDCCurrent
                    return list.ToArray();
                });

            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, publisher.Object, dataface, client.Object);

            // Poll interval is 1
            await bb.PollAsync(1);

            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)BbSimpleV1Description.Register.TotalStateOfCharge, (ushort)BbSimpleV1Description.Register.TotalDCCurrent, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            Assert.AreEqual(totalStateOfCharge, bb.StateOfCharge);
            Assert.AreEqual(totalStateOfHealth, bb.StateOfHealth);
            Assert.AreEqual(totalDCVoltage, bb.TotalDCVoltage);
            Assert.AreEqual(totalDCCurrent, bb.TotalDCCurrent);
        }
    }
}
