using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.GenericModbusDevices
{
    public class CircuitBreakerConfiguration : IEntityTypeConfiguration<CircuitBreakerConfig>
    {
        public void Configure(EntityTypeBuilder<CircuitBreakerConfig> builder)
        {
            builder.HasOne(x => x.CircuitBreakerDeviceConfig)
                .WithMany()
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}
