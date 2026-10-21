// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.UnitTest.PowerConversionSystems.Simples
{
    [TestClass]
    public sealed class PcsSimpleV1SimulationTest
    {
        [TestMethod]
        public void PcsSimpleV1SimulationNullStoreThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Simulation(null!, CreatePcsSettings(), new SimulatedGrid()));
        }


        [TestMethod]
        public void PcsSimpleV1SimulationNullGridThrowsTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());

            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Simulation(store, CreatePcsSettings(), null!));
        }


        [TestMethod]
        public async Task TickAsyncStartsStoppedWithoutPowerTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            pcs.AttachBatteryBanks(new[] { await CreateConnectedBankAsync(11, 5000000, 2500000) });
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, 1000);

            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.Off, pcs.State);
            Assert.AreEqual((short)PcsSimpleV1Description.State.Off, ReadRegister(store, PcsSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.P));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.ACBreaker));
            Assert.AreEqual(1, ReadRegister(store, PcsSimpleV1Description.Register.DcContactor));
            Assert.AreEqual(6000, (ushort)ReadRegister(store, PcsSimpleV1Description.Register.Frequency));
        }


        [TestMethod]
        [DataRow(1000, 2500000d, 1000, DisplayName = "Follows the active power setpoint")]
        [DataRow(3000, 2500000d, 2000, DisplayName = "Limited by the PCS nameplate")]
        [DataRow(1800, 1500000d, 1500, DisplayName = "Limited by the battery bank rate")]
        [DataRow(-1200, 2500000d, -1200, DisplayName = "Charges with a negative setpoint")]
        public async Task TickAsyncOnFollowsLimitedSetpointTest(int setpointKw, double bankRate, int expectedKw)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            BbSimpleV1Simulation bank = await CreateConnectedBankAsync(11, 5000000, bankRate);
            pcs.AttachBatteryBanks(new[] { bank });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, (short)setpointKw);

            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.On, pcs.State);
            Assert.AreEqual((short)PcsSimpleV1Description.State.On, ReadRegister(store, PcsSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(expectedKw, ReadRegister(store, PcsSimpleV1Description.Register.P));
            Assert.AreEqual(expectedKw * 1000d, bank.ActivePower, 0.001);
            Assert.AreEqual(1, ReadRegister(store, PcsSimpleV1Description.Register.ACBreaker));
        }


        [TestMethod]
        [DataRow(1000, 2000, DisplayName = "Positive discharge capability while discharging")]
        [DataRow(0, 2000, DisplayName = "Positive discharge capability while idle")]
        [DataRow(-1000, -1500, DisplayName = "Negative charge capability while charging")]
        public async Task TickAsyncActiveAvailablePowerFollowsPowerDirectionTest(int setpointKw, int expectedAvailableKw)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            SimulationModbusDataStore bankStore = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BatteryBankSimulationSettings bankSettings = CreateBankSettings(11, 5000000, 2500000) with { NameplateMaximumChargeRate = 1500000 };
            BbSimpleV1Simulation bank = new BbSimpleV1Simulation(bankStore, bankSettings);
            bankStore.HoldingRegisters.WritePoints((ushort)BbSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)BbSimpleV1Description.State.Connected });
            await bank.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            pcs.AttachBatteryBanks(new[] { bank });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, (short)setpointKw);

            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(expectedAvailableKw, ReadRegister(store, PcsSimpleV1Description.Register.PAvailable));
        }


        [TestMethod]
        public async Task TickAsyncWithoutConnectedBankStartsWithoutAvailablePowerTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            SimulationModbusDataStore bankStore = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            pcs.AttachBatteryBanks(new[] { new BbSimpleV1Simulation(bankStore, CreateBankSettings(11, 5000000, 2500000)) });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, 1000);
            WriteCommand(store, PcsSimpleV1Description.Register.QReference, 500);

            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.On, pcs.State);
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.P));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.Q));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.PAvailable));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.QAvailable));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.DcContactor));
        }


        [TestMethod]
        public async Task TickAsyncReactivePowerLimitedByApparentPowerTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            pcs.AttachBatteryBanks(new[] { await CreateConnectedBankAsync(11, 5000000, 2500000) });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, 2000);
            WriteCommand(store, PcsSimpleV1Description.Register.QReference, 2000);

            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            // Q = √(2.5² - 2²) = 1.5 MVAr
            Assert.AreEqual(1500, ReadRegister(store, PcsSimpleV1Description.Register.Q));
            Assert.AreEqual(1500, ReadRegister(store, PcsSimpleV1Description.Register.QAvailable));
        }


        [TestMethod]
        public async Task TickAsyncSplitsPowerByUsableCapacityTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            BbSimpleV1Simulation large = await CreateConnectedBankAsync(11, 5000000, 2500000);
            BbSimpleV1Simulation small = await CreateConnectedBankAsync(12, 2500000, 2500000);
            pcs.AttachBatteryBanks(new[] { large, small });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, 1500);

            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(1000000, large.ActivePower, 0.001);
            Assert.AreEqual(500000, small.ActivePower, 0.001);
        }


        [TestMethod]
        public async Task SetEventsFaultStopsPowerAndOpensBreakerTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            BbSimpleV1Simulation bank = await CreateConnectedBankAsync(11, 5000000, 2500000);
            pcs.AttachBatteryBanks(new[] { bank });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, 1000);
            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            pcs.SetEvents(null, 0b0000_0101, null);
            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.Fault, pcs.State);
            Assert.AreEqual((short)PcsSimpleV1Description.State.Fault, ReadRegister(store, PcsSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(0b0000_0101, ReadRegister(store, PcsSimpleV1Description.Register.CurrentFault));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.P));
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.ACBreaker));
            Assert.AreEqual(0, bank.ActivePower);
        }


        [TestMethod]
        public async Task SetEventsClearingFaultKeepsFaultStateTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            pcs.AttachBatteryBanks(new[] { await CreateConnectedBankAsync(11, 5000000, 2500000) });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            pcs.SetEvents(null, 1, null);

            pcs.SetEvents(null, 0, null);
            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.Fault, pcs.State);
            Assert.AreEqual(0, ReadRegister(store, PcsSimpleV1Description.Register.CurrentFault));
            Assert.AreEqual((short)PcsSimpleV1Description.State.Fault, ReadRegister(store, PcsSimpleV1Description.Register.CurrentState));
        }


        [TestMethod]
        [DataRow(PcsSimpleV1Description.State.On, PowerConversionSystemSimulationState.On, DisplayName = "Start command written again leaves the fault")]
        [DataRow(PcsSimpleV1Description.State.Off, PowerConversionSystemSimulationState.Off, DisplayName = "Stop command leaves the fault")]
        [DataRow(PcsSimpleV1Description.State.Standby, PowerConversionSystemSimulationState.Standby, DisplayName = "Standby command leaves the fault")]
        public async Task TickAsyncNextCommandLeavesFaultTest(PcsSimpleV1Description.State command, PowerConversionSystemSimulationState expected)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            pcs.AttachBatteryBanks(new[] { await CreateConnectedBankAsync(11, 5000000, 2500000) });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            pcs.SetEvents(null, 1, null);
            pcs.SetEvents(null, 0, null);

            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)command);
            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(expected, pcs.State);
        }


        [TestMethod]
        public async Task SlowDownPowerRampsActivePowerTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());
            pcs.AttachBatteryBanks(new[] { await CreateConnectedBankAsync(11, 5000000, 2500000) });
            WriteCommand(store, PcsSimpleV1Description.Register.SelectorState, (short)PcsSimpleV1Description.State.On);
            WriteCommand(store, PcsSimpleV1Description.Register.PReference, 1000);
            pcs.SlowDownPower(10, 0);

            await pcs.TickAsync(TimeSpan.FromSeconds(1), CancellationToken.None);
            double afterOneSecond = pcs.ActivePower;
            await pcs.TickAsync(TimeSpan.FromSeconds(10), CancellationToken.None);

            // 10% of 2MW per second = 200kW per second
            Assert.AreEqual(200000, afterOneSecond, 0.001);
            Assert.AreEqual(1000000, pcs.ActivePower, 0.001);
            Assert.AreEqual(10, pcs.SlowActivePowerByPercent);
        }


        [TestMethod]
        [DataRow(-1d, 0d)]
        [DataRow(0d, -1d)]
        public void SlowDownPowerNegativeThrowsTest(double active, double reactive)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => pcs.SlowDownPower(active, reactive));
        }


        [TestMethod]
        public void SetEventsWritesWarningAndVendorEventBitmasksTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, CreatePcsSettings(), new SimulatedGrid());

            pcs.SetEvents(0b0001_1000, null, 0b0000_0001);

            Assert.AreEqual(0b0001_1000, ReadRegister(store, PcsSimpleV1Description.Register.CurrentWarning));
            Assert.AreEqual(0b0000_0001, ReadRegister(store, PcsSimpleV1Description.Register.CurrentVendorEvent));
            Assert.AreEqual(PowerConversionSystemSimulationState.Off, pcs.State);
            Assert.IsTrue(pcs.HasWarningRegister);
            Assert.IsTrue(pcs.HasFaultRegister);
            Assert.IsTrue(pcs.HasVendorEventRegister);
        }


        /// <summary>
        /// 2MW / 2MVAr / 2.5MVA PCS.
        /// </summary>
        private PowerConversionSystemSimulationSettings CreatePcsSettings()
        {
            return new PowerConversionSystemSimulationSettings
            {
                DeviceId = 1,
                Name = "PCS 1",
                NameplateMaximumActivePower = 2000000,
                NameplateMaximumReactivePower = 2000000,
                NameplateMaximumApparentPower = 2500000,
                NameplateMaximumACCurrent = 2187
            };
        }


        /// <summary>
        /// Battery bank with a usable window of 10..90% and 87.5% usable state of charge at start.
        /// </summary>
        private BatteryBankSimulationSettings CreateBankSettings(int deviceId, double nameplateCapacity, double rate)
        {
            return new BatteryBankSimulationSettings
            {
                DeviceId = deviceId,
                Name = $"BB 1.{deviceId}",
                NameplateCapacity = nameplateCapacity,
                NameplateMaximumChargeRate = rate,
                NameplateMaximumDischargeRate = rate,
                MinimumDcVoltage = 1000,
                MaximumDcVoltage = 1500
            };
        }


        private async Task<BbSimpleV1Simulation> CreateConnectedBankAsync(int deviceId, double nameplateCapacity, double rate)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation bank = new BbSimpleV1Simulation(store, CreateBankSettings(deviceId, nameplateCapacity, rate));
            store.HoldingRegisters.WritePoints((ushort)BbSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)BbSimpleV1Description.State.Connected });
            await bank.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            return bank;
        }


        /// <summary>
        /// Writes a register like a Modbus client (controller) does.
        /// </summary>
        private void WriteCommand(SimulationModbusDataStore store, PcsSimpleV1Description.Register register, short value)
        {
            store.HoldingRegisters.WritePoints((ushort)register, new ushort[] { unchecked((ushort)value) });
        }


        private short ReadRegister(SimulationModbusDataStore store, PcsSimpleV1Description.Register register)
        {
            return unchecked((short)store.Device.HoldingRegisters.ReadPoints((ushort)register, 1)[0]);
        }
    }
}
