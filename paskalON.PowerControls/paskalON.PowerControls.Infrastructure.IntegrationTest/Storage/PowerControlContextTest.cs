// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.PowerControls.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.PowerControls.Infrastructure.Storage;

namespace paskalON.PowerControls.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public sealed class PowerControlContextTest
    {
        private DbContextOptions<PowerControlContext>? _options;

        [TestInitialize]
        public void Initialize()
        {
            string variable = "DB_CONNECTION_STRING";
            string? connectionString = Environment.GetEnvironmentVariable(variable);
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
            _options = new DbContextOptionsBuilder<PowerControlContext>().UseNpgsql(connectionString).Options;
        }


        [TestMethod]
        public void CreatePowerControlContext()
        {
            using PowerControlContext context = new PowerControlContext(_options!);
            context.Database.EnsureDeleted();
            bool created = context.Database.EnsureCreated();

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void CreateBessTest()
        {
            SimpleSetBess sample = new SimpleSetBess();

            using (PowerControlContext context = new PowerControlContext(_options!))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Constraints
                context.SystemPowerConstraintConfigs.Add(sample.SystemPowerConstraintConfig!);
                context.SystemRampConstraintConfigs.Add(sample.SystemRampConstraintConfig!);
                context.DerUnitPowerConstraintConfigs.Add(sample.DerUnitPowerConstraintConfig!);
                context.DerUnitRampConstraintConfig.Add(sample.DerUnitRampConstraintConfig!);
                // Power controls
                context.SystemPowerControlConfigs.Add(sample.SystemPowerControlConfig!);
                context.DerUnitPowerControlConfigs.Add(sample.DerUnitPowerControlConfig!);
                context.DerUnitEnergyStoragePowerControlConfigs.Add(sample.DerUnitEnergyStoragePowerControlConfig!);

                context.SaveChanges();
            }

            using (PowerControlContext context = new PowerControlContext(_options!))
            {
                Assert.AreEqual(1, context.SystemPowerConstraintConfigs.Count());
                Assert.AreEqual(1, context.SystemRampConstraintConfigs.Count());
                Assert.AreEqual(1, context.DerUnitPowerConstraintConfigs.Count());
                Assert.AreEqual(1, context.DerUnitRampConstraintConfig.Count());
                Assert.AreEqual(1, context.SystemPowerControlConfigs.Count());
                Assert.AreEqual(1, context.DerUnitPowerControlConfigs.Count());
                Assert.AreEqual(1, context.DerUnitEnergyStoragePowerControlConfigs.Count());
            }
        }
    }
}
