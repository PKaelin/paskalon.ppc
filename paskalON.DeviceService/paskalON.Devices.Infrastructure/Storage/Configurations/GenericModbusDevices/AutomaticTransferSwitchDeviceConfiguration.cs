using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;


namespace paskalON.Devices.Infrastructure.Storage.Configurations.GenericModbusDevices
{
    public class AutomaticTransferSwitchDeviceConfiguration : IEntityTypeConfiguration<AutomaticTransferSwitchDeviceConfig>
    {
        public void Configure(EntityTypeBuilder<AutomaticTransferSwitchDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}
