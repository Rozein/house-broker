using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseBroker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLocationConstrain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_CityId_PostalCode",
                table: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CityId_Area_PostalCode",
                table: "Location",
                columns: new[] { "CityId", "Area", "PostalCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Location_CityId_Area_PostalCode",
                table: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CityId_PostalCode",
                table: "Location",
                columns: new[] { "CityId", "PostalCode" },
                unique: true);
        }
    }
}
