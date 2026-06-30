using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs;

namespace paskalON.Devices.Infrastructure.Storage.Configurations
{
    public class ModbusConfiguration : IEntityTypeConfiguration<ModbusConfig>
    {
        public void Configure(EntityTypeBuilder<ModbusConfig> builder)
        {
            builder.HasOne(x => x.ModbusConnectionConfig)
                .WithMany()
                .HasForeignKey(x => x.ModbusConnectionConfigId)
                .IsRequired();

            builder.Property(x => x.Address).IsRequired();
            builder.Property(x => x.Port).IsRequired();
            builder.Property(x => x.AddressFamily).IsRequired();
            builder.Property(x => x.StationId).IsRequired();
        }
    }
}
