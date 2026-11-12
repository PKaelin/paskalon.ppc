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
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.PowerConversionSystems
{
    /// <summary>
    /// Tests of the class <see cref="ActivePowerRangeCalculator"/>.
    /// </summary>
    [TestClass]
    public class ActivePowerRangeCalculatorTest
    {
        /// <summary>
        /// Nameplate maximum active power of the PCS in watts.
        /// </summary>
        private const double PcsNameplate = 3630000;


        /// <summary>
        /// Circuit of the DER units.
        /// </summary>
        private DerCircuit? _circuit;


        /// <summary>
        /// Battery storage unit of the battery banks.
        /// </summary>
        private DerBatteryStorageUnit? _unit;


        /// <summary>
        /// Creates the DER hierarchy with an empty battery storage unit.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<Der> der = new Mock<Der>(NullLogger.Instance, derConfig.Object);
            Mock<DerGroupConfig> groupConfig = new Mock<DerGroupConfig>();
            groupConfig.SetupGet(x => x.Name).Returns("DerGroupConfig");
            Mock<DerGroup> group = new Mock<DerGroup>(NullLogger.Instance, groupConfig.Object, der.Object);
            Mock<DerCircuitConfig> circuitConfig = new Mock<DerCircuitConfig>();
            circuitConfig.SetupGet(x => x.Name).Returns("DerCircuitConfig");
            _circuit = new Mock<DerCircuit>(NullLogger.Instance, circuitConfig.Object, group.Object).Object;
            Mock<DerBatteryStorageUnitConfig> unitConfig = new Mock<DerBatteryStorageUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerBatteryStorageUnitConfig");
            _unit = new DerBatteryStorageUnit(NullLogger.Instance, unitConfig.Object, _circuit);
        }


        /// <summary>
        /// Disposes the battery storage unit.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            _unit?.Dispose();
        }


        /// <summary>
        /// The DER unit is required.
        /// </summary>
        [TestMethod]
        public void ActivePowerRangeCalculatorCalculateNullUnitThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ActivePowerRangeCalculator.Calculate(1000000, PcsNameplate, null!));
        }


        /// <summary>
        /// The range is unknown while the PCS does not report its available active power.
        /// </summary>
        [TestMethod]
        public void ActivePowerRangeCalculatorCalculateUnknownAvailableReturnsNullTest()
        {
            _unit!.BatteryBanks.Add(CreateBank(50, 5000000, 5000000));

            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(null, PcsNameplate, _unit);

            Assert.IsNull(range);
        }


        /// <summary>
        /// The reported value limits the direction it describes, the other direction is limited by the nameplates,
        /// a battery bank at the usable minimum cannot discharge and at the usable maximum cannot charge.
        /// </summary>
        /// <param name="reportedAvailable">The available active power reported by the PCS in watts.</param>
        /// <param name="usableStateOfCharge">The usable state of charge of the battery bank in percent.</param>
        /// <param name="expectedMinimum">The expected minimum (charge) active power in watts.</param>
        /// <param name="expectedMaximum">The expected maximum (discharge) active power in watts.</param>
        [TestMethod]
        [DataRow(3000000, 50, -3630000, 3000000)]
        [DataRow(-2000000, 50, -2000000, 3630000)]
        [DataRow(0, 0, -3630000, 0)]
        [DataRow(-3000000, 0, -3000000, 0)]
        [DataRow(3000000, 100, 0, 3000000)]
        [DataRow(-3000000, 100, 0, 3630000)]
        public void ActivePowerRangeCalculatorCalculateBatteryTest(double reportedAvailable, double usableStateOfCharge, double expectedMinimum, double expectedMaximum)
        {
            _unit!.BatteryBanks.Add(CreateBank(usableStateOfCharge, 5000000, 5000000));

            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(reportedAvailable, PcsNameplate, _unit);

            Assert.IsNotNull(range);
            Assert.AreEqual(expectedMinimum, range.Value.Minimum);
            Assert.AreEqual(expectedMaximum, range.Value.Maximum);
        }


        /// <summary>
        /// The charge and discharge rates of the battery banks limit the range below the nameplate of the PCS.
        /// </summary>
        [TestMethod]
        public void ActivePowerRangeCalculatorCalculateBatteryLimitedByBankRatesTest()
        {
            _unit!.BatteryBanks.Add(CreateBank(50, 1000000, 1500000));

            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(3000000, PcsNameplate, _unit);

            Assert.IsNotNull(range);
            Assert.AreEqual(-1000000, range.Value.Minimum);
            Assert.AreEqual(1500000, range.Value.Maximum);
        }


        /// <summary>
        /// An empty battery bank does not add its discharge rate, the other battery banks can still discharge.
        /// </summary>
        [TestMethod]
        public void ActivePowerRangeCalculatorCalculateEmptyBankDoesNotDischargeTest()
        {
            _unit!.BatteryBanks.Add(CreateBank(0, 2000000, 2000000));
            _unit!.BatteryBanks.Add(CreateBank(50, 2000000, 2000000));

            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(3000000, PcsNameplate, _unit);

            Assert.IsNotNull(range);
            Assert.AreEqual(-3630000, range.Value.Minimum);
            Assert.AreEqual(2000000, range.Value.Maximum);
        }


        /// <summary>
        /// Battery banks that are not connected or do not report their state of charge are not available.
        /// </summary>
        /// <param name="state">The state of the battery bank.</param>
        /// <param name="hasStateOfCharge">Whether the battery bank reports its usable state of charge.</param>
        [TestMethod]
        [DataRow(BatteryBankState.Disconnected, true)]
        [DataRow(BatteryBankState.Fault, true)]
        [DataRow(BatteryBankState.Connected, false)]
        public void ActivePowerRangeCalculatorCalculateUnavailableBankReturnsZeroRangeTest(BatteryBankState state, bool hasStateOfCharge)
        {
            _unit!.BatteryBanks.Add(CreateBank(hasStateOfCharge ? 50 : null, 5000000, 5000000, state));

            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(3000000, PcsNameplate, _unit);

            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Value.Minimum);
            Assert.AreEqual(0, range.Value.Maximum);
        }


        /// <summary>
        /// A battery bank in standby is available.
        /// </summary>
        [TestMethod]
        public void ActivePowerRangeCalculatorCalculateStandbyBankIsAvailableTest()
        {
            _unit!.BatteryBanks.Add(CreateBank(50, 5000000, 5000000, BatteryBankState.Standby));

            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(3000000, PcsNameplate, _unit);

            Assert.IsNotNull(range);
            Assert.AreEqual(-3630000, range.Value.Minimum);
            Assert.AreEqual(3000000, range.Value.Maximum);
        }


        /// <summary>
        /// A producing unit (solar) cannot consume active power, its maximum is the reported available active power.
        /// </summary>
        [TestMethod]
        public void ActivePowerRangeCalculatorCalculateSolarTest()
        {
            Mock<DerSolarUnitConfig> unitConfig = new Mock<DerSolarUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerSolarUnitConfig");
            DerSolarUnit solarUnit = new DerSolarUnit(NullLogger.Instance, unitConfig.Object, _circuit!);

            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(2500000, PcsNameplate, solarUnit);

            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Value.Minimum);
            Assert.AreEqual(2500000, range.Value.Maximum);
        }


        /// <summary>
        /// A battery unit can discharge while a battery bank is above the usable minimum and charge while one is below the usable maximum.
        /// </summary>
        /// <param name="usableStateOfCharge">The usable state of charge of the battery bank in percent.</param>
        /// <param name="expectedCanDischarge">Whether the unit is expected to discharge.</param>
        /// <param name="expectedCanCharge">Whether the unit is expected to charge.</param>
        [TestMethod]
        [DataRow(0, false, true)]
        [DataRow(0.1, true, true)]
        [DataRow(50, true, true)]
        [DataRow(99.9, true, true)]
        [DataRow(100, true, false)]
        public void ActivePowerRangeCalculatorCanDischargeAndChargeTest(double usableStateOfCharge, bool expectedCanDischarge, bool expectedCanCharge)
        {
            _unit!.BatteryBanks.Add(CreateBank(usableStateOfCharge, 5000000, 5000000));

            bool canDischarge = ActivePowerRangeCalculator.CanDischarge(_unit);
            bool canCharge = ActivePowerRangeCalculator.CanCharge(_unit);

            Assert.AreEqual(expectedCanDischarge, canDischarge);
            Assert.AreEqual(expectedCanCharge, canCharge);
        }


        /// <summary>
        /// Without configured battery banks the state of charge is unknown to the PPC: the SOC limits do not apply
        /// and the range is limited by the reported value and the nameplate of the PCS only.
        /// </summary>
        /// <param name="reportedAvailable">The available active power reported by the PCS in watts.</param>
        /// <param name="expectedMinimum">The expected minimum (charge) active power in watts.</param>
        /// <param name="expectedMaximum">The expected maximum (discharge) active power in watts.</param>
        [TestMethod]
        [DataRow(3000000, -3630000, 3000000)]
        [DataRow(-2000000, -2000000, 3630000)]
        public void ActivePowerRangeCalculatorCalculateWithoutConfiguredBanksTest(double reportedAvailable, double expectedMinimum, double expectedMaximum)
        {
            (double Minimum, double Maximum)? range = ActivePowerRangeCalculator.Calculate(reportedAvailable, PcsNameplate, _unit!);
            bool canDischarge = ActivePowerRangeCalculator.CanDischarge(_unit!);
            bool canCharge = ActivePowerRangeCalculator.CanCharge(_unit!);

            Assert.IsNotNull(range);
            Assert.AreEqual(expectedMinimum, range.Value.Minimum);
            Assert.AreEqual(expectedMaximum, range.Value.Maximum);
            Assert.IsTrue(canDischarge);
            Assert.IsTrue(canCharge);
        }


        /// <summary>
        /// Configured but unavailable battery banks block charging and discharging (fail-safe).
        /// </summary>
        [TestMethod]
        public void ActivePowerRangeCalculatorCanDischargeAndChargeUnavailableBankBlocksTest()
        {
            _unit!.BatteryBanks.Add(CreateBank(null, 5000000, 5000000));

            bool canDischarge = ActivePowerRangeCalculator.CanDischarge(_unit);
            bool canCharge = ActivePowerRangeCalculator.CanCharge(_unit);

            Assert.IsFalse(canDischarge);
            Assert.IsFalse(canCharge);
        }


        /// <summary>
        /// Creates a battery bank with the usable state of charge and nameplate rates.
        /// </summary>
        /// <param name="usableStateOfCharge">The usable state of charge in percent, null if unknown.</param>
        /// <param name="maximumChargeRate">The nameplate maximum charge rate in watts.</param>
        /// <param name="maximumDischargeRate">The nameplate maximum discharge rate in watts.</param>
        /// <param name="state">The state of the battery bank.</param>
        /// <returns>The battery bank.</returns>
        private BatteryBankBase CreateBank(double? usableStateOfCharge, double maximumChargeRate, double maximumDischargeRate,
            BatteryBankState state = BatteryBankState.Connected)
        {
            BatteryBankDeviceConfig deviceConfig = new BatteryBankDeviceConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankDeviceConfig",
                ClassName = "Test",
                NameplateMaximumChargeRate = maximumChargeRate,
                NameplateMaximumDischargeRate = maximumDischargeRate
            };
            BatteryBankConfig config = new BatteryBankConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankConfig",
                IsActive = true,
                DeviceId = 1,
                BatteryBankDeviceConfig = deviceConfig,
                ModbusConfig = new Mock<ModbusConfig>().Object,
                DerUnitConfig = new Mock<DerUnitConfig>().Object
            };
            BatteryBank bank = new BatteryBank(NullLogger.Instance, config, _unit!, new Mock<IMetricsPublisher>().Object, new Mock<IModbusDataface>().Object);
            bank.State = state;
            bank.UsableStateOfCharge = usableStateOfCharge;

            return bank;
        }
    }
}
