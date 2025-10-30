using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Patients",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 1,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 2,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 3,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 4,
                column: "Status",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Patients");
        }
    }
}
