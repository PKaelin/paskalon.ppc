// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.Devices.Domain.UnitTest.GenericModbusDevices
{
    [TestClass]
    public class GenericModbusDeviceTest
    {
        private DerConfig? _derConfig;
        private GenericModbusConfig? _genericModbusConfig;
        private ModbusConnectionConfig? _connectionConfig;
        private GenericModbusDeviceConfig? _deviceConfig;
        private GenericModbusMapConfig? _mapConfig;



        [TestInitialize]
        public void TestInitialize()
        {
            _derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };

            _connectionConfig = new ModbusConnectionConfig
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
                OperationTimeoutMilliseconds = 1003,
                SendRetryCount = 1,
                SendRetryIntervalMilliseconds = 4002,
                ServerToClientAliveIntervalSeconds = -1,
                ServerMaximumConnections = 5
            };

            _mapConfig = new GenericModbusMapConfig
            {
                ChangedBy = "Test",
                Name = "GenericModbusMapConfig",
            };

            _deviceConfig = new GenericModbusDeviceConfig
            {
                ChangedBy = "Test",
                Name = "GenericModbusDeviceConfig",
                ClassName = "ClassName",
                GenericModbusMapConfig = _mapConfig
            };

            _genericModbusConfig = new GenericModbusConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig",
                IsActive = true,
                DeviceId = 1,
                DerConfig = _derConfig,
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                UnitId = 1,
                ModbusConnectionConfig = _connectionConfig,
                GenericModbusDeviceConfig = _deviceConfig
            };
        }


        [TestMethod]
        public void GenericModbusNoMapDefinedTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            List<GenericModbusEntryBase> entries = new List<GenericModbusEntryBase>();

            GenericModbusDevice gmd = new GenericModbusDevice(NullLogger.Instance, _genericModbusConfig!, entries, publisher.Object, dataface);

            Assert.HasCount(0, dataface.Registers);
        }


        [TestMethod]
        public void GenericModbusDeviceCoilTest()
        {
            List<GenericModbusCoilPointConfig> coils = new List<GenericModbusCoilPointConfig>();
            GenericModbusCoilPointConfig coil1 = new GenericModbusCoilPointConfig
            {
                ChangedBy = "Test",
                GenericModbusMapConfig = _mapConfig!,
                IsAlarm = false,
                IsAlarmReset = false,
                ModbusDataType = ModbusDataType.MbBool,
                ModbusNumber = 1000,
                Name = "Coil1"
            };
            coils.Add(coil1);

            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            List<GenericModbusEntryBase> entries = new List<GenericModbusEntryBase>();

            foreach (GenericModbusCoilPointConfig config in coils)
            {
                entries.Add(new GenericModbusPointEntry(config));
            }

            GenericModbusDevice gmd = new GenericModbusDevice(NullLogger.Instance, _genericModbusConfig!, entries, publisher.Object, dataface);

            // Only registers non writeable registers.
            Assert.HasCount(0, dataface.Registers);
        }


        [TestMethod]
        public void GenericModbusDiscreteInputTest()
        {
            List<GenericModbusDiscreteInputPointConfig> discretes = new List<GenericModbusDiscreteInputPointConfig>();

            GenericModbusDiscreteInputPointConfig discrete1 = new GenericModbusDiscreteInputPointConfig
            {
                ChangedBy = "Test",
                GenericModbusMapConfig = _mapConfig!,
                IsAlarm = false,
                IsAlarmReset = false,
                ModbusDataType = ModbusDataType.MbBool,
                ModbusNumber = 1000,
                Name = "Discrete1"
            };

            discretes.Add(discrete1);

            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            List<GenericModbusEntryBase> entries = new List<GenericModbusEntryBase>();

            foreach (GenericModbusDiscreteInputPointConfig config in discretes)
            {
                entries.Add(new GenericModbusPointEntry(config));
            }

            GenericModbusDevice gmd = new GenericModbusDevice(NullLogger.Instance, _genericModbusConfig!, entries, publisher.Object, dataface);

            Assert.HasCount(1, dataface.Registers);
            IModbusRegisterEntry? entry = dataface.Registers.FirstOrDefault(r => r.Name == discrete1.Name);
            Assert.IsNotNull(entry);
            Assert.AreEqual(discrete1.ModbusNumber, entry.Register);
            Assert.AreEqual(discrete1.ModbusDataType, entry.DataType);
        }


        [TestMethod]
        public void GenericModbusInputRegisterTest()
        {
            List<GenericModbusInputRegisterConfig> inputs = new List<GenericModbusInputRegisterConfig>();

            GenericModbusInputRegisterConfig input1 = new GenericModbusInputRegisterConfig
            {
                ChangedBy = "Test",
                GenericModbusMapConfig = _mapConfig!,
                BitIndex = -1,
                IndividualOffset = 0,
                ModbusScale = ModbusScale.NoScale,
                ReverseSign = false,
                ModbusDataType = ModbusDataType.MbBool,
                ModbusNumber = 1000,
                Name = "Input1"
            };

            inputs.Add(input1);

            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            List<GenericModbusEntryBase> entries = new List<GenericModbusEntryBase>();

            foreach (GenericModbusInputRegisterConfig config in inputs)
            {
                entries.Add(new GenericModbusRegisterEntry(config));
            }

            GenericModbusDevice gmd = new GenericModbusDevice(NullLogger.Instance, _genericModbusConfig!, entries, publisher.Object, dataface);

            Assert.HasCount(1, dataface.Registers);
            IModbusRegisterEntry? entry = dataface.Registers.FirstOrDefault(r => r.Name == input1.Name);
            Assert.IsNotNull(entry);
            Assert.AreEqual(input1.ModbusNumber, entry.Register);
            Assert.AreEqual(input1.ModbusDataType, entry.DataType);
        }


        [TestMethod]
        public void GenericModbusHoldingRegisterTest()
        {
            List<GenericModbusHoldingRegisterConfig> holdings = new List<GenericModbusHoldingRegisterConfig>();
            GenericModbusHoldingRegisterConfig holding1 = new GenericModbusHoldingRegisterConfig
            {
                ChangedBy = "Test",
                GenericModbusMapConfig = _mapConfig!,
                BitIndex = -1,
                IndividualOffset = 0,
                ModbusScale = ModbusScale.NoScale,
                ReverseSign = false,
                ModbusDataType = ModbusDataType.MbBool,
                ModbusNumber = 1000,
                Name = "Holding1"
            };

            holdings.Add(holding1);

            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            List<GenericModbusEntryBase> entries = new List<GenericModbusEntryBase>();

            foreach (GenericModbusHoldingRegisterConfig config in holdings)
            {
                entries.Add(new GenericModbusRegisterEntry(config));
            }

            GenericModbusDevice gmd = new GenericModbusDevice(NullLogger.Instance, _genericModbusConfig!, entries, publisher.Object, dataface);

            // Only registers non writeable registers.
            Assert.HasCount(0, dataface.Registers);
        }


    }
}
