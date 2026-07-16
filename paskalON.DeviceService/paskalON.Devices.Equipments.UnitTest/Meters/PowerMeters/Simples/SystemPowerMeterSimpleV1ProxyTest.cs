// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Equipments.C37;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Protocols.C37118;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.Meters.PowerMeters.Simples
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
        public void CreatePowerMeterWithNullClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreatePowerMeterWithMockedClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Mock<IC37Client> client = new Mock<IC37Client>();

            SystemPowerMeterSimpleV1Proxy pm = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_pmConfig!.Name, pm.Name);
        }


        [TestMethod]
        public async Task PowerMeterConnectTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Mock<IC37Client> client = new Mock<IC37Client>();
            client.Setup(x => x.SendCommandAsync(C37CommandType.TurnOnTransmission));

            SystemPowerMeterSimpleV1Proxy pm = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, client.Object);

            await pm.ConnectAsync();

            client.Verify(x => x.SendCommandAsync(It.IsAny<C37CommandType>()), Times.Once);
            Assert.AreEqual(PowerMeterState.Connecting, pm.State);
        }


        [TestMethod]
        public async Task PowerMeterDisconnectTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Mock<IC37Client> client = new Mock<IC37Client>();
            client.Setup(x => x.SendCommandAsync(C37CommandType.TurnOffTransmission));

            SystemPowerMeterSimpleV1Proxy pm = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, client.Object);

            await pm.DisconnectAsync();

            client.Verify(x => x.SendCommandAsync(It.IsAny<C37CommandType>()), Times.Once);
            Assert.AreEqual(PowerMeterState.Disconnecting, pm.State);
        }


        [TestMethod]
        public async Task PowerMeterTransmisionTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            Mock<IC37Client> client = new Mock<IC37Client>();

            C37TransmissionEngine engine = new C37TransmissionEngine(NullLogger.Instance, client.Object, dataface);
        }
    }
}
