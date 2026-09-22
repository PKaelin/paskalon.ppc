// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs.UnitTest
{
    [TestClass]
    public sealed class PowerConstraintConfigTest
    {
        [TestMethod]
        public void PowerConstraintMaxSmallerThanMinActiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MinimumActivePowerWatt = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MaximumActivePowerWatt = -1);
        }


        [TestMethod]
        public void PowerConstraintMinBiggerThanMaxActiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MaximumActivePowerWatt = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MinimumActivePowerWatt = 1);
        }


        [TestMethod]
        public void PowerConstraintMaxActiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MinimumActivePowerWatt = -10,
                MaximumActivePowerWatt = 10
            };

            Assert.AreEqual(10, config.MaximumActivePowerWatt);
            Assert.AreEqual(-10, config.MinimumActivePowerWatt);
            Assert.IsNull(config.MaximumReactivePowerVars);
            Assert.IsNull(config.MinimumReactivePowerVars);
        }


        [TestMethod]
        public void PowerConstraintMaxSmallerThanMinReactiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MinimumReactivePowerVars = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MaximumReactivePowerVars = -1);
        }


        [TestMethod]
        public void PowerConstraintMinBiggerThanMaxReactiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MaximumReactivePowerVars = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MinimumReactivePowerVars = 1);
        }


        [TestMethod]
        public void PowerConstraintMaxReactiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MinimumReactivePowerVars = -10,
                MaximumReactivePowerVars = 10
            };

            Assert.AreEqual(10, config.MaximumReactivePowerVars);
            Assert.AreEqual(-10, config.MinimumReactivePowerVars);
            Assert.IsNull(config.MaximumActivePowerWatt);
            Assert.IsNull(config.MinimumActivePowerWatt);
        }
    }
}
