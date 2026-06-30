using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.GenericModbusDevices
{
    public class AutomaticTransferSwitchConfiguration : IEntityTypeConfiguration<AutomaticTransferSwitchConfig>
    {
        public void Configure(EntityTypeBuilder<AutomaticTransferSwitchConfig> builder)
        {
            builder.HasOne(x => x.AutomaticTransferSwitchDeviceConfig)
                .WithMany()
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}
