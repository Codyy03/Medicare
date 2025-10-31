using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class finalFixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_DoctorID",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_PatientID",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Specializations");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 1,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 2,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 3,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 4,
                column: "Status",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_DoctorID",
                table: "RefreshTokens",
                column: "DoctorID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PatientID",
                table: "RefreshTokens",
                column: "PatientID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_DoctorID",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_PatientID",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Specializations",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "ID",
                keyValue: 1,
                column: "Link",
                value: "#");

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "ID",
                keyValue: 2,
                column: "Link",
                value: "#");

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "ID",
                keyValue: 3,
                column: "Link",
                value: "#");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_DoctorID",
                table: "RefreshTokens",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PatientID",
                table: "RefreshTokens",
                column: "PatientID");
        }
    }
}
