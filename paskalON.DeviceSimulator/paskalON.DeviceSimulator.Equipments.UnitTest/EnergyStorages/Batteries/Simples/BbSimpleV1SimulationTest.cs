// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.UnitTest.EnergyStorages.Batteries.Simples
{
    [TestClass]
    public sealed class BbSimpleV1SimulationTest
    {

        [TestMethod]
        public void BbSimpleV1SimulationNullStoreThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Simulation(null!, CreateSettings()));
        }


        [TestMethod]
        public void BbSimpleV1SimulationNullSettingsThrowsTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());

            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Simulation(store, (BatteryBankSimulationSettings)null!));
        }


        [TestMethod]
        public void BbSimpleV1SimulationStartsDisconnectedAndFullyChargedTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());

            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(store, CreateSettings());

            Assert.AreEqual(BatteryBankSimulationState.Disconnected, simulation.State);
            Assert.AreEqual(87.5, simulation.UsableStateOfCharge);
            Assert.AreEqual(80, simulation.AbsoluteStateOfCharge, 0.000001);
            Assert.AreEqual(4000000, simulation.UsableCapacity);
            Assert.AreEqual(0, simulation.AvailableDischargePower);
            Assert.AreEqual(0, simulation.AvailableChargePower);
        }


        [TestMethod]
        public async Task TickAsyncWithoutConnectCommandWritesDisconnectedRegistersTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(store, CreateSettings());

            await simulation.TickAsync(TimeSpan.FromSeconds(1), CancellationToken.None);

            Assert.AreEqual(BatteryBankSimulationState.Disconnected, simulation.State);
            Assert.AreEqual((ushort)BbSimpleV1Description.State.Disconnected, ReadRegister(store, BbSimpleV1Description.Register.CurrentState));
            Assert.AreEqual((ushort)8750, ReadRegister(store, BbSimpleV1Description.Register.TotalStateOfCharge));
            Assert.AreEqual((ushort)1400, ReadRegister(store, BbSimpleV1Description.Register.TotalDCVoltage));
            Assert.AreEqual((ushort)0, ReadRegister(store, BbSimpleV1Description.Register.TotalDCCurrent));
        }


        [TestMethod]
        public async Task TickAsyncConnectCommandConnectsTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());

            BbSimpleV1Simulation simulation = await CreateConnectedAsync(store);

            Assert.AreEqual(BatteryBankSimulationState.Connected, simulation.State);
            Assert.AreEqual((ushort)BbSimpleV1Description.State.Connected, ReadRegister(store, BbSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(2500000, simulation.AvailableDischargePower);
            Assert.AreEqual(2500000, simulation.AvailableChargePower);
        }


        [TestMethod]
        public async Task TickAsyncDisconnectCommandDisconnectsTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = await CreateConnectedAsync(store);
            simulation.SetAllocatedActivePower(1000000);
            store.HoldingRegisters.WritePoints((ushort)BbSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)BbSimpleV1Description.State.Disconnected });

            await simulation.TickAsync(TimeSpan.FromMinutes(30), CancellationToken.None);

            Assert.AreEqual(BatteryBankSimulationState.Disconnected, simulation.State);
            Assert.AreEqual(0, simulation.ActivePower);
            Assert.AreEqual(87.5, simulation.UsableStateOfCharge);
        }


        [TestMethod]
        [DataRow(2d, BatteryBankSimulationState.PreCharging, DisplayName = "Pre-charging before the delay elapsed")]
        [DataRow(5d, BatteryBankSimulationState.Connected, DisplayName = "Connected when the delay elapsed")]
        public async Task TickAsyncPreChargeDelaysConnectTest(double elapsedSeconds, BatteryBankSimulationState expected)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(store, CreateSettings(preChargeDelaySeconds: 5));
            store.HoldingRegisters.WritePoints((ushort)BbSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)BbSimpleV1Description.State.Connected });
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            await simulation.TickAsync(TimeSpan.FromSeconds(elapsedSeconds), CancellationToken.None);

            Assert.AreEqual(expected, simulation.State);
        }


        [TestMethod]
        public void SetAllocatedActivePowerWhileDisconnectedIsIgnoredTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(store, CreateSettings());

            simulation.SetAllocatedActivePower(1000000);

            Assert.AreEqual(0, simulation.ActivePower);
        }


        [TestMethod]
        public async Task TickAsyncDischargeIntegratesStateOfChargeTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = await CreateConnectedAsync(store);
            simulation.SetAllocatedActivePower(2000000);

            await simulation.TickAsync(TimeSpan.FromMinutes(30), CancellationToken.None);

            // 2MW * 0.5h / 4MWh = 25% of the usable capacity
            Assert.AreEqual(62.5, simulation.UsableStateOfCharge, 0.000001);
            Assert.AreEqual((ushort)6250, ReadRegister(store, BbSimpleV1Description.Register.TotalStateOfCharge));
            Assert.AreEqual((ushort)BbSimpleV1Description.State.Discharging, ReadRegister(store, BbSimpleV1Description.Register.CurrentState));
        }


        [TestMethod]
        public async Task TickAsyncChargeIntegratesStateOfChargeTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = await CreateConnectedAsync(store);
            simulation.SetAllocatedActivePower(-1000000);

            await simulation.TickAsync(TimeSpan.FromMinutes(30), CancellationToken.None);

            // 1MW * 0.5h / 4MWh = 12.5% of the usable capacity
            Assert.AreEqual(100, simulation.UsableStateOfCharge, 0.000001);
            Assert.AreEqual((ushort)BbSimpleV1Description.State.Charging, ReadRegister(store, BbSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(0, simulation.AvailableChargePower);
        }


        [TestMethod]
        public async Task TickAsyncDischargeCutsOffAtZeroTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = await CreateConnectedAsync(store);
            simulation.SetAllocatedActivePower(2500000);

            await simulation.TickAsync(TimeSpan.FromHours(2), CancellationToken.None);

            Assert.AreEqual(0, simulation.UsableStateOfCharge);
            Assert.AreEqual(0, simulation.AvailableDischargePower);
            Assert.AreEqual(2500000, simulation.AvailableChargePower);
        }


        [TestMethod]
        public async Task TickAsyncWritesVoltageAndSignedCurrentTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = await CreateConnectedAsync(store);
            simulation.SetAllocatedActivePower(-700000);

            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            short current = unchecked((short)ReadRegister(store, BbSimpleV1Description.Register.TotalDCCurrent));
            ushort voltage = ReadRegister(store, BbSimpleV1Description.Register.TotalDCVoltage);
            Assert.AreEqual(1400, voltage, 1);
            Assert.AreEqual(-500, current, 1);
        }


        [TestMethod]
        public void SetEventsWritesBitmasksTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(store, CreateSettings());

            simulation.SetEvents(0b0000_0011, 0b1000_0000_0000_0001, 0b0000_0010);

            Assert.AreEqual((ushort)0b0000_0011, ReadRegister(store, BbSimpleV1Description.Register.CurrentWarning));
            Assert.AreEqual((ushort)0b1000_0000_0000_0001, ReadRegister(store, BbSimpleV1Description.Register.CurrentFault));
            Assert.AreEqual((ushort)0b0000_0010, ReadRegister(store, BbSimpleV1Description.Register.CurrentVendorEvent));
            Assert.AreEqual((ushort)0b0000_0011, simulation.Warning);
            Assert.AreEqual((ushort)0b1000_0000_0000_0001, simulation.Fault);
            Assert.AreEqual((ushort)0b0000_0010, simulation.VendorEvent);
        }


        [TestMethod]
        public async Task SetEventsNullLeavesRegisterUnchangedTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation simulation = await CreateConnectedAsync(store);
            simulation.SetEvents(5, 6, 7);

            simulation.SetEvents(null, 0, null);
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual((ushort)5, simulation.Warning);
            Assert.AreEqual((ushort)0, simulation.Fault);
            Assert.AreEqual((ushort)7, simulation.VendorEvent);
            Assert.AreEqual(BatteryBankSimulationState.Connected, simulation.State);
        }


        /// <summary>
        /// 5MWh battery bank with a usable window of 10..90% (4MWh usable capacity) and 2.5MW charge and discharge rate.
        /// </summary>
        private BatteryBankSimulationSettings CreateSettings(double preChargeDelaySeconds = 0)
        {
            return new BatteryBankSimulationSettings
            {
                DeviceId = 11,
                Name = "BB 1.1",
                NameplateCapacity = 5000000,
                NameplateMaximumChargeRate = 2500000,
                NameplateMaximumDischargeRate = 2500000,
                AbsoluteMinimumStateOfCharge = 0,
                AbsoluteMaximumStateOfCharge = 100,
                UsableMinimumStateOfCharge = 10,
                UsableMaximumStateOfCharge = 90,
                PreferredMinimumStateOfCharge = 20,
                PreferredMaximumStateOfCharge = 80,
                MinimumDcVoltage = 1000,
                MaximumDcVoltage = 1500,
                PreChargeDelay = TimeSpan.FromSeconds(preChargeDelaySeconds)
            };
        }


        private ushort ReadRegister(SimulationModbusDataStore store, BbSimpleV1Description.Register register)
        {
            return store.Device.HoldingRegisters.ReadPoints((ushort)register, 1)[0];
        }

        private async Task<BbSimpleV1Simulation> CreateConnectedAsync(SimulationModbusDataStore store)
        {
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(store, CreateSettings());
            store.HoldingRegisters.WritePoints((ushort)BbSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)BbSimpleV1Description.State.Connected });
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            return simulation;
        }
    }
}