// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Equipments.EnergyResources.Solars.Simples;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.EnergyResources.Solars.Simples
{
    [TestClass]
    public class SolarPanelSimpleV1ProxyTest
    {
        private Mock<DerSolarUnit>? _unit;
        private Mock<SolarPanelConfig>? _solarConfig;

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
            Mock<DerSolarUnitConfig> unitConfig = new Mock<DerSolarUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerSolarUnit");
            _unit = new Mock<DerSolarUnit>(NullLogger.Instance, unitConfig.Object, circuit.Object);
            // Device
            _solarConfig = new Mock<SolarPanelConfig>();
            _solarConfig.SetupGet(x => x.Name).Returns("SolarPanelConfig");
        }


        [TestMethod]
        public void CreateSolarPanelWithNullDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SolarPanelSimpleV1Proxy(NullLogger.Instance, _solarConfig!.Object, _unit!.Object, publisher.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateSolarPanelTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();

            SolarPanelSimpleV1Proxy sp = new SolarPanelSimpleV1Proxy(NullLogger.Instance, _solarConfig!.Object, _unit!.Object, publisher.Object, dataface.Object);

            Assert.AreEqual(_solarConfig.Object.Name, sp.Name);
        }

    }
}
