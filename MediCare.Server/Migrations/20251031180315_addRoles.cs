using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class addRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Patients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Doctors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 1,
                column: "Role",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 2,
                column: "Role",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 3,
                column: "Role",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 4,
                column: "Role",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 5,
                column: "Role",
                value: 2);

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "ID", "DoctorDescription", "Email", "EndHour", "Facility", "Name", "PasswordHash", "PhoneNumber", "Role", "StartHour", "Surname" },
                values: new object[] { 7, "Dr. Evan Chris is a cardiologist focused on hypertension treatment, cardiac stress testing, and patient education.", "Evan.chris@medicare.com", new TimeOnly(15, 30, 0), "Room 101, MediCare Center", "Evan", "AQAAAAIAAYagAAAAEKh2N3ZC1HkP7HhKJ7blQ7tb4jHY0mM3PwOydZx5Lq3yW6SkmtFfw4tq1LZzW9zENg==", "888888888", 3, new TimeOnly(7, 30, 0), "Chris" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 1,
                column: "Role",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 2,
                column: "Role",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 3,
                column: "Role",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "ID",
                keyValue: 4,
                column: "Role",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Doctors");
        }
    }
}
