// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.UnitTest.Equipments;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.PowerConversionSystems
{
    [TestClass]
    public class SolarPanelTest
    {
        private DerSolarUnit? _unit;
        private SolarPanelConfig? _solarConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            Der der = new Der(NullLogger.Instance, derConfig);
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
            DerGroup group = new DerGroup(NullLogger.Instance, groupConfig, der);
            DerCircuitConfig circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = groupConfig };
            DerCircuit circuit = new DerCircuit(NullLogger.Instance, circuitConfig, group);
            DerSolarUnitConfig unitConfig = new DerSolarUnitConfig { ChangedBy = "Test", Name = "DerUnitConfig", DerCircuitConfig = circuitConfig };
            _unit = new DerSolarUnit(NullLogger.Instance, unitConfig, circuit);
            SolarPanelDeviceConfig solarDeviceConfig = new SolarPanelDeviceConfig { ChangedBy = "Test", Name = "SolarPanelDeviceConfig", ClassName = "ClassName" };

            _solarConfig = new SolarPanelConfig
            {
                DeviceId = 1,
                IsActive = true,
                ChangedBy = "Test",
                Name = "SolarPanelConfig",
                DerUnitConfig = unitConfig,
                SolarPanelDeviceConfig = solarDeviceConfig
            };
        }


        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SolarPanel(NullLogger.Instance, null, _unit!, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SolarPanel(NullLogger.Instance, _solarConfig!, null, publisher.Object, dataface.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void RegisterDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            ModbusRegister dataface = new ModbusRegister("Test");
            SolarPanel solarPanel = new SolarPanel(NullLogger.Instance, _solarConfig!, _unit!, publisher.Object, dataface);

            Assert.IsNotNull(solarPanel.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(1, dataface.Registers);
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(solarPanel.SolarPanelDeviceTest)));
        }

    }
}
