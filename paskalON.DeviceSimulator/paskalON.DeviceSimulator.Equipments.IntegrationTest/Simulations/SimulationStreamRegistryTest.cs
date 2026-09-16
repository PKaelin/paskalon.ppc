// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Communication.Protocols.C37118.Types;
using paskalON.Dataface.C37s;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.DeviceSimulator.Equipments.IntegrationTest.Simulations
{
    [TestClass]
    public sealed class SimulationStreamRegistryTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private Mock<DerConfig>? _derConfig;
        private SystemPowerMeterConfig? _pmConfig;
        private C37Config? _c37Config;
        private PowerMeterMapC37Config? _powerMeterMapC37Config;
        private IServiceProvider? _serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _derConfig = new Mock<DerConfig>();
            _derConfig.SetupGet(x => x.Name).Returns("DerConfig");

            C37ConnectionConfig connectionConfig = new C37ConnectionConfig
            {
                ChangedBy = "Test",
                Name = "C37Config for all",
                ConnectionTimeoutMilliseconds = 1001,
                DisconnectionTimeoutMilliseconds = 1002,
                ConnectRetryCount = 0,
                ConnectRetryIntervalMilliseconds = 4001,
            };

            _powerMeterMapC37Config = new PowerMeterMapC37Config
            {
                ChangedBy = "Test",
                Name = "PowerMeterMapC37Config",
                // Power
                ApparentPower = "Analog0",
                ActivePower = "Analog1",
                ActivePowerA = "Analog2",
                ActivePowerB = "Analog3",
                ActivePowerC = "Analog4",
                ReactivePower = "Analog5",
                ReactivePowerA = "Analog6",
                ReactivePowerB = "Analog7",
                ReactivePowerC = "Analog8",
                EnergyDelivered = "Analog9",
                EnergyReceived = "Analog10",
                ReactiveEnergyDelivered = "Analog11",
                ReactiveEnergyReceived = "Analog12",
                // Voltage
                VoltageA = "Phasor0",
                VoltageB = "Phasor1",
                VoltageC = "Phasor2",
                VoltageAB = "Phasor3",
                VoltageBC = "Phasor4",
                VoltageCA = "Phasor5",
                VoltagePositiveSequence = "Phasor6",
                VoltageLLAvg = "Analog100",
                // Current
                CurrentA = "Phasor7",
                CurrentB = "Phasor8",
                CurrentC = "Phasor9",
            };

            _c37Config = new C37Config
            {
                ChangedBy = "Test",
                Name = "C37Config",
                C37ConnectionConfig = connectionConfig,
                AddressFamily = AddressFamily.InterNetwork,
                StationName = "PMU",
                StreamId = 1,
                Address = "127.0.0.1",
                Port = 52,
                TransportLayer = C37TransportLayer.UDP
            };

            PowerMeterDeviceConfig powerMeterDeviceConfig = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "PowerMeterDeviceConfig",
                ClassName = "ClassName",
                PowerMeterMapC37Config = _powerMeterMapC37Config
            };

            _pmConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerMeterConfig",
                IsActive = true,
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = _derConfig.Object,
                PowerMeterDeviceConfig = powerMeterDeviceConfig,
                C37Config = _c37Config,
            };

            ServiceCollection services = new ServiceCollection();
            Mock<ILoggerFactory> mockLoggerFactory = new Mock<ILoggerFactory>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            services.AddSingleton<ILoggerFactory>(mockLoggerFactory.Object);
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            _serviceProvider = services.BuildServiceProvider();
        }



        [TestMethod]
        [DataRow("PMU2", (ushort)1, "127.0.0.1", (ushort)52)]
        [DataRow("PMU", (ushort)2, "127.0.0.1", (ushort)52)]
        [DataRow("PMU", (ushort)1, "127.0.0.2", (ushort)52)]
        [DataRow("PMU", (ushort)1, "127.0.0.1", (ushort)53)]
        public void SimulationStreamRegistryKeepsStreamsDistinctAndStableTest(string stationName, ushort streamId, string address, ushort port)
        {
            SimulationStreamRegistry streams = new SimulationStreamRegistry();
            C37DeviceFactory factory = new C37DeviceFactory(_serviceProvider!);
            (IC37Dataface dataface, IC37Client client) = factory.Create(_c37Config!);
            SystemPowerMeterSimpleV1Proxy device = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, _publisher.Object, dataface, client);

            C37Config c37Config = new C37Config
            {
                ChangedBy = "Test",
                Name = "C37Config",
                C37ConnectionConfig = new C37ConnectionConfig { ChangedBy = "Test", Name = "C37Config for all" },
                AddressFamily = AddressFamily.InterNetwork,
                StationName = stationName,
                StreamId = streamId,
                Address = address,
                Port = port,
                TransportLayer = C37TransportLayer.UDP
            };

            PowerMeterDeviceConfig powerMeterDeviceConfig = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "PowerMeterDeviceConfig",
                ClassName = "ClassName",
                PowerMeterMapC37Config = _powerMeterMapC37Config
            };

            SystemPowerMeterConfig pmConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerMeterConfig",
                IsActive = true,
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = _derConfig!.Object,
                PowerMeterDeviceConfig = powerMeterDeviceConfig,
                C37Config = c37Config,
            };

            (IC37Dataface dataface2, IC37Client client2) = factory.Create(c37Config);
            SystemPowerMeterSimpleV1Proxy device2 = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, pmConfig, _publisher.Object, dataface2, client2);

            PmuDataSimulation firstStream = streams.GetOrCreate(device);
            PmuDataSimulation sameStream = streams.GetOrCreate(device);
            PmuDataSimulation secondStream = streams.GetOrCreate(device2);

            Assert.AreSame(firstStream, sameStream);
            Assert.AreNotSame(firstStream, secondStream);
            Assert.AreEqual((ushort)1, firstStream.StreamId);
            Assert.AreEqual((ushort)streamId, secondStream.StreamId);
            Assert.HasCount(2, streams.Streams);
        }
    }
}
