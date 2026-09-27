// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Devices.Infrastructure.Storage.Repositories;

namespace paskalON.Devices.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public class DerRepositoryTest : DatabaseTestBase
    {
        [TestMethod]
        public async Task VersionRepositoryEmptyTableTest()
        {
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            DerRepository repository = new DerRepository(NullLogger<DerRepository>.Instance, context);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await repository.GetDer());
        }


        [TestMethod]
        public async Task VersionRepositoryDerOnlyTableTest()
        {
            DerConfig? der = null;

            await using (DeviceServiceContext context = CreateDbContext<DeviceServiceContext>())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                context.DerConfigs.Add(new DerConfig { ChangedBy = "Test", Name = "DerConfig" });
                context.SaveChanges();
            }

            await using (DeviceServiceContext context = CreateDbContext<DeviceServiceContext>())
            {
                DerRepository repository = new DerRepository(NullLogger<DerRepository>.Instance, context);
                der = await repository.GetDer();
            }

            Assert.IsNotNull(der);
            Assert.HasCount(0, der.DerGroupConfigs);
            Assert.HasCount(0, der.AuxiliaryPowerMeterConfigs);
            Assert.HasCount(0, der.SystemPowerMeterConfigs);
            Assert.HasCount(0, der.ExternalPowerMeterConfigs);
        }
    }
}
