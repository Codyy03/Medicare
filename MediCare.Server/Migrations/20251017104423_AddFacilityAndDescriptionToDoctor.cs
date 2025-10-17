using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityAndDescriptionToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoctorDescription",
                table: "Doctors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Facility",
                table: "Doctors",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "DoctorDescription", "Facility" },
                values: new object[] { "Dr. John Smith is an experienced cardiologist and surgeon with over 15 years of practice. He specializes in preventive cardiology, minimally invasive surgery, and patient-centered care.", "Room 203, MediCare Center" });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "DoctorDescription", "Facility" },
                values: new object[] { "Dr. Emily Johnson is a dedicated neurologist with over 10 years of experience. She focuses on patient-centered care, neurological diagnostics, and innovative treatment methods.", "Building A, Floor 2, MediCare Center" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorDescription",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Facility",
                table: "Doctors");
        }
    }
}
