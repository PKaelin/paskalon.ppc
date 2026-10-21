// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.Meters.PowerMeters.Simples;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Protocols.Modbus.Stores;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.DeviceSimulator.Equipments.IntegrationTest.Meters.PowerMeters.Simples
{
    [TestClass]
    public sealed class SystemPowerMeterSimpleV1SimulationTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private IServiceProvider? _serviceProvider;


        [TestInitialize]
        public void TestInitialize()
        {
            ServiceCollection services = new ServiceCollection();
            Mock<ILoggerFactory> mockLoggerFactory = new Mock<ILoggerFactory>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            services.AddSingleton<ILoggerFactory>(mockLoggerFactory.Object);
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            _serviceProvider = services.BuildServiceProvider();
        }


        private SystemPowerMeterSimpleV1Proxy CreateMeter(bool isReversePowerFlow)
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");

            PowerMeterMapC37Config map = new PowerMeterMapC37Config
            {
                ChangedBy = "Test",
                Name = "PowerMeterMapC37Config",
                ApparentPower = "Analog0",
                ActivePower = "Analog1",
                ActivePowerA = "Analog2",
                ActivePowerB = "Analog3",
                ActivePowerC = "Analog4",
                ReactivePower = "Analog5",
                ReactivePowerA = "Analog6",
                ReactivePowerB = "Analog7",
                ReactivePowerC = "Analog8",
                VoltageA = "Phasor0",
                VoltageB = "Phasor1",
                VoltageC = "Phasor2",
                VoltageAB = "Phasor3",
                VoltageBC = "Phasor4",
                VoltageCA = "Phasor5",
                VoltagePositiveSequence = "Phasor6",
                VoltageLLAvg = "Analog13",
                CurrentA = "Phasor7",
                CurrentB = "Phasor8",
                CurrentC = "Phasor9",
            };

            C37Config c37Config = new C37Config
            {
                ChangedBy = "Test",
                Name = "C37Config",
                C37ConnectionConfig = new C37ConnectionConfig { ChangedBy = "Test", Name = "C37Config for all" },
                AddressFamily = AddressFamily.InterNetwork,
                StationName = "PMU",
                StreamId = 1,
                Address = "127.0.0.1",
                Port = 4712,
                TransportLayer = C37TransportLayer.UDP
            };

            SystemPowerMeterConfig meterConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "System Power Meter",
                IsActive = true,
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = derConfig.Object,
                PowerMeterDeviceConfig = new PowerMeterDeviceConfig
                {
                    ChangedBy = "Test",
                    Name = "System Power Meter Device",
                    ClassName = "paskalON.Devices.Equipments.Meters.PowerMeters.Simples.SystemPowerMeterSimpleV1Proxy",
                    IsReversePowerFlow = isReversePowerFlow,
                    IsCurrentSigned = true,
                    PowerMeterMapC37Config = map
                },
                C37Config = c37Config,
            };

            C37DeviceFactory factory = new C37DeviceFactory(_serviceProvider!);
            (IC37Dataface dataface, IC37Client client) = factory.Create(c37Config);

            return new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, meterConfig, _publisher.Object, dataface, client);
        }


        /// <summary>
        /// Creates a started PCS with a connected battery bank that outputs the given power.
        /// </summary>
        private static async Task<PowerConversionSystemSimulationBase> CreateRunningPcsAsync(int deviceId, short activeKw, short reactiveKvar, SimulatedGrid grid)
        {
            SimulationModbusDataStore bankStore = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            BbSimpleV1Simulation bank = new BbSimpleV1Simulation(bankStore, new BatteryBankSimulationSettings
            {
                DeviceId = deviceId,
                Name = $"BB {deviceId}",
                NameplateCapacity = 5000000,
                NameplateMaximumChargeRate = 2500000,
                NameplateMaximumDischargeRate = 2500000,
                MinimumDcVoltage = 1000,
                MaximumDcVoltage = 1500
            });
            bankStore.HoldingRegisters.WritePoints((ushort)BbSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)BbSimpleV1Description.State.Connected });
            await bank.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            SimulationModbusDataStore pcsStore = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            PcsSimpleV1Simulation pcs = new PcsSimpleV1Simulation(pcsStore, new PowerConversionSystemSimulationSettings
            {
                DeviceId = deviceId,
                Name = $"PCS {deviceId}",
                NameplateMaximumActivePower = 2000000,
                NameplateMaximumReactivePower = 2000000,
                NameplateMaximumApparentPower = 2500000
            }, grid);
            pcs.AttachBatteryBanks(new[] { bank });
            pcsStore.HoldingRegisters.WritePoints((ushort)PcsSimpleV1Description.Register.SelectorState, new ushort[] { (ushort)PcsSimpleV1Description.State.On });
            pcsStore.HoldingRegisters.WritePoints((ushort)PcsSimpleV1Description.Register.PReference, new ushort[] { unchecked((ushort)activeKw) });
            pcsStore.HoldingRegisters.WritePoints((ushort)PcsSimpleV1Description.Register.QReference, new ushort[] { unchecked((ushort)reactiveKvar) });
            await pcs.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            return pcs;
        }


        [TestMethod]
        [DataRow(false, 1d, DisplayName = "Load oriented meter")]
        [DataRow(true, -1d, DisplayName = "Reversed power flow meter")]
        public async Task TickAsyncMeasuresSumOfPowerConversionSystemsTest(bool isReversePowerFlow, double direction)
        {
            SimulatedGrid grid = new SimulatedGrid();
            grid.Configure(60, 34500);
            SystemPowerMeterSimpleV1Proxy meter = CreateMeter(isReversePowerFlow);
            PmuDataSimulation stream = new SimulationStreamRegistry().GetOrCreate(meter);
            PowerConversionSystemSimulationBase[] pcs = new[]
            {
                await CreateRunningPcsAsync(1, 1200, 500, grid),
                await CreateRunningPcsAsync(2, 600, 0, grid)
            };
            SystemPowerMeterSimpleV1Simulation simulation = new SystemPowerMeterSimpleV1Simulation(stream, meter, pcs, grid);

            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(1800000 * direction, stream.Analogs["Analog1"].Measurement, 0.5);
            Assert.AreEqual(600000 * direction, stream.Analogs["Analog2"].Measurement, 0.5);
            Assert.AreEqual(500000 * direction, stream.Analogs["Analog5"].Measurement, 0.5);
            Assert.AreEqual(Math.Sqrt(1800000d * 1800000d + 500000d * 500000d), stream.Analogs["Analog0"].Measurement, 1);
            Assert.AreEqual(60, stream.Frequency, 0.0001);
        }


        [TestMethod]
        public async Task TickAsyncWritesVoltageAndCurrentPhasorsTest()
        {
            SimulatedGrid grid = new SimulatedGrid();
            grid.Configure(60, 34500);
            SystemPowerMeterSimpleV1Proxy meter = CreateMeter(false);
            PmuDataSimulation stream = new SimulationStreamRegistry().GetOrCreate(meter);
            PowerConversionSystemSimulationBase[] pcs = new[] { await CreateRunningPcsAsync(1, 1800, 500, grid) };
            SystemPowerMeterSimpleV1Simulation simulation = new SystemPowerMeterSimpleV1Simulation(stream, meter, pcs, grid);

            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            double phaseVoltage = 34500 / Math.Sqrt(3);
            double current = Math.Sqrt(1800000d * 1800000d + 500000d * 500000d) / (Math.Sqrt(3) * 34500);
            double powerFactorAngle = Math.Atan2(500000, 1800000);
            Assert.AreEqual(34500, stream.Analogs["Analog13"].Measurement, 0.5);
            Assert.AreEqual(phaseVoltage, stream.Phasors["Phasor0"].Magnitude, 0.5);
            Assert.AreEqual(0, stream.Phasors["Phasor0"].Angle, 0.0001);
            Assert.AreEqual(-2 * Math.PI / 3, stream.Phasors["Phasor1"].Angle, 0.0001);
            Assert.AreEqual(2 * Math.PI / 3, stream.Phasors["Phasor2"].Angle, 0.0001);
            Assert.AreEqual(34500, stream.Phasors["Phasor3"].Magnitude, 0.5);
            Assert.AreEqual(Math.PI / 6, stream.Phasors["Phasor3"].Angle, 0.0001);
            Assert.AreEqual(current, stream.Phasors["Phasor7"].Magnitude, 0.01);
            Assert.AreEqual(-powerFactorAngle, stream.Phasors["Phasor7"].Angle, 0.0001);
        }


        [TestMethod]
        public async Task TickAsyncWithoutPowerConversionSystemsMeasuresNothingTest()
        {
            SimulatedGrid grid = new SimulatedGrid();
            SystemPowerMeterSimpleV1Proxy meter = CreateMeter(false);
            PmuDataSimulation stream = new SimulationStreamRegistry().GetOrCreate(meter);
            SystemPowerMeterSimpleV1Simulation simulation = new SystemPowerMeterSimpleV1Simulation(stream, meter,
                Array.Empty<PowerConversionSystemSimulationBase>(), grid);

            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual(0, stream.Analogs["Analog1"].Measurement);
            Assert.AreEqual(0, stream.Phasors["Phasor7"].Magnitude);
            Assert.AreEqual(SimulatedGrid.DefaultNominalVoltageLineToLine, stream.Analogs["Analog13"].Measurement, 0.5);
        }
    }
}
