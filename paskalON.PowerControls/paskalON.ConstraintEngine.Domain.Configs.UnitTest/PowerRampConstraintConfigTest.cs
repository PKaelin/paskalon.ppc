// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
                    MaximumActivePowerKiloWattRampRatePerSecond = -1
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
                    MaximumReactivePowerKiloVarsRampRatePerSecond = -1
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
                MaximumActivePowerKiloWattRampRatePerSecond = 1,
                MaximumReactivePowerKiloVarsRampRatePerSecond = 2
            };

            Assert.AreEqual(1, config.MaximumActivePowerKiloWattRampRatePerSecond);
            Assert.AreEqual(2, config.MaximumReactivePowerKiloVarsRampRatePerSecond);
        }
    }
}
