using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData.Migrations
{
    /// <summary>
    /// Second migration adding a unique index on the sample device name.
    /// </summary>
    [DbContext(typeof(SampleContext))]
    [Migration("20260102000000_AddUniqueDeviceName")]
    public class AddUniqueDeviceName : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SampleDevices_Name",
                table: "SampleDevices",
                column: "Name",
                unique: true);
        }


        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_SampleDevices_Name", table: "SampleDevices");
        }
    }
}
