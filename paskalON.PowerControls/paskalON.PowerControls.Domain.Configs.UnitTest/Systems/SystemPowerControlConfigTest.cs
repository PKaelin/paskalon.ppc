// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;

namespace paskalON.PowerControls.Domain.Configs.UnitTest.Systems
{
    [TestClass]
    public class SystemPowerControlConfigTest
    {
        [TestMethod]
        public void SystemPowerControlConfigAddConstraintsTest()
        {
            Mock<DerUnitPowerConstraintConfig> constraint = new Mock<DerUnitPowerConstraintConfig>();
            constraint.Object.Name = "Constraint1";

            SystemPowerControlConfig config = new SystemPowerControlConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerControlConfig",
                IsActive = true,
                IsEnabled = true,
            };

            config.Constraints.Add(constraint.Object);

            Assert.HasCount(1, config.Constraints);
            Assert.IsNotNull(config.Constraints.FirstOrDefault(c => c.Name == constraint.Object.Name));
        }
    }
}
