// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Domain.Configs.UnitTest.PowerConversionSystems
{
    [TestClass]
    public class PowerConversionSystemDeviceConfigTest
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(0.5)]
        [DataRow(250000)]
        public void StandbyActivePowerWattsValidTest(double standby)
        {
            PowerConversionSystemDeviceConfig config = CreateConfig();

            config.StandbyActivePowerWatts = standby;

            Assert.AreEqual(standby, config.StandbyActivePowerWatts);
        }


        [TestMethod]
        [DataRow(-0.001)]
        [DataRow(-250000)]
        public void StandbyActivePowerWattsNegativeTest(double standby)
        {
            PowerConversionSystemDeviceConfig config = CreateConfig();

            Action set = () => config.StandbyActivePowerWatts = standby;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
            Assert.AreEqual(0, config.StandbyActivePowerWatts);
        }


        [TestMethod]
        public void DefaultsTest()
        {
            PowerConversionSystemDeviceConfig config = CreateConfig();

            bool zeroOutput = config.ZeroOutputOnCommLoss;

            Assert.IsTrue(zeroOutput);
            Assert.IsNotNull(config.Customs);
            Assert.IsEmpty(config.Customs);
        }


        private static PowerConversionSystemDeviceConfig CreateConfig()
        {
            PowerConversionSystemDeviceConfig config = new PowerConversionSystemDeviceConfig
            {
                ChangedBy = TestConfigFactory.ChangedBy,
                Name = "Device PCS 1",
                ClassName = "PcsSimpleV1Proxy",
                NameplateMaximumActivePower = 250000,
                NameplateMaximumReactivePower = 125000,
                NameplateMaximumApparentPower = 280000
            };

            return config;
        }
    }
}
