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
            Assert.AreEqual(2000, config.MaximumRampIntervalMilliseconds);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-2000)]
        public void PowerRampConstraintMaximumRampIntervalNotPositiveTest(int maximumRampIntervalMilliseconds)
        {
            PowerRampConstraintConfig config = new PowerRampConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerRampConstraintConfig"
            };

            Action act = () => config.MaximumRampIntervalMilliseconds = maximumRampIntervalMilliseconds;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(5000)]
        public void PowerRampConstraintMaximumRampIntervalTest(int maximumRampIntervalMilliseconds)
        {
            PowerRampConstraintConfig config = new PowerRampConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerRampConstraintConfig"
            };

            config.MaximumRampIntervalMilliseconds = maximumRampIntervalMilliseconds;

            Assert.AreEqual(maximumRampIntervalMilliseconds, config.MaximumRampIntervalMilliseconds);
        }
    }
}
