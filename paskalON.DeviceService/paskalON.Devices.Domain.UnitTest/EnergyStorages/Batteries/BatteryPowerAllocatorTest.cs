// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.EnergyStorages.Batteries
{
    [TestClass]
    public sealed class BatteryPowerAllocatorTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private Mock<IModbusDataface> _dataface = new Mock<IModbusDataface>();
        private DerBatteryStorageUnit? _unit;


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
            _unit = new DerBatteryStorageUnit(NullLogger.Instance, unitConfig.Object, circuit.Object);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateNullUnitThrowsTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();

            Assert.ThrowsExactly<ArgumentNullException>(() => allocator.Allocate(null!));
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateNoBatteryBanksDoesNothingTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            DerBatteryStorageUnit unit = CreateUnit(5000000, []);

            allocator.Allocate(unit);

            Assert.AreEqual(0, unit.BatteryBanks.Count);
        }


        [TestMethod]
        [DataRow(5000000, 30, 5000000)]
        [DataRow(5000000, 50, 5000000)]
        [DataRow(5000000, 80, 5000000)]
        [DataRow(5000000, 0, 0)]
        [DataRow(5000000, 1, 5000000)]
        [DataRow(5000000, -5, 0)]
        [DataRow(5000000, 105, 5000000)]
        [DataRow(7000000, 50, 5000000)]
        public void BatteryPowerAllocatorAllocateSingleBankDischargeTest(double totalPower, double stateOfCharge, double expectedPower)
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank = CreateBank(stateOfCharge, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(totalPower, [bank]);

            allocator.Allocate(unit);

            Assert.AreEqual(expectedPower, bank.AllocatedActivePowerValue);
        }


        [TestMethod]
        [DataRow(-5000000, 30, -5000000)]
        [DataRow(-5000000, 50, -5000000)]
        [DataRow(-2000000, 80, -2000000)]
        [DataRow(-5000000, 0, 0)]
        [DataRow(-5000000, 1, -5000000)]
        [DataRow(-5000000, -5, 0)]
        [DataRow(-5000000, 105, -5000000)]
        [DataRow(-7000000, 50, -5000000)]
        public void BatteryPowerAllocatorAllocateSingleBankChargeTest(double totalPower, double stateOfCharge, double expectedPower)
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank = CreateBank(stateOfCharge, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(totalPower, [bank]);

            allocator.Allocate(unit);

            Assert.AreEqual(expectedPower, bank.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateDistributesPowerProportionallyToSocTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank1 = CreateBank(20, 5000000, 5000000);
            BatteryBankBase bank2 = CreateBank(80, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(5000000, [bank1, bank2]);

            allocator.Allocate(unit);

            Assert.AreEqual(1000000, bank1.AllocatedActivePowerValue);
            Assert.AreEqual(4000000, bank2.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateDistributesPowerProportionallyToSocWhileChargingTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank1 = CreateBank(20, 5000000, 5000000);
            BatteryBankBase bank2 = CreateBank(80, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(-5000000, [bank1, bank2]);

            allocator.Allocate(unit);

            Assert.AreEqual(-1000000, bank1.AllocatedActivePowerValue);
            Assert.AreEqual(-4000000, bank2.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateLimitsDischargeToNameplateRateTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank1 = CreateBank(90, 5000000, 2000000);
            BatteryBankBase bank2 = CreateBank(10, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(10000000, [bank1, bank2]);

            allocator.Allocate(unit);

            Assert.AreEqual(2000000, bank1.AllocatedActivePowerValue);
            Assert.AreEqual(1000000, bank2.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateLimitsChargeToNameplateRateTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank1 = CreateBank(90, 2000000, 5000000);
            BatteryBankBase bank2 = CreateBank(10, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(-10000000, [bank1, bank2]);

            allocator.Allocate(unit);

            Assert.AreEqual(-2000000, bank1.AllocatedActivePowerValue);
            Assert.AreEqual(-1000000, bank2.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateZeroPowerSetsAvailableBanksToZeroTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();

            BatteryBankBase bank1 = CreateBank(20, 5000000, 5000000);
            BatteryBankBase bank2 = CreateBank(80, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(0, [bank1, bank2]);

            allocator.Allocate(unit);

            Assert.AreEqual(0, bank1.AllocatedActivePowerValue);
            Assert.AreEqual(0, bank2.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateIgnoresBatteryInMaintenanceTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase maintenanceBank1 = CreateBank(50, 5000000, 5000000);
            BatteryBankBase maintenanceBank2 = CreateBank(50, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(5000000, [maintenanceBank1, maintenanceBank2], isInMaintenanceMode: true);

            allocator.Allocate(unit);

            Assert.AreEqual(0, maintenanceBank1.AllocatedActivePowerValue);
            Assert.AreEqual(0, maintenanceBank2.AllocatedActivePowerValue);

        }


        [TestMethod]
        [DataRow(BatteryBankState.Connected)]
        [DataRow(BatteryBankState.Standby)]
        public void BatteryPowerAllocatorAllocateIncludesConnectedAndStandbyBanksTest(BatteryBankState state)
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();

            BatteryBankBase bank = CreateBank(50, 5000000, 5000000, state: state);
            DerBatteryStorageUnit unit = CreateUnit(5000000, [bank]);

            allocator.Allocate(unit);

            Assert.AreEqual(5000000, bank.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateIgnoresDisconnectedBankTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank = CreateBank(50, 5000000, 5000000, state: BatteryBankState.Disconnected);
            DerBatteryStorageUnit unit = CreateUnit(5000000, [bank]);

            allocator.Allocate(unit);

            Assert.AreEqual(0, bank.AllocatedActivePowerValue);
        }



        [TestMethod]
        public void BatteryPowerAllocatorAllocateBankWithNullSocTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank = CreateBank(null, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(5000000, [bank]);

            allocator.Allocate(unit);

            Assert.AreEqual(0, bank.AllocatedActivePowerValue);
        }


        [TestMethod]
        public void BatteryPowerAllocatorAllocateDoesNotExceedTotalPowerForUncappedBanksTest()
        {
            BatteryPowerAllocator allocator = new BatteryPowerAllocator();
            BatteryBankBase bank1 = CreateBank(25, 5000000, 5000000);
            BatteryBankBase bank2 = CreateBank(75, 5000000, 5000000);
            DerBatteryStorageUnit unit = CreateUnit(4000000, [bank1, bank2]);

            allocator.Allocate(unit);

            double? allocatedPower = bank1.AllocatedActivePowerValue + bank2.AllocatedActivePowerValue;

            Assert.AreEqual(4000000, allocatedPower);
        }




        class BbTest : BatteryBankBase
        {
            public BbTest(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit batteryStorageUnit, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, batteryStorageUnit, publisher, dataface)
            {
            }

            public override Task HeartbeatAsync(CancellationToken cancellationToken)
            {
                // Not required for unit tests.
                return Task.CompletedTask;
            }

            protected override void RegisterDataface()
            {
            }
        }


        class PcsTest : PowerConversionSystemBase
        {
            public PcsTest(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, derUnit, publisher, dataface)
            {
            }

            protected override void RegisterDataface()
            {
            }

            public override Task HeartbeatAsync(CancellationToken cancellationToken)
            {
                // Not required for unit tests.
                return Task.CompletedTask;
            }
        }



        private DerBatteryStorageUnit CreateUnit(double totalPower, List<BatteryBankBase> banks, bool isInMaintenanceMode = false)
        {
            banks.ForEach(b => _unit!.BatteryBanks.Add(b));
            Mock<PowerConversionSystemConfig> pcsConfig = new Mock<PowerConversionSystemConfig>();
            pcsConfig.SetupGet(x => x.Name).Returns("PowerConversionSystemConfig");
            _unit!.PowerConversionSystem = new PcsTest(NullLogger.Instance, pcsConfig!.Object, _unit!, _publisher.Object, _dataface.Object);
            _unit!.PowerConversionSystem!.ActivePowerValue = totalPower;
            _unit!.IsInMaintenanceMode = isInMaintenanceMode;

            return _unit;
        }


        private BatteryBankBase CreateBank(double? stateOfCharge, double maximumChargeRate, double maximumDischargeRate,
            BatteryBankState state = BatteryBankState.Connected)
        {
            BatteryBankDeviceConfig bbDeviceConfig = new BatteryBankDeviceConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankDeviceConfig",
                ClassName = "Test",
                NameplateMaximumChargeRate = maximumChargeRate,
                NameplateMaximumDischargeRate = maximumDischargeRate
            };

            BatteryBankConfig bbConfig = new BatteryBankConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankConfig",
                IsActive = true,
                DeviceId = 1,
                BatteryBankDeviceConfig = bbDeviceConfig,
                ModbusConfig = new Mock<ModbusConfig>().Object,
                DerUnitConfig = new Mock<DerUnitConfig>().Object,
            };

            BbTest bb = new BbTest(NullLogger.Instance, bbConfig, _unit!, _publisher.Object, _dataface.Object);
            bb.State = state;
            bb.StateOfCharge = stateOfCharge;

            return bb;
        }
    }
}
