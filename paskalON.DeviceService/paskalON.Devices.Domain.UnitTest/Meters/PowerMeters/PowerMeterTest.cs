// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.Meters.PowerMeters
{
    [TestClass]
    public class PowerMeterTest
    {
        private PowerMeterMapC37Config? _powerMeterMapC37Config;
        private PowerMeterDeviceConfig? _powerMeterDeviceConfig;
        private SystemPowerMeterConfig? _powerMeterConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<C37Config> c37Config = new Mock<C37Config>();
            c37Config.SetupGet(x => x.Name).Returns("C37Config");

            _powerMeterMapC37Config = new PowerMeterMapC37Config
            {
                ChangedBy = "Test",
                Name = "PowerMeterMapC37Config",
                // Power
                ActivePower = "AP",
                ActivePowerA = "APA",
                ActivePowerB = "APB",
                ActivePowerC = "ABC",
                ReactivePower = "RAP",
                ReactivePowerA = "RAPA",
                ReactivePowerB = "RAPB",
                ReactivePowerC = "RAPC",
                ApparentPower = "APP",
                // Current
                CurrentA = "CA",
                CurrentB = "CB",
                CurrentC = "CC",
                // Energy
                EnergyDelivered = "ED",
                EnergyReceived = "ER",
                ReactiveEnergyDelivered = "RED",
                ReactiveEnergyReceived = "RER",
                // Voltage
                VoltageA = "VA",
                VoltageB = "VB",
                VoltageC = "VC",
                VoltageAB = "VAB",
                VoltageBC = "VBC",
                VoltageCA = "VCA",
                VoltageLLAvg = "VLLAvg",
                VoltagePositiveSequence = "VPS",
            };

            _powerMeterDeviceConfig = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "PowerMeterDeviceConfig",
                ClassName = "ClassName",
                PowerMeterMapC37Config = _powerMeterMapC37Config
            };

            _powerMeterConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "PowerMeterBaseConfig",
                IsActive = true,
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = derConfig.Object,
                C37Config = c37Config.Object,
                PowerMeterDeviceConfig = _powerMeterDeviceConfig
            };
        }



        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerMeter(NullLogger.Instance, null!, publisher.Object, dataface.Object));
        }


        [TestMethod]
        public void RegisterDatafaceWrongRegisterTypeTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            Assert.ThrowsExactly<ArgumentException>(() => new PowerMeter(NullLogger.Instance, _powerMeterConfig!, publisher.Object, dataface));
        }



        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            PowerMeter powerMeter = new PowerMeter(NullLogger.Instance, _powerMeterConfig!, publisher.Object, dataface);

            HashSet<string?> expectedNames = new HashSet<string?>
            {
                "FREQUENCY",
                // Power
                _powerMeterMapC37Config!.ActivePower,
                _powerMeterMapC37Config!.ActivePowerA,
                _powerMeterMapC37Config!.ActivePowerB,
                _powerMeterMapC37Config!.ActivePowerC,
                _powerMeterMapC37Config!.ApparentPower,
                _powerMeterMapC37Config!.ReactivePower,
                _powerMeterMapC37Config!.ReactivePowerA,
                _powerMeterMapC37Config!.ReactivePowerB,
                _powerMeterMapC37Config!.ReactivePowerC,
                // Current
                _powerMeterMapC37Config!.CurrentA,
                _powerMeterMapC37Config!.CurrentB,
                _powerMeterMapC37Config!.CurrentC,
                // Energy
                _powerMeterMapC37Config!.EnergyDelivered,
                _powerMeterMapC37Config!.EnergyReceived,
                _powerMeterMapC37Config!.ReactiveEnergyDelivered,
                _powerMeterMapC37Config!.ReactiveEnergyReceived,
                // Voltage
                _powerMeterMapC37Config!.VoltageA,
                _powerMeterMapC37Config!.VoltageB,
                _powerMeterMapC37Config!.VoltageC,
                _powerMeterMapC37Config!.VoltageAB,
                _powerMeterMapC37Config!.VoltageBC,
                _powerMeterMapC37Config!.VoltageCA,
                _powerMeterMapC37Config!.VoltageLLAvg,
                _powerMeterMapC37Config!.VoltagePositiveSequence,
            };

            HashSet<string> registeredNames = dataface.Registers.Select(r => r.Name).ToHashSet();

            Assert.IsNotNull(powerMeter.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(expectedNames.Count, registeredNames);
            CollectionAssert.AreEquivalent(expectedNames.ToList(), registeredNames.ToList());
        }


        [TestMethod]
        public void RegisterDatafaceOnlyConfiguredTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");

            _powerMeterMapC37Config = new PowerMeterMapC37Config
            {
                ChangedBy = "Test",
                Name = "PowerMeterMapC37Config",
                ActivePower = "AP",
                ReactivePower = "RAP",
            };

            _powerMeterDeviceConfig!.PowerMeterMapC37Config = _powerMeterMapC37Config;
            PowerMeter powerMeter = new PowerMeter(NullLogger.Instance, _powerMeterConfig!, publisher.Object, dataface);

            HashSet<string?> expectedNames = new HashSet<string?>
            {
                "FREQUENCY",
                // Power
                _powerMeterMapC37Config!.ActivePower,
                _powerMeterMapC37Config!.ReactivePower,
            };

            HashSet<string> registeredNames = dataface.Registers.Select(r => r.Name).ToHashSet();

            Assert.IsNotNull(powerMeter.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(expectedNames.Count, dataface.Registers);
            Assert.HasCount(expectedNames.Count, registeredNames);
            CollectionAssert.AreEquivalent(expectedNames.ToList(), registeredNames.ToList());
        }


        [TestMethod]
        public async Task PowerMeterConnectTest()
        {
            FakeLogger<PowerMeter> logger = new FakeLogger<PowerMeter>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            PowerMeter powerMeter = new PowerMeter(logger, _powerMeterConfig!, publisher.Object, dataface);

            await powerMeter.ConnectAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PowerMeterState.Connecting, powerMeter.State);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("connect requested", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("state changed", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task PowerMeterDisconnectTest()
        {
            FakeLogger<PowerMeter> logger = new FakeLogger<PowerMeter>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            PowerMeter powerMeter = new PowerMeter(logger, _powerMeterConfig!, publisher.Object, dataface);

            await powerMeter.DisconnectAsync();

            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Information);
            Assert.AreEqual(PowerMeterState.Disconnecting, powerMeter.State);
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("disconnect requested", StringComparison.OrdinalIgnoreCase)));
            Assert.IsNotNull(logs.FirstOrDefault(m => m.Message.Contains("state changed", StringComparison.OrdinalIgnoreCase)));
        }

    }
}