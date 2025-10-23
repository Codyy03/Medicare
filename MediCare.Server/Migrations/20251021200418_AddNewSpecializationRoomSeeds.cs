using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSpecializationRoomSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SpecializationRooms",
                columns: new[] { "RoomID", "SpecializationID" },
                values: new object[,]
                {
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 4, 2 },
                    { 7, 2 },
                    { 9, 2 },
                    { 10, 2 },
                    { 4, 3 },
                    { 8, 3 },
                    { 9, 3 },
                    { 10, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 9, 2 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 9, 3 });

            migrationBuilder.DeleteData(
                table: "SpecializationRooms",
                keyColumns: new[] { "RoomID", "SpecializationID" },
                keyValues: new object[] { 10, 3 });
        }
    }
}
