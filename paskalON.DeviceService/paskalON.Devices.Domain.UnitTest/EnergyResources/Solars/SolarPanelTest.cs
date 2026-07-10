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
        private DerConfig? _derConfig;
        private Der? _der;
        private DerGroupConfig? _groupConfig;
        private DerGroup? _group;
        private DerCircuitConfig? _circuitConfig;
        private DerCircuit? _circuit;
        private DerSolarUnitConfig? _unitConfig;
        private DerSolarUnit? _unit;
        private SolarPanelDeviceConfig? _solarDeviceConfig;
        private SolarPanelConfig? _solarConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            _derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            _der = new Der(NullLogger.Instance, _derConfig);
            _groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = _derConfig };
            _group = new DerGroup(NullLogger.Instance, _groupConfig, _der);
            _circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = _groupConfig };
            _circuit = new DerCircuit(NullLogger.Instance, _circuitConfig, _group);
            _unitConfig = new DerSolarUnitConfig { ChangedBy = "Test", Name = "DerUnitConfig", DerCircuitConfig = _circuitConfig };
            _unit = new DerSolarUnit(NullLogger.Instance, _unitConfig, _circuit);

            _solarDeviceConfig = new SolarPanelDeviceConfig { ChangedBy = "Test", Name = "SolarPanelDeviceConfig", ClassName = "ClassName" };

            _solarConfig = new SolarPanelConfig
            {
                DeviceId = 1,
                IsActive = true,
                ChangedBy = "Test",
                Name = "SolarPanelConfig",
                DerUnitConfig = _unitConfig,
                SolarPanelDeviceConfig = _solarDeviceConfig
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
            ModbusRegister dataface = new ModbusRegister();
            SolarPanel solarPanel = new SolarPanel(NullLogger.Instance, _solarConfig!, _unit!, publisher.Object, dataface);

            Assert.IsNotNull(solarPanel.Dataface);
            Assert.IsNotNull(dataface.Registers);
            Assert.HasCount(1, dataface.Registers);
            Assert.IsNotNull(dataface.Registers.FirstOrDefault(r => r.Name == nameof(solarPanel.SolarPanelDeviceTest)));
        }

    }
}
