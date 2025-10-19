using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedsToVisitsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitDateTime",
                table: "Visits");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "VisitTime",
                table: "Visits",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.InsertData(
                table: "Visits",
                columns: new[] { "ID", "DoctorID", "PatientID", "RoomID", "StatusID", "VisitDate", "VisitTime" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 1, new DateOnly(2025, 10, 20), new TimeOnly(10, 30, 0) },
                    { 2, 2, 2, 2, 1, new DateOnly(2025, 10, 21), new TimeOnly(13, 0, 0) },
                    { 3, 1, 2, 1, 2, new DateOnly(2025, 10, 22), new TimeOnly(9, 30, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "VisitTime",
                table: "Visits");

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitDateTime",
                table: "Visits",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
