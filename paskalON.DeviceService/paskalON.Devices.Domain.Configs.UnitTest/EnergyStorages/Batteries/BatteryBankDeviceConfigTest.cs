// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;

namespace paskalON.Devices.Domain.Configs.UnitTest.EnergyStorages.Batteries
{
    [TestClass]
    public class BatteryBankDeviceConfigTest
    {
        private const string AbsoluteMin = nameof(BatteryBankDeviceConfig.AbsoluteMinimumStateOfCharge);
        private const string AbsoluteMax = nameof(BatteryBankDeviceConfig.AbsoluteMaximumStateOfCharge);
        private const string UsableMin = nameof(BatteryBankDeviceConfig.UsableMinimumStateOfCharge);
        private const string UsableMax = nameof(BatteryBankDeviceConfig.UsableMaximumStateOfCharge);
        private const string PreferredMin = nameof(BatteryBankDeviceConfig.PreferredMinimumStateOfCharge);
        private const string PreferredMax = nameof(BatteryBankDeviceConfig.PreferredMaximumStateOfCharge);
        private const string PreferredMinTemperature = nameof(BatteryBankDeviceConfig.PreferredMinimumTemperature);
        private const string PreferredMaxTemperature = nameof(BatteryBankDeviceConfig.PreferredMaximumTemperature);


        [TestMethod]
        public void StateOfChargeDefaultWindowsNestedTest()
        {
            BatteryBankDeviceConfig config = CreateConfig();

            double[] window = GetStateOfChargeWindow(config);

            CollectionAssert.AreEqual(new double[] { 0, 10, 20, 80, 90, 100 }, window);
            Assert.IsTrue(config.ZeroCapacityOnCommLoss);
            Assert.IsEmpty(config.Customs);
        }


        [TestMethod]
        [DataRow(AbsoluteMin, 0)]
        [DataRow(AbsoluteMin, 10)]
        [DataRow(AbsoluteMax, 100)]
        [DataRow(AbsoluteMax, 90)]
        [DataRow(UsableMin, 0)]
        [DataRow(UsableMin, 20)]
        [DataRow(UsableMax, 80)]
        [DataRow(UsableMax, 100)]
        [DataRow(PreferredMin, 10)]
        [DataRow(PreferredMin, 80)]
        [DataRow(PreferredMax, 20)]
        [DataRow(PreferredMax, 90)]
        public void StateOfChargeAtWindowLimitTest(string propertyName, double value)
        {
            BatteryBankDeviceConfig config = CreateConfig();

            PropertyAccessor.Set(config, propertyName, value);

            Assert.AreEqual(value, PropertyAccessor.Get(config, propertyName));
        }


        [TestMethod]
        [DataRow(AbsoluteMin, -0.1)]
        [DataRow(AbsoluteMax, 100.1)]
        [DataRow(UsableMin, -0.1)]
        [DataRow(UsableMax, 100.1)]
        [DataRow(PreferredMin, -0.1)]
        [DataRow(PreferredMax, 100.1)]
        public void StateOfChargeOutsidePercentRangeTest(string propertyName, double value)
        {
            BatteryBankDeviceConfig config = CreateConfig();
            object? original = PropertyAccessor.Get(config, propertyName);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PropertyAccessor.Set(config, propertyName, value));

            Assert.AreEqual(original, PropertyAccessor.Get(config, propertyName));
        }


        [TestMethod]
        [DataRow(UsableMin, 9.9, AbsoluteMin, 10)]
        [DataRow(UsableMax, 90.1, AbsoluteMax, 90)]
        [DataRow(PreferredMin, 9.9, UsableMin, 10)]
        [DataRow(PreferredMax, 90.1, UsableMax, 90)]
        public void StateOfChargeOutsideOuterWindowTest(string propertyName, double value, string outerPropertyName, double outerValue)
        {
            BatteryBankDeviceConfig config = CreateConfig();
            PropertyAccessor.Set(config, outerPropertyName, outerValue);

            Action set = () => PropertyAccessor.Set(config, propertyName, value);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
        }


        [TestMethod]
        [DataRow(AbsoluteMin, 100.1)]
        [DataRow(AbsoluteMax, -1)]
        [DataRow(UsableMin, 90.1)]
        [DataRow(UsableMax, 100.9)]
        [DataRow(PreferredMin, 80.1)]
        [DataRow(PreferredMax, 19.9)]
        public void StateOfChargeInvertsNestedWindowTest(string propertyName, double value)
        {
            BatteryBankDeviceConfig config = CreateConfig();
            object? original = PropertyAccessor.Get(config, propertyName);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PropertyAccessor.Set(config, propertyName, value));

            Assert.AreEqual(original, PropertyAccessor.Get(config, propertyName));
        }


        [TestMethod]
        public void TemperatureWithinAbsoluteLimitsTest()
        {
            BatteryBankDeviceConfig config = CreateConfig();

            config.PreferredMaximumTemperature = 40;
            config.PreferredMinimumTemperature = 15;

            Assert.AreEqual(40, config.PreferredMaximumTemperature);
            Assert.AreEqual(15, config.PreferredMinimumTemperature);
        }


        [TestMethod]
        [DataRow(PreferredMinTemperature, -0.1)]
        [DataRow(PreferredMaxTemperature, 60.1)]
        public void TemperatureOutsideAbsoluteLimitsTest(string propertyName, double value)
        {
            BatteryBankDeviceConfig config = CreateConfig();

            Action set = () => PropertyAccessor.Set(config, propertyName, value);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
        }


        [TestMethod]
        public void TemperaturePreferredMinimumAbovePreferredMaximumTest()
        {
            BatteryBankDeviceConfig config = CreateConfig();
            config.PreferredMaximumTemperature = 40;

            Action set = () => config.PreferredMinimumTemperature = 45;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
        }


        private BatteryBankDeviceConfig CreateConfig()
        {
            BatteryBankDeviceConfig config = new BatteryBankDeviceConfig
            {
                ChangedBy = TestConfigFactory.ChangedBy,
                Name = "Device Battery 1",
                ClassName = "BatteryBankSimpleV1Proxy",
                BatteryType = BatteryType.LithiumIon,
                NameplateCapacity = 1000000,
                NameplateMaximumChargeRate = 250000,
                NameplateMaximumDischargeRate = 250000,
                AbsoluteMinimumTemperature = 0,
                AbsoluteMaximumTemperature = 60
            };

            return config;
        }


        private double[] GetStateOfChargeWindow(BatteryBankDeviceConfig config)
        {
            return
            [
                config.AbsoluteMinimumStateOfCharge,
                config.UsableMinimumStateOfCharge,
                config.PreferredMinimumStateOfCharge,
                config.PreferredMaximumStateOfCharge,
                config.UsableMaximumStateOfCharge,
                config.AbsoluteMaximumStateOfCharge
            ];
        }
    }
}
