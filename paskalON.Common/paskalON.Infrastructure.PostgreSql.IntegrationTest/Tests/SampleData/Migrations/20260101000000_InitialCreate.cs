using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData.Migrations
{
    /// <summary>
    /// Initial migration creating the sample device table.
    /// </summary>
    [DbContext(typeof(SampleContext))]
    [Migration("20260101000000_InitialCreate")]
    public class InitialCreate : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RatedPowerInKilowatt = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleDevices", x => x.Id);
                });
        }


        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SampleDevices");
        }
    }
}
