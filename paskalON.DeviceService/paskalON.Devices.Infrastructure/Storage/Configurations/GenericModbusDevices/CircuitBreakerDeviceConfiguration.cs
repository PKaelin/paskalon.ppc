// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.GenericModbusDevices
{
    public class CircuitBreakerDeviceConfiguration : IEntityTypeConfiguration<CircuitBreakerDeviceConfig>
    {
        public void Configure(EntityTypeBuilder<CircuitBreakerDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}
