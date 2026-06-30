using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Ders
{
    public class DerCircuitConfiguration : IEntityTypeConfiguration<DerCircuitConfig>
    {
        public void Configure(EntityTypeBuilder<DerCircuitConfig> builder)
        {
            builder.HasMany(x => x.DerUnitConfigs)
                .WithOne(x => x.DerCircuitConfig)
                .HasForeignKey(x => x.DerCircuitConfigId)
                .IsRequired();

            builder.HasOne(x => x.CircuitBreakerConfig)
                .WithOne(x => x.DerCircuitConfig)
                .HasForeignKey<CircuitBreakerConfig>(x => x.DerCircuitConfigId)
                .IsRequired();

            builder.HasOne(x => x.CircuitPowerMeterConfig)
                .WithOne(x => x.DerCircuitConfig)
                .HasForeignKey<CircuitPowerMeterConfig>(x => x.DerCircuitConfigId)
                .IsRequired();
        }
    }
}
