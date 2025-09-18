using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSeedToNewsItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NewsItems",
                keyColumn: "ID",
                keyValue: 1,
                column: "ImageURL",
                value: "https://i.ibb.co/k2hBfcpL/blood-pressure.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NewsItems",
                keyColumn: "ID",
                keyValue: 1,
                column: "ImageURL",
                value: "");
        }
    }
}
