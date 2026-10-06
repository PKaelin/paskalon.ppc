using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData.Migrations
{
    /// <summary>
    /// Model snapshot of the <see cref="SampleContext"/> after all migrations have been applied.
    /// </summary>
    [DbContext(typeof(SampleContext))]
    public class SampleContextModelSnapshot : ModelSnapshot
    {
        /// <inheritdoc/>
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData.SampleDevice", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)");

                b.Property<double>("RatedPowerInKilowatt")
                    .HasColumnType("double precision");

                b.HasKey("Id");

                b.HasIndex("Name")
                    .IsUnique();

                b.ToTable("SampleDevices");
            });
        }
    }
}
