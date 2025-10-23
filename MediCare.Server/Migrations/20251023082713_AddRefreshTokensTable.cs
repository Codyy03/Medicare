using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Doctors_DoctorID",
                table: "RefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Patients_PatientID",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "RefreshTokens");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_PatientID",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_PatientID");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_DoctorID",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_DoctorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Doctors_DoctorID",
                table: "RefreshTokens",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Patients_PatientID",
                table: "RefreshTokens",
                column: "PatientID",
                principalTable: "Patients",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Doctors_DoctorID",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Patients_PatientID",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_PatientID",
                table: "RefreshToken",
                newName: "IX_RefreshToken_PatientID");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_DoctorID",
                table: "RefreshToken",
                newName: "IX_RefreshToken_DoctorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Doctors_DoctorID",
                table: "RefreshToken",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Patients_PatientID",
                table: "RefreshToken",
                column: "PatientID",
                principalTable: "Patients",
                principalColumn: "ID");
        }
    }
}
