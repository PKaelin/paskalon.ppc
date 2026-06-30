using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Ders
{
    public class DerConfiguration : IEntityTypeConfiguration<DerConfig>
    {
        public void Configure(EntityTypeBuilder<DerConfig> builder)
        {
            builder.HasMany(x => x.DerGroupConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.GenericModbusConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.AutomaticTransferSwitchConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.SystemPowerMeterConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.AuxiliaryPowerMeterConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.ExternalPowerMeterConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();
        }
    }
}
