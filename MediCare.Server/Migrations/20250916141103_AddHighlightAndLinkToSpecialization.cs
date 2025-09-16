using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddHighlightAndLinkToSpecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Specializations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpecializationHighlight",
                table: "Specializations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Link", "SpecializationHighlight" },
                values: new object[] { "#", "Protect your heart with expert cardiovascular care and diagnostics." });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Link", "SpecializationHighlight" },
                values: new object[] { "#", "Restore mobility and strength with advanced orthopedic solutions" });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "Link", "SpecializationHighlight", "SpecializationName" },
                values: new object[] { "#", "Healthy skin starts here comprehensive dermatological treatments for all ages.", "8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "SpecializationHighlight",
                table: "Specializations");

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "ID",
                keyValue: 3,
                column: "SpecializationName",
                value: "Dermatologist");
        }
    }
}
