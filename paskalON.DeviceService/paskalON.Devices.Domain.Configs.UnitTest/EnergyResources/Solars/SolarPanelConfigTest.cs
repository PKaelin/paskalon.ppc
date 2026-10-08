// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;

namespace paskalON.Devices.Domain.Configs.UnitTest.EnergyResources.Solars
{
    [TestClass]
    public class SolarPanelConfigTest
    {
        private const double Delta = 1e-9;


        [TestMethod]
        [DataRow(SolarConnectionType.Series, 20, 10 * 20.0, 40.8 * 20, 1.5, 12.87)]
        [DataRow(SolarConnectionType.Parallel, 20, 10, 40.8, 1.5 * 20, 12.87 * 20)]
        [DataRow(SolarConnectionType.Series, 1, 10, 40.8, 1.5, 12.87)]
        [DataRow(SolarConnectionType.Parallel, 1, 10, 40.8, 1.5, 12.87)]
        public void SumsByConnectionTypeTest(SolarConnectionType connectionType, int numberOfPanels,
            double expectedMinimumVoltage, double expectedMaximumVoltage, double expectedMinimumCurrent, double expectedMaximumCurrent)
        {
            SolarPanelConfig config = CreateConfig(connectionType, numberOfPanels);

            double minimumVoltage = config.MinimumVoltageSum;
            double maximumVoltage = config.MaximumVoltageSum;
            double minimumCurrent = config.MinimumCurrentSum;
            double maximumCurrent = config.MaximumCurrentSum;

            Assert.AreEqual(expectedMinimumVoltage, minimumVoltage, Delta);
            Assert.AreEqual(expectedMaximumVoltage, maximumVoltage, Delta);
            Assert.AreEqual(expectedMinimumCurrent, minimumCurrent, Delta);
            Assert.AreEqual(expectedMaximumCurrent, maximumCurrent, Delta);
        }


        [TestMethod]
        public void SumsWithoutDeviceConfigTest()
        {
            SolarPanelConfig config = CreateConfig(SolarConnectionType.Series, 20);
            config.SolarPanelDeviceConfig = null!;

            double[] sums = [config.MinimumVoltageSum, config.MaximumVoltageSum, config.MinimumCurrentSum, config.MaximumCurrentSum];

            CollectionAssert.AreEqual(new double[] { 0, 0, 0, 0 }, sums);
        }


        [TestMethod]
        public void NumberOfPanelsDefaultTest()
        {
            SolarPanelDeviceConfig deviceConfig = TestConfigFactory.CreateSolarPanelDeviceConfig();

            SolarPanelConfig config = new SolarPanelConfig
            {
                ChangedBy = TestConfigFactory.ChangedBy,
                Name = "PV 1",
                DeviceId = 1,
                IsActive = true,
                DerUnitConfig = TestConfigFactory.CreateBatteryStorageUnitConfig(),
                SolarPanelDeviceConfig = deviceConfig
            };

            Assert.AreEqual(0, config.NumberOfPanels);
            Assert.AreEqual(SolarConnectionType.Series, config.ConnectionType);
            Assert.AreEqual(0, config.MaximumVoltageSum);
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void NumberOfPanelsNegativeTest(int numberOfPanels)
        {
            SolarPanelConfig config = CreateConfig(SolarConnectionType.Series, 20);

            Action set = () => config.NumberOfPanels = numberOfPanels;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
        }


        private SolarPanelConfig CreateConfig(SolarConnectionType connectionType, int numberOfPanels)
        {
            SolarPanelDeviceConfig deviceConfig = TestConfigFactory.CreateSolarPanelDeviceConfig();
            deviceConfig.MaximumVoltage = 40.8;
            deviceConfig.MinimumVoltage = 10;
            deviceConfig.MaximumCurrent = 12.87;
            deviceConfig.MinimumCurrent = 1.5;

            SolarPanelConfig config = new SolarPanelConfig
            {
                ChangedBy = TestConfigFactory.ChangedBy,
                Name = "PV 1",
                DeviceId = 1,
                IsActive = true,
                DerUnitConfig = TestConfigFactory.CreateBatteryStorageUnitConfig(),
                SolarPanelDeviceConfig = deviceConfig,
                ConnectionType = connectionType,
                NumberOfPanels = numberOfPanels
            };

            return config;
        }
    }
}
