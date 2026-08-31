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
                MinimumActivePowerKiloWatt = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MaximumActivePowerKiloWatt = -1);
        }


        [TestMethod]
        public void PowerConstraintMinBiggerThanMaxActiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MaximumActivePowerKiloWatt = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MinimumActivePowerKiloWatt = 1);
        }


        [TestMethod]
        public void PowerConstraintMaxActiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MinimumActivePowerKiloWatt = -10,
                MaximumActivePowerKiloWatt = 10
            };

            Assert.AreEqual(10, config.MaximumActivePowerKiloWatt);
            Assert.AreEqual(-10, config.MinimumActivePowerKiloWatt);
            Assert.IsNull(config.MaximumReactivePowerKiloVars);
            Assert.IsNull(config.MinimumReactivePowerKiloVars);
        }


        [TestMethod]
        public void PowerConstraintMaxSmallerThanMinReactiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MinimumReactivePowerKiloVars = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MaximumReactivePowerKiloVars = -1);
        }


        [TestMethod]
        public void PowerConstraintMinBiggerThanMaxReactiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MaximumReactivePowerKiloVars = 0
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MinimumReactivePowerKiloVars = 1);
        }


        [TestMethod]
        public void PowerConstraintMaxReactiveTest()
        {
            PowerConstraintConfig config = new PowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerConstraintConfig",
                MinimumReactivePowerKiloVars = -10,
                MaximumReactivePowerKiloVars = 10
            };

            Assert.AreEqual(10, config.MaximumReactivePowerKiloVars);
            Assert.AreEqual(-10, config.MinimumReactivePowerKiloVars);
            Assert.IsNull(config.MaximumActivePowerKiloWatt);
            Assert.IsNull(config.MinimumActivePowerKiloWatt);
        }
    }
}
