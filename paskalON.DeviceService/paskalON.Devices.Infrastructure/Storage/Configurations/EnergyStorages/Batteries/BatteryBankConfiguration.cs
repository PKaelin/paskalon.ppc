using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.EnergyStorages.Batteries
{
    public class BatteryBankConfiguration : IEntityTypeConfiguration<BatteryBankConfig>
    {
        public void Configure(EntityTypeBuilder<BatteryBankConfig> builder)
        {
            builder.HasOne(x => x.BatteryBankDeviceConfig)
                .WithMany()
                .HasForeignKey(x => x.BatteryBankDeviceConfigId)
                .IsRequired();

            // TODO: Implement clean relationship
            builder.HasOne(x => x.ModbusConfig)
                .WithMany()
                .HasForeignKey(x => x.ModbusConfigId)
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}
