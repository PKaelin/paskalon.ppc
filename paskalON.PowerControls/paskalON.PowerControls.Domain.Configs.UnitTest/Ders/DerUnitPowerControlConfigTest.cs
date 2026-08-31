// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Strategies;

namespace paskalON.PowerControls.Domain.Configs.UnitTest.Ders
{
    [TestClass]
    public sealed class DerUnitPowerControlConfigTest
    {
        [TestMethod]
        public void DerUnitPowerControlConfigWeightPriorityTest()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DerUnitPowerControlConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerControlConfig",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal,
                DerUnitName = "Unit1",
                Weight = -1,
            });
        }


        [TestMethod]
        public void DerUnitPowerControlConfigConstructorTest()
        {
            DerUnitPowerControlConfig config = new DerUnitPowerControlConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerControlConfig",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal,
                DerUnitName = "Unit1",
                Weight = 1.5,
                Priority = 2
            };

            Assert.AreEqual(1.5, config.Weight);
            Assert.AreEqual((ushort)2, config.Priority);
        }


        [TestMethod]
        public void DerUnitPowerControlConfigAddConstraintsTest()
        {
            Mock<DerUnitPowerConstraintConfig> constraint = new Mock<DerUnitPowerConstraintConfig>();
            constraint.Object.Name = "Constraint1";

            DerUnitPowerControlConfig config = new DerUnitPowerControlConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerControlConfig",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal,
                DerUnitName = "Unit1",
            };

            config.Constraints.Add(constraint.Object);

            Assert.HasCount(1, config.Constraints);
            Assert.IsNotNull(config.Constraints.FirstOrDefault(c => c.Name == constraint.Object.Name));
        }
    }
}
