using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitReasonAndAdditionalNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Rooms");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 1,
                column: "RoomType",
                value: "Cardiology Consultation Room");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 2,
                column: "RoomType",
                value: "Orthopedic Consultation Room");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "RoomNumber", "RoomType" },
                values: new object[] { 103, "Dermatology Consultation Room" });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "ID", "RoomNumber", "RoomType" },
                values: new object[] { 4, 201, "Operating Room" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.AddColumn<bool>(
                name: "Availability",
                table: "Rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Availability", "RoomType" },
                values: new object[] { true, "Consultation Room" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Availability", "RoomType" },
                values: new object[] { true, "Consultation Room" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "Availability", "RoomNumber", "RoomType" },
                values: new object[] { false, 201, "Operating Room" });
        }
    }
}
