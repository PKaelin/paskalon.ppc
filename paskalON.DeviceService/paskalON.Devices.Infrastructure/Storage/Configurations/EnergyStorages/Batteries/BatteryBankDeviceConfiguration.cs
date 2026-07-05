// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.EnergyStorages.Batteries
{
    public class BatteryBankDeviceConfiguration : IEntityTypeConfiguration<BatteryBankDeviceConfig>
    {
        public void Configure(EntityTypeBuilder<BatteryBankDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}
