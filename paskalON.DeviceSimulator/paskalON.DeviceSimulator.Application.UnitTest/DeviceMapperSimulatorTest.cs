// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.DeviceSimulator.Dto.Simulations;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Protocols.C37118;
using paskalON.Protocols.Modbus.Stores;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.DeviceSimulator.Application.UnitTest
{
    [TestClass]
    public sealed class DeviceMapperSimulatorTest
    {
        private static DeviceMapperSimulator CreateMapper(Func<string, int, bool> isResponding)
        {
            return new DeviceMapperSimulator(new SimulationStoreRegistry(), new SimulationStreamRegistry(), new SimulationDeviceRegistry(), isResponding);
        }


        private static SystemPowerMeterSimpleV1Proxy CreateSystemPowerMeter()
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");

            SystemPowerMeterConfig meterConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "System Power Meter",
                IsActive = true,
                DeviceId = 21,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = derConfig.Object,
                PowerMeterDeviceConfig = new PowerMeterDeviceConfig
                {
                    ChangedBy = "Test",
                    Name = "System Power Meter Device",
                    ClassName = "paskalON.Devices.Equipments.Meters.PowerMeters.Simples.SystemPowerMeterSimpleV1Proxy",
                    IsCurrentSigned = true,
                    PowerMeterMapC37Config = new PowerMeterMapC37Config
                    {
                        ChangedBy = "Test",
                        Name = "PowerMeterMapC37Config",
                        ActivePower = "Analog1",
                        ReactivePower = "Analog5"
                    }
                },
                C37Config = new C37Config
                {
                    ChangedBy = "Test",
                    Name = "C37Config",
                    C37ConnectionConfig = new C37ConnectionConfig { ChangedBy = "Test", Name = "C37Config for all" },
                    AddressFamily = AddressFamily.InterNetwork,
                    StationName = "PMU",
                    StreamId = 1,
                    Address = "paskalon.devicesimulator.service",
                    Port = 4712,
                    TransportLayer = C37TransportLayer.TCP
                }
            };

            return new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, meterConfig, new Mock<IMetricsPublisher>().Object,
                new Mock<IC37Dataface>().Object, new Mock<IC37Client>().Object);
        }


        [TestMethod]
        public void DeviceMapperSimulatorNullIsRespondingThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                new DeviceMapperSimulator(new SimulationStoreRegistry(), new SimulationStreamRegistry(), new SimulationDeviceRegistry(), null!));
        }


        [TestMethod]
        public void MapSimulationPowerConversionSystemMapsEventsAndSlowDownTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(store, new PowerConversionSystemSimulationSettings
            {
                DeviceId = 1,
                Name = "PCS 1",
                NameplateMaximumActivePower = 2000000,
                NameplateMaximumReactivePower = 2000000,
                NameplateMaximumApparentPower = 2500000
            }, new SimulatedGrid());
            pcs.SetEvents(0b0000_0100, 0b0000_0001, 0b0000_0010);
            pcs.SlowDownPower(10, 5);
            DeviceMapperSimulator mapper = CreateMapper((address, port) => true);

            SimulationDto dto = mapper.MapSimulation(pcs, "paskalon.devicesimulator.service", 502);

            Assert.AreEqual(SimulationDeviceType.PowerConversionSystem, dto.DeviceType);
            Assert.AreEqual(nameof(PowerConversionSystemSimulationState.Fault), dto.State);
            Assert.IsTrue(dto.IsResponding);
            Assert.IsTrue(dto.HasWarningRegister);
            Assert.IsTrue(dto.HasFaultRegister);
            Assert.IsTrue(dto.HasVendorEventRegister);
            Assert.AreEqual((ushort)0b0000_0100, dto.Warning);
            Assert.AreEqual((ushort)0b0000_0001, dto.Fault);
            Assert.AreEqual((ushort)0b0000_0010, dto.VendorEvent);
            Assert.AreEqual(10, dto.SlowActivePowerByPercent);
            Assert.AreEqual(5, dto.SlowReactivePowerByPercent);
            Assert.IsNull(dto.UsableStateOfCharge);
        }


        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void MapSimulationBatteryBankMapsCommunicationAndStateOfChargeTest(bool isResponding)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation bank = new BbSimpleV1Simulation(store, new BatteryBankSimulationSettings
            {
                DeviceId = 11,
                Name = "BB 1.1",
                NameplateCapacity = 5000000,
                NameplateMaximumChargeRate = 2500000,
                NameplateMaximumDischargeRate = 2500000,
                MinimumDcVoltage = 1000,
                MaximumDcVoltage = 1500
            });
            string? requestedAddress = null;
            int requestedPort = 0;
            DeviceMapperSimulator mapper = CreateMapper((address, port) =>
            {
                requestedAddress = address;
                requestedPort = port;

                return isResponding;
            });

            SimulationDto dto = mapper.MapSimulation(bank, "paskalon.devicesimulator.service", 503);

            Assert.AreEqual(SimulationDeviceType.BatteryBank, dto.DeviceType);
            Assert.AreEqual(nameof(BatteryBankSimulationState.Disconnected), dto.State);
            Assert.AreEqual(isResponding, dto.IsResponding);
            Assert.AreEqual("paskalon.devicesimulator.service", requestedAddress);
            Assert.AreEqual(503, requestedPort);
            Assert.AreEqual(87.5, dto.UsableStateOfCharge);
            Assert.AreEqual(100, dto.PreferredStateOfCharge);
            Assert.AreEqual(BatteryBankSimulationBase.MinimumChargeSpeedFactor, dto.ChargeSpeedFactor);
            Assert.AreEqual((ushort)0, dto.Fault);
            Assert.IsNull(dto.SlowActivePowerByPercent);
        }


        [TestMethod]
        public void MapSimulationBatteryBankMapsPreferredStateOfChargeAndChargeSpeedTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation bank = new BbSimpleV1Simulation(store, new BatteryBankSimulationSettings
            {
                DeviceId = 11,
                Name = "BB 1.1",
                NameplateCapacity = 5000000,
                NameplateMaximumChargeRate = 2500000,
                NameplateMaximumDischargeRate = 2500000,
                MinimumDcVoltage = 1000,
                MaximumDcVoltage = 1500
            });
            bank.SetPreferredStateOfCharge(50);
            bank.SetChargeSpeedFactor(100);
            DeviceMapperSimulator mapper = CreateMapper((address, port) => true);

            SimulationDto dto = mapper.MapSimulation(bank, "paskalon.devicesimulator.service", 503);

            Assert.AreEqual(50, dto.PreferredStateOfCharge);
            Assert.AreEqual(50, dto.UsableStateOfCharge);
            Assert.AreEqual(100, dto.ChargeSpeedFactor);
            Assert.IsNull(dto.SlowActivePowerByPercent);
        }


        [TestMethod]
        [DataRow(true, DeviceMapperSimulator.PowerMeterStreamingState)]
        [DataRow(false, DeviceMapperSimulator.PowerMeterStoppedState)]
        public void MapPowerMeterSimulationMapsCommunicationTest(bool isResponding, string expectedState)
        {
            SystemPowerMeterSimpleV1Proxy meter = CreateSystemPowerMeter();
            string? requestedAddress = null;
            int requestedPort = 0;
            DeviceMapperSimulator mapper = CreateMapper((address, port) =>
            {
                requestedAddress = address;
                requestedPort = port;

                return isResponding;
            });

            SimulationDto dto = mapper.MapPowerMeterSimulation(meter);

            Assert.AreEqual(SimulationDeviceType.PowerMeter, dto.DeviceType);
            Assert.AreEqual(expectedState, dto.State);
            Assert.AreEqual(isResponding, dto.IsResponding);
            Assert.AreEqual("paskalon.devicesimulator.service", requestedAddress);
            Assert.AreEqual(4712, requestedPort);
            Assert.IsFalse(dto.HasWarningRegister);
            Assert.IsFalse(dto.HasFaultRegister);
            Assert.IsFalse(dto.HasVendorEventRegister);
            Assert.IsNull(dto.Warning);
            Assert.IsNull(dto.Fault);
            Assert.IsNull(dto.VendorEvent);
        }


        [TestMethod]
        public void MapPowerMeterSimulationNullDeviceThrowsTest()
        {
            DeviceMapperSimulator mapper = CreateMapper((address, port) => true);

            Assert.ThrowsExactly<ArgumentNullException>(() => mapper.MapPowerMeterSimulation(null!));
        }
    }
}
