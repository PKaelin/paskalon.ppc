// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs.UnitTest
{
    [TestClass]
    public class PowerRampConstraintConfigTest
    {
        [TestMethod]
        public void PowerRampConstraintMaxNegativeActiveTest()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                new PowerRampConstraintConfig
                {
                    ChangedBy = "Test",
                    Name = "PowerRampConstraintConfig",
                    MaximumActivePowerWattRampRatePerSecond = -1
                };
            });
        }


        [TestMethod]
        public void PowerRampConstraintMaxNegativeReactiveTest()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                new PowerRampConstraintConfig
                {
                    ChangedBy = "Test",
                    Name = "PowerRampConstraintConfig",
                    MaximumReactivePowerVarsRampRatePerSecond = -1
                };
            });
        }


        [TestMethod]
        public void PowerRampConstraintTest()
        {
            PowerRampConstraintConfig config = new PowerRampConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerRampConstraintConfig",
                MaximumActivePowerWattRampRatePerSecond = 1,
                MaximumReactivePowerVarsRampRatePerSecond = 2
            };

            Assert.AreEqual(1, config.MaximumActivePowerWattRampRatePerSecond);
            Assert.AreEqual(2, config.MaximumReactivePowerVarsRampRatePerSecond);
        }
    }
}
