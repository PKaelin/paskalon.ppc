using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Ders
{
    public class DerGroupConfiguration : IEntityTypeConfiguration<DerGroupConfig>
    {
        public void Configure(EntityTypeBuilder<DerGroupConfig> builder)
        {
            builder.HasMany(x => x.DerCircuits)
                .WithOne(x => x.DerGroupConfig)
                .HasForeignKey(x => x.DerGroupConfigId)
                .IsRequired();
        }
    }
}
