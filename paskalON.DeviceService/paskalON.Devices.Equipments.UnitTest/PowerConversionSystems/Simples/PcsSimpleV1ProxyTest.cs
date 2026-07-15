// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.Devices.Equipments.UnitTest.PowerConversionSystems.Simples
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
                StationId = 1,
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
        public void CreatePcsWithNullClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreatePcsWithMockedClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_pcsConfig!.Name, pcs.Name);
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

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

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

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

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

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

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

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.SetReactivePowerTargetAsync(reactivePowerTarget);

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.QReference, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            // Reactive power is written in kilo vars
            Assert.AreEqual(reactivePowerTarget / 1000, reactivePower);
        }


        [TestMethod]
        public async Task PcsPollTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            ModbusDataConverter converter = new ModbusDataConverter();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.ConvertRawData(It.IsAny<bool[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((bool[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });
            client.Setup(x => x.ConvertRawData(It.IsAny<ushort[]>(), It.IsAny<IModbusRegisterEntry>(), It.IsAny<ushort>()))
                .Returns((ushort[] data, IModbusRegisterEntry register, ushort start) => { return converter.ConvertRawData(data, register, start); });

            double p = 11;
            double q = 12;
            double frequency = 50.45;
            double dcCurrent = 13;
            double dcVoltage = 14;
            double acCurrent = 15;
            double acVoltage = 16;

            client
                .Setup(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.P, (ushort)PcsSimpleV1Description.Register.Q, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    List<ushort> list = new List<ushort>();
                    list.AddRange(converter.RegisterArrayFromValue(p, ModbusDataType.MbInt16, ModbusScale.NoScale));    // P
                    list.AddRange(converter.RegisterArrayFromValue(q, ModbusDataType.MbInt16, ModbusScale.NoScale));    // Q
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

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface, client.Object);

            // Poll interval is 1
            await pcs.PollAsync(1);

            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.P, (ushort)PcsSimpleV1Description.Register.Q, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.Frequency, (ushort)PcsSimpleV1Description.Register.ACVoltage, It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(x => x.ReadHoldingRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

            Assert.AreEqual(p, pcs.ActivePowerValue);
            Assert.AreEqual(q, pcs.ReactivePowerValue);
            Assert.AreEqual(frequency, pcs.Frequency);
            Assert.AreEqual(dcCurrent, pcs.DCCurrent);
            Assert.AreEqual(dcVoltage, pcs.DCVoltage);
            Assert.AreEqual(acCurrent, pcs.ACCurrent);
            Assert.AreEqual(acVoltage, pcs.ACVoltage);
        }


        // TODO: poll 2
        /*
         Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange((int)PcsSimpleV1Description.Register.CurrentState, (int)PcsSimpleV1Description.Register.DcContactor,
         ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass2));
        */
    }
}
