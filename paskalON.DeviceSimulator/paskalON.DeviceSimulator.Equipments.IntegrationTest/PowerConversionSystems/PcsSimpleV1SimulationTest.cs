// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Application.Factories;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Stores;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.DeviceSimulator.Equipments.IntegrationTest.PowerConversionSystems
{
    [TestClass]
    public sealed class PcsSimpleV1SimulationTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private Mock<DerBatteryStorageUnit>? _unit;
        private PowerConversionSystemConfig? _pcsConfig;
        private ModbusConfig? _pcsModbusConfig;


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

            PowerConversionSystemDeviceConfig deviceConfig = new PowerConversionSystemDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device PCS 1",
                ClassName = "paskalON.Devices.Equipments.PowerConversionSystems.Simples.PcsSimpleV1Proxy",
                NameplateMaximumActivePower = 3630000,
                NameplateMaximumReactivePower = 3630000,
                NameplateMaximumApparentPower = 3630000,
                NameplateMaximumACCurrent = 3175,
                MaximumDCVoltage = 1140,
                MinimumDCVoltage = 100,
                ZeroOutputOnCommLoss = true
            };

            ModbusConnectionConfig modbusConnection = new ModbusConnectionConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConnectionConfig",
            };

            _pcsModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                UnitId = 1,
                ModbusConnectionConfig = modbusConnection
            };

            // Device
            _pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                DeviceId = 1,
                Name = "PowerConversionSystemConfig",
                PowerConversionSystemDeviceConfig = deviceConfig,
                ModbusConfig = _pcsModbusConfig,
                DerUnitConfig = unitConfig.Object,
            };
        }


        /// <summary>
        /// Creates a connected 5MWh battery bank with a 2.5MW charge and discharge rate.
        /// </summary>
        private static async Task<BbSimpleV1Simulation> CreateConnectedBankAsync()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BatteryBankSimulationSettings settings = new BatteryBankSimulationSettings
            {
                DeviceId = 11,
                Name = "BB 1.1",
                NameplateCapacity = 5000000,
                NameplateMaximumChargeRate = 2500000,
                NameplateMaximumDischargeRate = 2500000,
                MinimumDcVoltage = 1000,
                MaximumDcVoltage = 1500
            };
            BbSimpleV1Simulation bank = new BbSimpleV1Simulation(store, settings);
            store.HoldingRegisters.WritePoints((ushort)BbSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)BbSimpleV1Description.State.Connected });
            await bank.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            return bank;
        }


        private static async Task<short> ReadRegisterAsync(IModbusClient client, PcsSimpleV1Description.Register register)
        {
            ushort[]? values = await client.ReadHoldingRegistersAsync((ushort)register, (ushort)register);

            Assert.IsNotNull(values);

            return unchecked((short)values[0]);
        }


        [TestMethod]
        public void PcsSimpleV1SimulationNullStoreTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);

            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Simulation(null!, device, new SimulatedGrid()));
        }


        [TestMethod]
        public void PcsSimpleV1SimulationNullDeviceTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDataStore store = stores.GetOrCreate(_pcsModbusConfig!);

            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Simulation(store, (PcsSimpleV1Proxy)null!, new SimulatedGrid()));
        }


        [TestMethod]
        public void PcsSimpleV1SimulationConstructorTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);

            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(stores.GetOrCreate(_pcsModbusConfig!), device, new SimulatedGrid());

            Assert.AreEqual(device.Name, simulation.Name);
            Assert.AreEqual(device.DeviceId, simulation.DeviceId);
            Assert.AreEqual(3630000, simulation.Settings.NameplateMaximumActivePower);
            Assert.AreEqual(3630000, simulation.Settings.NameplateMaximumReactivePower);
            // V = 3.63MVA / (√3 · 3175A) ≈ 660V
            Assert.AreEqual(660, simulation.Settings.NominalACVoltage, 1);
            Assert.AreEqual(PowerConversionSystemSimulationState.Off, simulation.State);
        }


        [TestMethod]
        public async Task PcsSimpleV1SimulationProxyCommandsDriveOutputTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);
            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(stores.GetOrCreate(_pcsModbusConfig!), device, new SimulatedGrid());
            BbSimpleV1Simulation bank = await CreateConnectedBankAsync();
            simulation.AttachBatteryBanks(new[] { bank });

            await device.StartAsync();
            await device.SetActivePowerTargetAsync(1500000);
            await device.SetReactivePowerTargetAsync(-400000);
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.On, simulation.State);
            Assert.AreEqual((short)PcsSimpleV1Description.State.On, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(1500, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.P));
            Assert.AreEqual(-400, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.Q));
            Assert.AreEqual(2500, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.PAvailable));
            Assert.AreEqual(1500000, bank.ActivePower, 0.001);
        }


        [TestMethod]
        public async Task PcsSimpleV1SimulationProxyStopStopsOutputTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);
            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(stores.GetOrCreate(_pcsModbusConfig!), device, new SimulatedGrid());
            simulation.AttachBatteryBanks(new[] { await CreateConnectedBankAsync() });
            await device.StartAsync();
            await device.SetActivePowerTargetAsync(1500000);
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            await device.StopAsync();
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.Off, simulation.State);
            Assert.AreEqual((short)PcsSimpleV1Description.State.Off, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(0, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.P));
            Assert.AreEqual(0, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.ACBreaker));
        }


        [TestMethod]
        public async Task PcsSimpleV1SimulationProxyStartAfterFaultLeavesFaultTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);
            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(stores.GetOrCreate(_pcsModbusConfig!), device, new SimulatedGrid());
            simulation.AttachBatteryBanks(new[] { await CreateConnectedBankAsync() });
            await device.StartAsync();
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            simulation.SetEvents(null, 0b0000_0010, null);
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            short faultedState = await ReadRegisterAsync(client, PcsSimpleV1Description.Register.CurrentState);
            simulation.SetEvents(null, 0, null);

            await device.StartAsync();
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual((short)PcsSimpleV1Description.State.Fault, faultedState);
            Assert.AreEqual(PowerConversionSystemSimulationState.On, simulation.State);
            Assert.AreEqual((short)PcsSimpleV1Description.State.On, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(0, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.CurrentFault));
        }


        [TestMethod]
        public async Task PcsSimpleV1SimulationProxyStartWhileFaultActiveStaysFaultedTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);
            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(stores.GetOrCreate(_pcsModbusConfig!), device, new SimulatedGrid());
            simulation.AttachBatteryBanks(new[] { await CreateConnectedBankAsync() });
            simulation.SetEvents(null, 0b0000_0010, null);

            await device.StartAsync();
            await device.SetActivePowerTargetAsync(1500000);
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(PowerConversionSystemSimulationState.Fault, simulation.State);
            Assert.AreEqual((short)PcsSimpleV1Description.State.Fault, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.CurrentState));
            Assert.AreEqual(0, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.P));
            Assert.AreEqual(0, await ReadRegisterAsync(client, PcsSimpleV1Description.Register.ACBreaker));
        }
    }
}
