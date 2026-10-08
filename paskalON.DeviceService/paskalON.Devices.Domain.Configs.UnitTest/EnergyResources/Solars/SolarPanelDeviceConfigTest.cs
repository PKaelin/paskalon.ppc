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
        private const string MinimumVoltage = nameof(SolarPanelDeviceConfig.MinimumVoltage);
        private const string MaximumVoltage = nameof(SolarPanelDeviceConfig.MaximumVoltage);
        private const string MinimumCurrent = nameof(SolarPanelDeviceConfig.MinimumCurrent);
        private const string MaximumCurrent = nameof(SolarPanelDeviceConfig.MaximumCurrent);


        [TestMethod]
        [DataRow(0, 40.8, 0, 12.87)]
        [DataRow(10, 50, 0, 20)]
        [DataRow(30, 30, 8.5, 8.5)]
        [DataRow(0, 0, 0, 0)]
        public void SetMinimumNotGreaterThanMaximumTest(double minimumVoltage, double maximumVoltage, double minimumCurrent, double maximumCurrent)
        {
            SolarPanelDeviceConfig config = CreateConfig(minimumVoltage, maximumVoltage, minimumCurrent, maximumCurrent);

            Assert.AreEqual(minimumVoltage, config.MinimumVoltage);
            Assert.AreEqual(maximumVoltage, config.MaximumVoltage);
            Assert.AreEqual(minimumCurrent, config.MinimumCurrent);
            Assert.AreEqual(maximumCurrent, config.MaximumCurrent);
        }


        [TestMethod]
        [DataRow(50.1, 50, 0, 20, MinimumVoltage)]
        [DataRow(10, 0, 0, 20, MinimumVoltage)]
        [DataRow(10, 50, 20.1, 20, MinimumCurrent)]
        [DataRow(10, 50, 5, 0, MinimumCurrent)]
        public void SetMinimumGreaterThanMaximumTest(double minimumVoltage, double maximumVoltage, double minimumCurrent, double maximumCurrent, string expectedProperty)
        {
            Func<SolarPanelDeviceConfig> create = () => CreateConfig(minimumVoltage, maximumVoltage, minimumCurrent, maximumCurrent);

            ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => create());

            Assert.Contains($"{expectedProperty} cannot be greater than", exception.Message);
        }


        [TestMethod]
        [DataRow(MaximumVoltage, 9.9)]
        [DataRow(MaximumCurrent, 1.9)]
        public void SetMaximumLessThanMinimumTest(string propertyName, double maximum)
        {
            SolarPanelDeviceConfig config = CreateConfig(10, 50, 2, 20);

            ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SetProperty(config, propertyName, maximum));

            Assert.Contains($"{propertyName} cannot be less than", exception.Message);
        }


        [TestMethod]
        [DataRow(MinimumVoltage)]
        [DataRow(MaximumVoltage)]
        [DataRow(MinimumCurrent)]
        [DataRow(MaximumCurrent)]
        public void SetNegativeValueTest(string propertyName)
        {
            SolarPanelDeviceConfig config = CreateConfig(10, 50, 2, 20);

            Action set = () => SetProperty(config, propertyName, -0.1);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
        }


        private SolarPanelDeviceConfig CreateConfig(double minimumVoltage, double maximumVoltage, double minimumCurrent, double maximumCurrent)
        {
            SolarPanelDeviceConfig config = new SolarPanelDeviceConfig
            {
                ChangedBy = TestConfigFactory.ChangedBy,
                Name = "Device PV 1",
                ClassName = "SolarPanelSimpleV1Proxy",
                MaximumVoltage = maximumVoltage,
                MinimumVoltage = minimumVoltage,
                MaximumCurrent = maximumCurrent,
                MinimumCurrent = minimumCurrent
            };

            return config;
        }


        private void SetProperty(SolarPanelDeviceConfig config, string propertyName, double value)
        {
            switch (propertyName)
            {
                case MinimumVoltage:
                    config.MinimumVoltage = value;
                    break;
                case MaximumVoltage:
                    config.MaximumVoltage = value;
                    break;
                case MinimumCurrent:
                    config.MinimumCurrent = value;
                    break;
                case MaximumCurrent:
                    config.MaximumCurrent = value;
                    break;
                default:
                    throw new ArgumentException($"Unknown property {propertyName}.", nameof(propertyName));
            }
        }


        private static double GetProperty(SolarPanelDeviceConfig config, string propertyName)
        {
            return propertyName switch
            {
                MinimumVoltage => config.MinimumVoltage,
                MaximumVoltage => config.MaximumVoltage,
                MinimumCurrent => config.MinimumCurrent,
                MaximumCurrent => config.MaximumCurrent,
                _ => throw new ArgumentException($"Unknown property {propertyName}.", nameof(propertyName))
            };
        }
    }
}
