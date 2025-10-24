using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDoctorSpecializationsRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DoctorSpecialization",
                columns: new[] { "DoctorID", "SpecializationID" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DoctorSpecialization",
                keyColumns: new[] { "DoctorID", "SpecializationID" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "DoctorSpecialization",
                keyColumns: new[] { "DoctorID", "SpecializationID" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "DoctorSpecialization",
                keyColumns: new[] { "DoctorID", "SpecializationID" },
                keyValues: new object[] { 5, 1 });
        }
    }
}
