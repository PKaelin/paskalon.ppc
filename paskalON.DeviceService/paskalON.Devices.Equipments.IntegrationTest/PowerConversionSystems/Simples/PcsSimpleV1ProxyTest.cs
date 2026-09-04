// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.Devices.Equipments.IntegrationTest.PowerConversionSystems.Simples
{
    [TestClass]
    public class PcsSimpleV1ProxyTest
    {
        private Mock<DerBatteryStorageUnit>? _unit;
        private PowerConversionSystemConfig? _pcsConfig;


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
            Mock<PowerConversionSystemDeviceConfig> deviceConfig = new Mock<PowerConversionSystemDeviceConfig>();
            deviceConfig.SetupGet(x => x.Name).Returns("PowerConversionSystemDeviceConfig");

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

            // Device
            _pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                DeviceId = 1,
                Name = "PowerConversionSystemConfig",
                PowerConversionSystemDeviceConfig = deviceConfig.Object,
                ModbusConfig = modbusConfig,
                DerUnitConfig = unitConfig.Object,
            };
        }


        [TestMethod]
        public async Task PcsPoll1Test()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            ModbusDataConverter converter = new ModbusDataConverter();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.State).Returns(ModbusClientState.Connected);
            client.Setup(x => x.ConvertRawData(It.IsAny<bool[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((bool[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });
            client.Setup(x => x.ConvertRawData(It.IsAny<ushort[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((ushort[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });

            double p = 11;
            double q = 12;
            double pAvailable = 21;
            double qAvailable = 22;
            double frequency = 50.45;
            double dcCurrent = 13;
            double dcVoltage = 14;
            double acCurrent = 15;
            double acVoltage = 16;

            client
                .Setup(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.P, (ushort)PcsSimpleV1Description.Register.QAvailable, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    List<ushort> list = new List<ushort>();
                    list.AddRange(converter.RegisterArrayFromValue(p, ModbusDataType.MbInt16, ModbusScale.NoScale));            // P
                    list.AddRange(converter.RegisterArrayFromValue(q, ModbusDataType.MbInt16, ModbusScale.NoScale));            // Q
                    list.AddRange(converter.RegisterArrayFromValue(pAvailable, ModbusDataType.MbInt16, ModbusScale.NoScale));   // P Available
                    list.AddRange(converter.RegisterArrayFromValue(qAvailable, ModbusDataType.MbInt16, ModbusScale.NoScale));   // Q Available
                    return list.ToArray();
                });

            client
                .Setup(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.Frequency, (ushort)PcsSimpleV1Description.Register.ACVoltage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    List<ushort> list = new List<ushort>();
                    list.AddRange(converter.RegisterArrayFromValue(frequency, ModbusDataType.MbInt16, ModbusScale.Upscale100));     // Frequency
                    list.AddRange(converter.RegisterArrayFromValue(dcCurrent, ModbusDataType.MbInt16, ModbusScale.NoScale));        // DCCurrent
                    list.AddRange(converter.RegisterArrayFromValue(dcVoltage, ModbusDataType.MbInt16, ModbusScale.NoScale));        // DCVoltage
                    list.AddRange(converter.RegisterArrayFromValue(acCurrent, ModbusDataType.MbInt16, ModbusScale.NoScale));        // ACCurrent
                    list.AddRange(converter.RegisterArrayFromValue(acVoltage, ModbusDataType.MbInt16, ModbusScale.NoScale));        // ACVoltage
                    return list.ToArray();
                });

            ModbusPollingEngine engine = new ModbusPollingEngine(NullLogger.Instance, client.Object, dataface);
            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface, client.Object);
            // Simulated already started
            pcs.State = PcsState.Started;

            // Poll interval is 1
            await engine.PollAsync(1, CancellationToken.None);

            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.P, (ushort)PcsSimpleV1Description.Register.QAvailable, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.Frequency, (ushort)PcsSimpleV1Description.Register.ACVoltage, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

            Assert.AreEqual(p, pcs.ActivePowerValue);
            Assert.AreEqual(q, pcs.ReactivePowerValue);
            Assert.AreEqual(pAvailable, pcs.ActiveAvailablePowerValue);
            Assert.AreEqual(qAvailable, pcs.ReactiveAvailablePowerValue);
            Assert.AreEqual(frequency, pcs.Frequency);
            Assert.AreEqual(dcCurrent, pcs.DCCurrent);
            Assert.AreEqual(dcVoltage, pcs.DCVoltage);
            Assert.AreEqual(acCurrent, pcs.ACCurrent);
            Assert.AreEqual(acVoltage, pcs.ACVoltage);
        }


        [TestMethod]
        public async Task PcsPoll3Test()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            ModbusDataConverter converter = new ModbusDataConverter();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.State).Returns(ModbusClientState.Connected);
            client.Setup(x => x.ConvertRawData(It.IsAny<bool[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((bool[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });
            client.Setup(x => x.ConvertRawData(It.IsAny<ushort[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((ushort[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });

            double currentState = (int)PcsSimpleV1Description.State.Stop;
            double currentWarning = (int)PcsSimpleV1Description.WarningCode.LowFrequency;
            double currentFault = (int)PcsSimpleV1Description.FaultCode.LowInputVoltage;
            double currentVendorEvent = (int)PcsSimpleV1Description.VendorEvents.MaintenanceDue;
            double aCBreaker = 1;
            double dcContactor = 1;

            client
                .Setup(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.CurrentState, (ushort)PcsSimpleV1Description.Register.DcContactor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    List<ushort> list = new List<ushort>();
                    list.AddRange(converter.RegisterArrayFromValue(currentState, ModbusDataType.MbInt16, ModbusScale.NoScale));         // CurrentState
                    list.AddRange(converter.RegisterArrayFromValue(currentWarning, ModbusDataType.MbInt16, ModbusScale.NoScale));       // CurrentWarning
                    list.AddRange(converter.RegisterArrayFromValue(currentFault, ModbusDataType.MbInt16, ModbusScale.NoScale));         // CurrentFault
                    list.AddRange(converter.RegisterArrayFromValue(currentVendorEvent, ModbusDataType.MbInt16, ModbusScale.NoScale));   // CurrentVendorEvent
                    list.AddRange(converter.RegisterArrayFromValue(aCBreaker, ModbusDataType.MbInt16, ModbusScale.NoScale));            // ACBreaker
                    list.AddRange(converter.RegisterArrayFromValue(dcContactor, ModbusDataType.MbInt16, ModbusScale.NoScale));          // DcContactor
                    return list.ToArray();
                });

            ModbusPollingEngine engine = new ModbusPollingEngine(NullLogger.Instance, client.Object, dataface);
            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface, client.Object);

            // Poll interval is 3
            await engine.PollAsync(3, CancellationToken.None);

            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.CurrentState, (ushort)PcsSimpleV1Description.Register.DcContactor, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

            Assert.AreEqual(PcsState.Stopped, pcs.State);
            // Warnings
            Assert.IsTrue(pcs.HasActiveWarnings);
            Assert.HasCount(1, pcs.WarningStates);
            Assert.AreEqual(PcsSimpleV1Description.WarningCode.LowFrequency.ToString(), pcs.WarningStates.First().Key);
            Assert.IsTrue(pcs.WarningStates.First().Value);
            // Faults
            Assert.IsTrue(pcs.HasActiveFaults);
            Assert.HasCount(1, pcs.FaultStates);
            Assert.AreEqual(PcsSimpleV1Description.FaultCode.LowInputVoltage.ToString(), pcs.FaultStates.First().Key);
            Assert.IsTrue(pcs.FaultStates.First().Value);
            // Vendor events
            Assert.IsTrue(pcs.HasVendorEvents);
            Assert.HasCount(1, pcs.VendorEvents);
            Assert.AreEqual(PcsSimpleV1Description.VendorEvents.MaintenanceDue.ToString(), pcs.VendorEvents.First().Key);
            Assert.IsTrue(pcs.VendorEvents.First().Value);
            // Breakers contactors            
            Assert.IsTrue(pcs.IsACBreakerClosed);
            Assert.IsNotNull(pcs.IsDcContactorClosed);
            Assert.HasCount(1, pcs.IsDcContactorClosed);
            Assert.IsTrue(pcs.IsDcContactorClosed.First());
        }
    }
}
