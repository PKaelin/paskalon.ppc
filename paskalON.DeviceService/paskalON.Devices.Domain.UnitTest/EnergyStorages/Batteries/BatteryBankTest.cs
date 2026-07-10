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
using System.Net.Sockets;

namespace paskalON.Devices.Domain.UnitTest.PowerConversionSystems
{
    [TestClass]
    public class BatteryBankTest
    {
        private DerConfig? _derConfig;
        private Der? _der;
        private DerGroupConfig? _groupConfig;
        private DerGroup? _group;
        private DerCircuitConfig? _circuitConfig;
        private DerCircuit? _circuit;
        private DerBatteryStorageUnitConfig? _unitConfig;
        private DerBatteryStorageUnit? _unit;
        private ModbusConnectionConfig? _modbusConnectionConfig;
        private ModbusConfig? _modbusConfig;
        private BatteryBankDeviceConfig? _bbDeviceConfig;
        private BatteryBankConfig? _bbConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            _derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            _der = new Der(NullLogger.Instance, _derConfig);
            _groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = _derConfig };
            _group = new DerGroup(NullLogger.Instance, _groupConfig, _der);
            _circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = _groupConfig };
            _circuit = new DerCircuit(NullLogger.Instance, _circuitConfig, _group);
            _unitConfig = new DerBatteryStorageUnitConfig { ChangedBy = "Test", Name = "DerUnitConfig", DerCircuitConfig = _circuitConfig };
            _unit = new DerBatteryStorageUnit(NullLogger.Instance, _unitConfig, _circuit);

            _modbusConnectionConfig = new ModbusConnectionConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConnectionConfig",
                PollingIntervalMilliseconds = 1001,
                MasterHeartBeatIntervalMilliseconds = 900,
                IsPipeliningEnabled = false,
                ConnectionTimeoutMilliseconds = 1001,
                DisconnectionTimeoutMilliseconds = 1002,
                ConnectRetryCount = 2,
                ConnectRetryIntervalMilliseconds = 4001,
                SendTimeoutMilliseconds = 1003,
                SendRetryCount = 1,
                SendRetryIntervalMilliseconds = 4002,
                ServerToClientAliveIntervalSeconds = -1,
                ServerMaximumConnections = 5
            };

            _modbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = _modbusConnectionConfig
            };

            _bbDeviceConfig = new BatteryBankDeviceConfig { ChangedBy = "Test", Name = "BatteryBankDeviceConfig", ClassName = "ClassName" };

            _bbConfig = new BatteryBankConfig
            {
                DeviceId = 1,
                IsActive = true,
                ChangedBy = "Test",
                Name = "BatteryBankConfig",
                DerUnitConfig = _unitConfig,
                ModbusConfig = _modbusConfig,
                BatteryBankDeviceConfig = _bbDeviceConfig
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
