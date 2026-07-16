// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Equipments.C37;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Protocols.C37118;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.IntegrationTest.Meters.PowerMeters.Simples
{
    [TestClass]
    public class SystemPowerMeterSimpleV1ProxyTest
    {
        private SystemPowerMeterConfig? _pmConfig;
        private PowerMeterMapC37Config? _powerMeterMapC37Config;


        [TestInitialize]
        public void TestInitialize()
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");

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
                PowerFactor = "Analog9",
                // Voltage
                VoltageA = "Phasor0",
                VoltageB = "Phasor1",
                VoltageC = "Phasor2",
                VoltageAB = "Phasor3",
                VoltageBC = "Phasor4",
                VoltageCA = "Phasor5",
                VoltagePositiveSequence = "Phasor6",
                VoltageLLAvg = "Analog10",
                // Current
                CurrentA = "Phasor7",
                CurrentB = "Phasor8",
                CurrentC = "Phasor9",
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
                DerConfig = derConfig.Object,
                PowerMeterDeviceConfig = powerMeterDeviceConfig
            };
        }


        [TestMethod]
        public async Task PowerMeterTransmissionTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            Mock<IC37Client> client = new Mock<IC37Client>();

            C37TransmissionEngine engine = new C37TransmissionEngine(NullLogger.Instance, client.Object, dataface);
        }

    }
}
