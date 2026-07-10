// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Communication.Protocols.C37118.Types;
using paskalON.Dataface.C37s;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.Meters.PowerMeters
{
    [TestClass]
    public class PowerMeterTest
    {
        private DerConfig? _derConfig;
        private Der? _der;
        private C37Config? _c37Config;
        private PowerMeterMapC37Config? _powerMeterMapC37Config;
        private PowerMeterDeviceConfig? _powerMeterDeviceConfig;
        private SystemPowerMeterConfig? _powerMeterConfig;

        [TestInitialize]
        public void TestInitialize()
        {
            _derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            _der = new Der(NullLogger.Instance, _derConfig);

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
                CurrentAngleA = "CANA",
                CurrentAngleB = "CANB",
                CurrentAngleC = "CANC",
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
                VoltageAngleA = "VANA",
                VoltageAngleB = "VANB",
                VoltageAngleC = "VANC",
                VoltageAB = "VAB",
                VoltageBC = "VBC",
                VoltageCA = "VCA",
                VoltageLLAvg = "VLLAvg",
                VoltagePositiveSequence = "VPS",
                VoltagePositiveSequenceAngle = "VPSA",
                // Misc
                Frequency = "FRQ",
            };

            _powerMeterDeviceConfig = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "PowerMeterDeviceConfig",
                ClassName = "ClassName",
                PowerMeterMapC37Config = _powerMeterMapC37Config
            };

            _c37Config = new C37Config
            {
                ChangedBy = "Test",
                Name = "C37Config",
                IpAddress = "localhost",
                Port = 11,
                IdOfDataBlock = 2,
                IdOfDataStream = 1,
                TransportLayer = C37TransportLayer.UDP,
            };

            _powerMeterConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "PowerMeterBaseConfig",
                IsActive = true,
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = _derConfig!,
                C37Config = _c37Config!,
                PowerMeterDeviceConfig = _powerMeterDeviceConfig
            };
        }



        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerMeter(NullLogger.Instance, null, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void RegisterDatafaceWrongRegisterTypeTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister();
            Assert.ThrowsExactly<ArgumentException>(() => new PowerMeter(NullLogger.Instance, _powerMeterConfig!, publisher.Object, dataface));
        }



        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register();
            PowerMeter powerMeter = new PowerMeter(NullLogger.Instance, _powerMeterConfig!, publisher.Object, dataface);

            var expectedNames = new HashSet<string?>
            {
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
                _powerMeterMapC37Config!.CurrentAngleA,
                _powerMeterMapC37Config!.CurrentB,
                _powerMeterMapC37Config!.CurrentAngleB,
                _powerMeterMapC37Config!.CurrentC,
                _powerMeterMapC37Config!.CurrentAngleC,
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
                _powerMeterMapC37Config!.VoltageAngleA,
                _powerMeterMapC37Config!.VoltageAngleB,
                _powerMeterMapC37Config!.VoltageAngleC,
                _powerMeterMapC37Config!.VoltageLLAvg,
                _powerMeterMapC37Config!.VoltagePositiveSequence,
                _powerMeterMapC37Config!.VoltagePositiveSequenceAngle,
                // Misc
                _powerMeterMapC37Config!.Frequency,
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
            C37Register dataface = new C37Register();

            _powerMeterMapC37Config = new PowerMeterMapC37Config
            {
                ChangedBy = "Test",
                Name = "PowerMeterMapC37Config",
                ActivePower = "AP",
                ReactivePower = "RAP",
            };

            _powerMeterDeviceConfig!.PowerMeterMapC37Config = _powerMeterMapC37Config;
            PowerMeter powerMeter = new PowerMeter(NullLogger.Instance, _powerMeterConfig!, publisher.Object, dataface);

            var expectedNames = new HashSet<string?>
            {
                // Power
                _powerMeterMapC37Config!.ActivePower,
                _powerMeterMapC37Config!.ReactivePower,
            };

            HashSet<string> registeredNames = dataface.Registers.Select(r => r.Name).ToHashSet();

            Assert.IsNotNull(powerMeter.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(2, dataface.Registers);
            Assert.HasCount(expectedNames.Count, registeredNames);
            CollectionAssert.AreEquivalent(expectedNames.ToList(), registeredNames.ToList());
        }

    }
}