using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedsToNewsItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NewsItems",
                columns: new[] { "ID", "Date", "Description", "ImageURL", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Join us for a free blood pressure check and consultation with our cardiology team.", "", "Free Blood Pressure Screening" },
                    { 2, new DateTime(2025, 11, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Get your flu shot before the season starts. No appointment needed.", "https://i.ibb.co/BHxNtvLj/vaccination.jpg", "Flu Vaccination Campaign" },
                    { 3, new DateTime(2025, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Learn how to prepare balanced meals with our nutritionist. Free entry.", "https://i.ibb.co/HTVch19N/healthy-eating.jpg", "Healthy Eating Workshop" },
                    { 4, new DateTime(2025, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Educational lectures and free glucose testing for all visitors.", "https://i.ibb.co/1VTGg5c/diabetes.jpg", "World Diabetes Day Awareness" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "ID",
                keyValue: 4);
        }
    }
}
