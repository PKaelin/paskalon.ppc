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
    public sealed class PowerControlContextTest : DatabaseTestBase
    {
        [TestMethod]
        public async Task CreatePowerControlContext()
        {
            await using PowerControlContext context = CreateDbContext<PowerControlContext>();
            await context.Database.EnsureDeletedAsync();
            bool created = await context.Database.EnsureCreatedAsync();

            Assert.IsTrue(created);
        }


        [TestMethod]
        public async Task CreatePowerControlMigrationContext()
        {
            await using PowerControlContext context = CreateDbContext<PowerControlContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }



        [TestMethod]
        public async Task CreateBessTest()
        {
            SimpleSetBess sample = new SimpleSetBess();

            await using (PowerControlContext context = CreateDbContext<PowerControlContext>())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                // Constraints
                context.SystemPowerConstraintConfigs.Add(sample.SystemPowerConstraintConfig!);
                context.SystemRampConstraintConfigs.Add(sample.SystemRampConstraintConfig!);
                context.DerUnitPowerConstraintConfigs.Add(sample.DerUnitPowerConstraintConfig!);
                context.DerUnitRampConstraintConfig.Add(sample.DerUnitRampConstraintConfig!);
                // Power controls
                context.SystemPowerControlConfigs.Add(sample.SystemPowerControlConfig!);
                context.DerUnitPowerControlConfigs.Add(sample.DerUnitPowerControlConfig!);
                context.DerUnitEnergyStoragePowerControlConfigs.Add(sample.DerUnitEnergyStoragePowerControlConfig!);

                await context.SaveChangesAsync();
            }

            await using (PowerControlContext context = CreateDbContext<PowerControlContext>())
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
