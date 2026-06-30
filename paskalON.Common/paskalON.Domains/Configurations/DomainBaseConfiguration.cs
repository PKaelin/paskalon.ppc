using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace paskalON.Domains.Configurations
{
    public class DomainBaseConfiguration : IEntityTypeConfiguration<DomainBase>
    {
        public void Configure(EntityTypeBuilder<DomainBase> builder)
        {
            // Tell EF Core to push all properties down to concrete tables
            builder.UseTpcMappingStrategy();

            // Primary key configuration
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChangedBy)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.ChangedDate)
                .IsRequired();
        }
    }
}
