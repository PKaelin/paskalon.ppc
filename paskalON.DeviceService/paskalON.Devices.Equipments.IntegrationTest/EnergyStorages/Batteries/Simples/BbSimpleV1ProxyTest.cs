// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
using paskalON.Devices.Equipments.Modbus;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.Devices.Equipments.IntegrationTest.EnergyStorages.Batteries.Simples
{
    [TestClass]
    public class BbSimpleV1ProxyTest
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
                UnitId = 1,
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
        public async Task BbPoll1Test()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
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

            ModbusPollingEngine engine = new ModbusPollingEngine(NullLogger.Instance, client.Object, dataface);
            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, publisher.Object, dataface, client.Object);

            // Poll interval is 1
            await engine.PollAsync(1, CancellationToken.None);

            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)BbSimpleV1Description.Register.TotalStateOfCharge, (ushort)BbSimpleV1Description.Register.TotalDCCurrent, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            Assert.AreEqual(totalStateOfCharge, bb.StateOfCharge);
            Assert.AreEqual(totalStateOfHealth, bb.StateOfHealth);
            Assert.AreEqual(totalDCVoltage, bb.TotalDCVoltage);
            Assert.AreEqual(totalDCCurrent, bb.TotalDCCurrent);
        }


        [TestMethod]
        public async Task BbPoll3Test()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            ModbusDataConverter converter = new ModbusDataConverter();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.ConvertRawData(It.IsAny<bool[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((bool[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });
            client.Setup(x => x.ConvertRawData(It.IsAny<ushort[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((ushort[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });

            double currentState = (int)BbSimpleV1Description.State.Connected;
            double currentWarning = (int)BbSimpleV1Description.WarningCode.CellExtremeVoltage;
            double currentFault = (int)BbSimpleV1Description.FaultCode.CellExtremeTemperature;
            double currentVendorEvent = (int)BbSimpleV1Description.VendorEvents.MaintenanceDue;

            client
                .Setup(x => x.ReadHoldingRegistersAsync((ushort)BbSimpleV1Description.Register.CurrentState, (ushort)BbSimpleV1Description.Register.CurrentVendorEvent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    List<ushort> list = new List<ushort>();
                    list.AddRange(converter.RegisterArrayFromValue(currentState, ModbusDataType.MbInt16, ModbusScale.NoScale));         // CurrentState
                    list.AddRange(converter.RegisterArrayFromValue(currentWarning, ModbusDataType.MbInt16, ModbusScale.NoScale));       // CurrentWarning
                    list.AddRange(converter.RegisterArrayFromValue(currentFault, ModbusDataType.MbInt16, ModbusScale.NoScale));         // CurrentFault
                    list.AddRange(converter.RegisterArrayFromValue(currentVendorEvent, ModbusDataType.MbInt16, ModbusScale.NoScale));   // CurrentVendorEvent
                    return list.ToArray();
                });

            ModbusPollingEngine engine = new ModbusPollingEngine(NullLogger.Instance, client.Object, dataface);
            BbSimpleV1Proxy bb = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, publisher.Object, dataface, client.Object);

            // Poll interval is 3
            await engine.PollAsync(3, CancellationToken.None);

            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)BbSimpleV1Description.Register.CurrentState, (ushort)BbSimpleV1Description.Register.CurrentVendorEvent, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

            Assert.AreEqual(BatteryBankState.Connected, bb.State);
            // Warnings
            Assert.IsTrue(bb.HasActiveWarnings);
            Assert.HasCount(1, bb.WarningStates);
            Assert.AreEqual(BbSimpleV1Description.WarningCode.CellExtremeVoltage.ToString(), bb.WarningStates.First().Key);
            Assert.IsTrue(bb.WarningStates.First().Value);
            // Faults
            Assert.IsTrue(bb.HasActiveFaults);
            Assert.HasCount(1, bb.FaultStates);
            Assert.AreEqual(BbSimpleV1Description.FaultCode.CellExtremeTemperature.ToString(), bb.FaultStates.First().Key);
            Assert.IsTrue(bb.FaultStates.First().Value);
            // Vendor events
            Assert.IsTrue(bb.HasVendorEvents);
            Assert.HasCount(1, bb.VendorEvents);
            Assert.AreEqual(BbSimpleV1Description.VendorEvents.MaintenanceDue.ToString(), bb.VendorEvents.First().Key);
            Assert.IsTrue(bb.VendorEvents.First().Value);
        }


    }
}
