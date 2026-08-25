// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.Devices.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public class DerRepositoryTest
    {
        [TestMethod]
        public async Task VersionRepositoryEmptyTableTest()
        {
            using DeviceServiceContext context = new DeviceServiceContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            DerRepository repository = new DerRepository(NullLogger.Instance, context);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await repository.GetDer());
        }


        [TestMethod]
        public async Task VersionRepositoryDerOnlyTableTest()
        {
            DerConfig? der = null;

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                context.DerConfigs.Add(new DerConfig { ChangedBy = "Test", Name = "DerConfig" });
                context.SaveChanges();
            }

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                DerRepository repository = new DerRepository(NullLogger.Instance, context);
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
