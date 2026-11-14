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
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.PowerConversionSystems
{
    [TestClass]
    public class BatteryBankTest
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
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Assert.ThrowsExactly<ArgumentNullException>(() => new BatteryBank(NullLogger.Instance, null!, _unit!.Object, publisher.Object, dataface.Object));
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Assert.ThrowsExactly<ArgumentNullException>(() => new BatteryBank(NullLogger.Instance, _bbConfig!.Object, null!, publisher.Object, dataface.Object));
        }


        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            BatteryBank batteryBank = new BatteryBank(NullLogger.Instance, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface);

            Assert.IsNotNull(batteryBank.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(4, dataface.Registers);
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.StateOfCharge)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.StateOfHealth)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.TotalDCVoltage)));
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(batteryBank.TotalDCCurrent)));
        }


        [TestMethod]
        public async Task BatteryBankConnectTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            BatteryBank batteryBank = new BatteryBank(logger, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface);

            await batteryBank.ConnectAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(BatteryBankState.Connecting, batteryBank.State);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("connect requested", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("state changed", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task BatteryBankDisconnectTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            BatteryBank batteryBank = new BatteryBank(logger, _bbConfig!.Object, _unit!.Object, publisher.Object, dataface);

            await batteryBank.DisconnectAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(BatteryBankState.Disconnecting, batteryBank.State);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("disconnect requested", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("state changed", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task UpdateStateOfSiblingBatteryBanksAtTheSameTimeDoesNotDeadlockTest()
        {
            // Both battery banks of one unit report their state at the same time. The barrier holds both inside their state change
            // event until the other one arrived as well, then the unit handler reads the state of the sibling. The state change event
            // must not be raised inside the data lock, otherwise both battery banks wait for the lock of each other forever.
            DerBatteryStorageUnit unit = CreateBatteryStorageUnit();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<BatteryBankConfig> firstConfig = new Mock<BatteryBankConfig>();
            firstConfig.SetupGet(x => x.Name).Returns("BB 44.1");
            Mock<BatteryBankConfig> secondConfig = new Mock<BatteryBankConfig>();
            secondConfig.SetupGet(x => x.Name).Returns("BB 44.2");
            BatteryBank first = new BatteryBank(NullLogger.Instance, firstConfig.Object, unit, publisher.Object, new ModbusRegister("BB 44.1"));
            BatteryBank second = new BatteryBank(NullLogger.Instance, secondConfig.Object, unit, publisher.Object, new ModbusRegister("BB 44.2"));
            using Barrier barrier = new Barrier(2);
            // Subscribed before the battery banks are added to the unit, so it runs before the handler of the unit
            first.StateChanged += (_, _) => barrier.SignalAndWait(TimeSpan.FromSeconds(5));
            second.StateChanged += (_, _) => barrier.SignalAndWait(TimeSpan.FromSeconds(5));
            unit.BatteryBanks.Add(first);
            unit.BatteryBanks.Add(second);
            Task firstReport = Task.Factory.StartNew(() => first.ReportState(BatteryBankState.Disconnected), CancellationToken.None,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task secondReport = Task.Factory.StartNew(() => second.ReportState(BatteryBankState.Disconnected), CancellationToken.None,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);

            Task reports = Task.WhenAll(firstReport, secondReport);
            Task completed = await Task.WhenAny(reports, Task.Delay(TimeSpan.FromSeconds(10)));

            Assert.AreSame(reports, completed, "The state updates of the sibling battery banks deadlocked");
            await reports;
            Assert.AreEqual(BatteryBankState.Disconnected, first.State);
            Assert.AreEqual(BatteryBankState.Disconnected, second.State);
        }


        /// <summary>
        /// Creates a battery storage unit that handles the state changes of its battery banks.
        /// </summary>
        /// <returns>The battery storage unit.</returns>
        private static DerBatteryStorageUnit CreateBatteryStorageUnit()
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<Der> der = new Mock<Der>(NullLogger.Instance, derConfig.Object);
            Mock<DerGroupConfig> groupConfig = new Mock<DerGroupConfig>();
            groupConfig.SetupGet(x => x.Name).Returns("DerGroupConfig");
            Mock<DerGroup> group = new Mock<DerGroup>(NullLogger.Instance, groupConfig.Object, der.Object);
            Mock<DerCircuitConfig> circuitConfig = new Mock<DerCircuitConfig>();
            circuitConfig.SetupGet(x => x.Name).Returns("DerCircuitConfig");
            DerCircuit circuit = new DerCircuit(NullLogger.Instance, circuitConfig.Object, group.Object);
            DerBatteryStorageUnitConfig unitConfig = new DerBatteryStorageUnitConfig { ChangedBy = "Test", Name = "Bess 44", DerCircuitConfig = circuitConfig.Object };

            return new DerBatteryStorageUnit(NullLogger.Instance, unitConfig, circuit);
        }

    }
}
