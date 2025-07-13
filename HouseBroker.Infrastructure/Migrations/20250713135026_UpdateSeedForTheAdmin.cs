using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseBroker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedForTheAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("111787ea-adb1-484a-a8d1-d479917d2943"),
                columns: new[] { "Email", "UserName" },
                values: new object[] { "admin@housebroker.com", "admin@housebroker.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("111787ea-adb1-484a-a8d1-d479917d2943"),
                columns: new[] { "Email", "UserName" },
                values: new object[] { "adming@housebroker.com", "adming@housebroker.com" });
        }
    }
}
