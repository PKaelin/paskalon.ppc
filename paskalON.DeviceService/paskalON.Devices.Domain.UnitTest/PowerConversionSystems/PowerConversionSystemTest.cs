// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.PhysicalUnits.Electricals.Powers;
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
            _deviceConfig = new PowerConversionSystemDeviceConfig { ChangedBy = "Test", Name = "PowerConversionSystemDeviceConfig", ClassName = "ClassName", StandbyActivePowerWatts = 65 };
            _pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                DeviceId = 1,
                Name = "PowerConversionSystemConfig",
                PowerConversionSystemDeviceConfig =
                _deviceConfig,
                ModbusConfig = new Mock<ModbusConfig>().Object,
                DerUnitConfig = unitConfig.Object,
            };
        }


        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Assert.ThrowsExactly<ArgumentNullException>(() => new Pcs(NullLogger.Instance, null!, _unit!.Object, publisher.Object, dataface.Object));
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Assert.ThrowsExactly<ArgumentNullException>(() => new Pcs(NullLogger.Instance, _pcsConfig!, null!, publisher.Object, dataface.Object));
        }


        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
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
            ModbusRegister dataface = new ModbusRegister("Test");
            Pcs pcs = new Pcs(logger, _pcsConfig!, _unit!.Object, publisher.Object, dataface);

            await pcs.StartAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PcsState.Starting, pcs.State);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("start requested", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("state changed", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task PcsStopTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            Pcs pcs = new Pcs(logger, _pcsConfig!, _unit!.Object, publisher.Object, dataface);

            await pcs.StopAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PcsState.Stopping, pcs.State);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("stop requested", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("state changed", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task PcsStandbyTest()
        {
            FakeLogger<BatteryBank> logger = new FakeLogger<BatteryBank>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            Pcs pcs = new Pcs(logger, _pcsConfig!, _unit!.Object, publisher.Object, dataface);

            await pcs.StandbyAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PcsState.EnteringStandby, pcs.State);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("standby requested", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains($"{_deviceConfig!.StandbyActivePowerWatts}", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("state changed", StringComparison.OrdinalIgnoreCase)));
        }


        /// <summary>
        /// The active power range is only known while the PCS is started and reports its available active power.
        /// A battery PCS without configured battery banks is limited by the reported value and its nameplate.
        /// </summary>
        /// <param name="state">The state of the PCS.</param>
        /// <param name="isRangeKnown">Whether the range is expected to be known.</param>
        [TestMethod]
        [DataRow(PcsState.Started, true)]
        [DataRow(PcsState.Stopped, false)]
        [DataRow(PcsState.Standby, false)]
        public void ActiveAvailablePowerRangeKnownWhenStartedTest(PcsState state, bool isRangeKnown)
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            _deviceConfig!.NameplateMaximumActivePower = 3630000;
            Pcs pcs = new Pcs(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface);
            pcs.State = state;
            pcs.ActiveAvailablePowerValue = 3000000;

            ActivePower? minimum = pcs.ActiveAvailablePowerMinimum;
            ActivePower? maximum = pcs.ActiveAvailablePowerMaximum;

            Assert.AreEqual(isRangeKnown, minimum.HasValue);
            Assert.AreEqual(isRangeKnown, maximum.HasValue);

            if (isRangeKnown)
            {
                Assert.AreEqual(-3630000, minimum!.Value.Watts);
                Assert.AreEqual(3000000, maximum!.Value.Watts);
            }
        }


        /// <summary>
        /// An active power target is limited to 0 if the battery cannot follow its direction:
        /// an empty battery cannot discharge but can charge, a full battery cannot charge but can discharge.
        /// </summary>
        /// <param name="usableStateOfCharge">The usable state of charge of the battery bank in percent.</param>
        /// <param name="target">The requested active power target in watts.</param>
        /// <param name="expectedTarget">The expected active power target in watts.</param>
        [TestMethod]
        [DataRow(0, 1000000, 0)]
        [DataRow(0, -1000000, -1000000)]
        [DataRow(100, -1000000, 0)]
        [DataRow(100, 1000000, 1000000)]
        [DataRow(50, 1000000, 1000000)]
        [DataRow(50, -1000000, -1000000)]
        public async Task SetActivePowerTargetLimitedByStateOfChargeTest(double usableStateOfCharge, double target, double expectedTarget)
        {
            Pcs pcs = CreateStartedPcs(NullLogger.Instance, usableStateOfCharge, out BatteryBank _);

            await pcs.SetActivePowerTargetAsync(target);

            Assert.AreEqual(expectedTarget, pcs.ActivePowerTarget!.Value.Watts);
        }


        /// <summary>
        /// The SOC limit is logged once when it becomes active and once when it is released, not with every control cycle.
        /// </summary>
        [TestMethod]
        public async Task SetActivePowerTargetStateOfChargeLimitLoggedOnceTest()
        {
            FakeLogger<Pcs> logger = new FakeLogger<Pcs>();
            Pcs pcs = CreateStartedPcs(logger, 0, out BatteryBank bank);

            await pcs.SetActivePowerTargetAsync(1000000);
            await pcs.SetActivePowerTargetAsync(1200000);
            bank.UsableStateOfCharge = 10;
            await pcs.SetActivePowerTargetAsync(1000000);

            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            Assert.AreEqual(1, logs.Count(log => log.Level == LogLevel.Warning && log.Message.Contains("SOC limit active", StringComparison.Ordinal)));
            Assert.AreEqual(1, logs.Count(log => log.Level == LogLevel.Information && log.Message.Contains("SOC limit released", StringComparison.Ordinal)));
            Assert.AreEqual(1000000, pcs.ActivePowerTarget!.Value.Watts);
        }


        /// <summary>
        /// The safety check sets the target to 0 when the battery runs empty while discharging or full while charging
        /// (same usable window limits in both directions) and keeps the reactive power target.
        /// </summary>
        /// <param name="target">The active power target in watts (positive discharge, negative charge).</param>
        /// <param name="limitStateOfCharge">The usable state of charge the battery reaches in percent (0 empty, 100 full).</param>
        [TestMethod]
        [DataRow(1000000, 0)]
        [DataRow(-1000000, 100)]
        public async Task CheckSafetyStateOfChargeLimitStopsTargetAndKeepsReactiveTest(double target, double limitStateOfCharge)
        {
            Pcs pcs = CreateStartedPcs(NullLogger.Instance, 50, out BatteryBank bank);
            await pcs.SetActivePowerTargetAsync(target);
            await pcs.SetReactivePowerTargetAsync(500000);
            pcs.ActivePowerTargetSet = new TaskCompletionSource<double?>(TaskCreationOptions.RunContinuationsAsynchronously);
            bank.UsableStateOfCharge = limitStateOfCharge;

            // The active power update runs the safety check, which sets the target in the background
            pcs.ActivePowerValue = target;
            double? enforcedTarget = await pcs.ActivePowerTargetSet.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.AreEqual(0, enforcedTarget);
            Assert.AreEqual(0, pcs.ActivePowerTarget!.Value.Watts);
            Assert.AreEqual(500000, pcs.ReactivePowerTarget!.Value.VoltAmperesReactive);
        }


        /// <summary>
        /// Creates a started PCS with one connected battery bank.
        /// </summary>
        /// <param name="logger">The logger of the PCS.</param>
        /// <param name="usableStateOfCharge">The usable state of charge of the battery bank in percent.</param>
        /// <param name="bank">The battery bank of the PCS.</param>
        /// <returns>The started PCS.</returns>
        private Pcs CreateStartedPcs(ILogger logger, double usableStateOfCharge, out BatteryBank bank)
        {
            BatteryBankConfig bankConfig = new BatteryBankConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankConfig",
                IsActive = true,
                DeviceId = 2,
                BatteryBankDeviceConfig = new BatteryBankDeviceConfig
                {
                    ChangedBy = "Test",
                    Name = "BatteryBankDeviceConfig",
                    ClassName = "Test",
                    NameplateMaximumChargeRate = 5000000,
                    NameplateMaximumDischargeRate = 5000000
                },
                ModbusConfig = new Mock<ModbusConfig>().Object,
                DerUnitConfig = new Mock<DerUnitConfig>().Object
            };
            bank = new BatteryBank(NullLogger.Instance, bankConfig, _unit!.Object, new Mock<IMetricsPublisher>().Object, new Mock<IModbusDataface>().Object);
            bank.State = BatteryBankState.Connected;
            bank.UsableStateOfCharge = usableStateOfCharge;
            _unit!.Object.BatteryBanks.Add(bank);
            Pcs pcs = new Pcs(logger, _pcsConfig!, _unit!.Object, new Mock<IMetricsPublisher>().Object, new ModbusRegister("Test"));
            pcs.State = PcsState.Started;

            return pcs;
        }
    }
}
