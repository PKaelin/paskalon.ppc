// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.PowerConversionSystems
{
    [TestClass]
    public class PowerConversionSystemTest
    {
        private Mock<DerBatteryStorageUnit>? _unit;
        private PowerConversionSystemConfig? _pcsConfig;
        private PowerConversionSystemDeviceConfig? _deviceConfig;


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
            _deviceConfig = new PowerConversionSystemDeviceConfig { ChangedBy = "Test", Name = "PowerConversionSystemDeviceConfig", ClassName = "ClassName", StandbyActivePowerKiloWatts = 65 };
            _pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                DeviceId = 1,
                Name = "PowerConversionSystemConfig",
                PowerConversionSystemDeviceConfig =
                _deviceConfig,
                ModbusConfig = new Mock<ModbusConfig>().Object,
            };
        }


        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new Pcs(NullLogger.Instance, null, _unit!.Object, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new Pcs(NullLogger.Instance, _pcsConfig!, null, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            Pcs pcs = new Pcs(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface);

            Assert.IsNotNull(pcs.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(4, dataface.Registers);
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(pcs.ActivePower)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(pcs.ReactivePower)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(pcs.DCCurrent)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(pcs.DCVoltage)));
        }



        [TestMethod]
        public async Task PcsStartTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            Pcs pcs = new Pcs(logger, _pcsConfig!, _unit!.Object, publisher.Object, dataface);

            await pcs.StartAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PcsState.Starting, pcs.State);
            Assert.HasCount(2, logs);
            Assert.IsTrue(logs.First().Message.Contains("start requested", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(logs.Last().Message.Contains("state changed", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public async Task PcsStopTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            Pcs pcs = new Pcs(logger, _pcsConfig!, _unit!.Object, publisher.Object, dataface);

            await pcs.StopAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PcsState.Stopping, pcs.State);
            Assert.HasCount(2, logs);
            Assert.IsTrue(logs.First().Message.Contains("stop requested", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(logs.Last().Message.Contains("state changed", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public async Task PcsStandbyTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            Pcs pcs = new Pcs(logger, _pcsConfig!, _unit!.Object, publisher.Object, dataface);

            await pcs.StandbyAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PcsState.EnteringStandby, pcs.State);
            Assert.HasCount(2, logs);
            Assert.IsTrue(logs.First().Message.Contains("standby requested", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(logs.First().Message.Contains($"{_deviceConfig!.StandbyActivePowerKiloWatts}", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(logs.Last().Message.Contains("state changed", StringComparison.OrdinalIgnoreCase));
        }
    }
}
