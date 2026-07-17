// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs;

namespace paskalON.Devices.Infrastructure.Storage.Configurations
{
    public class C37Configuration : IEntityTypeConfiguration<C37Config>
    {
        public void Configure(EntityTypeBuilder<C37Config> builder)
        {
            builder.Property(x => x.IpAddress).IsRequired();
            builder.Property(x => x.Port).IsRequired();
            builder.Property(x => x.TransportLayer).IsRequired();
            builder.Property(x => x.StreamId).IsRequired();
            builder.Property(x => x.StationName).IsRequired();
            // Depends on the encoding but lets assume 1 byte per character
            builder.Property(x => x.StationName).HasMaxLength(16);
        }
    }
}
