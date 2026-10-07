// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;

namespace paskalON.Devices.Domain.Configs.UnitTest.EnergyResources.Solars
{
    [TestClass]
    public class SolarPanelDeviceConfigTest
    {
        [TestMethod]
        [DataRow(0, 40.8, 0, 12.87)]
        [DataRow(10, 50, 0, 20)]
        [DataRow(30, 30, 8.5, 8.5)]
        public void ValidateWithMinimumNotGreaterThanMaximumTest(double minimumVoltage, double maximumVoltage, double minimumCurrent, double maximumCurrent)
        {
            SolarPanelDeviceConfig config = CreateConfig(minimumVoltage, maximumVoltage, minimumCurrent, maximumCurrent);

            Assert.AreEqual(minimumVoltage, config.MinimumVoltage);
            Assert.AreEqual(maximumVoltage, config.MaximumVoltage);
            Assert.AreEqual(minimumCurrent, config.MinimumCurrent);
            Assert.AreEqual(maximumCurrent, config.MaximumCurrent);
        }


        [TestMethod]
        [DataRow(50.1, 50, 0, 20, "MinimumVoltage")]
        [DataRow(10, 0, 0, 20, "MinimumVoltage")]
        [DataRow(10, 50, 20.1, 20, "MinimumCurrent")]
        [DataRow(10, 50, 5, 0, "MinimumCurrent")]
        public void ValidateWithMinimumGreaterThanMaximumTest(double minimumVoltage, double maximumVoltage, double minimumCurrent, double maximumCurrent, string expectedParamName)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CreateConfig(minimumVoltage, maximumVoltage, minimumCurrent, maximumCurrent));
        }


        private SolarPanelDeviceConfig CreateConfig(double minimumVoltage, double maximumVoltage, double minimumCurrent, double maximumCurrent)
        {
            SolarPanelDeviceConfig config = new SolarPanelDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device PV 1",
                ClassName = "SolarPanelSimpleV1Proxy",
                MaximumVoltage = maximumVoltage,
                MinimumVoltage = minimumVoltage,
                MaximumCurrent = maximumCurrent,
                MinimumCurrent = minimumCurrent
            };

            return config;
        }
    }
}
