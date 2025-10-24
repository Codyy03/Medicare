using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDoctorSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "ID", "DoctorDescription", "Email", "EndHour", "Facility", "Name", "PasswordHash", "PhoneNumber", "StartHour", "Surname" },
                values: new object[,]
                {
                    { 3, "Dr. Michael Anderson is a skilled cardiologist specializing in arrhythmia management and cardiac imaging diagnostics.", "michael.anderson@medicare.com", new TimeOnly(14, 0, 0), "Room 104, MediCare Center", "Michael", "AQAAAAIAAYagAAAAEP8zA5dHE7nyVqBLoZwbn3FPUcUJDN4lB3E4uIv9H1sF5xW0+2Fi7vsmzJYgPNh8lA==", "555111222", new TimeOnly(8, 0, 0), "Anderson" },
                    { 4, "Dr. Sophia Martinez has extensive experience in non-invasive cardiology and preventive heart health.", "sophia.martinez@medicare.com", new TimeOnly(18, 0, 0), "Room 105, MediCare Center", "Sophia", "AQAAAAIAAYagAAAAEPxZkz/F7fYxzUVEXn9xIQKo5Tk1DbiRdnXx7SC7pPjUJW1xVdYzqk5GfYjxyK6K+w==", "555333444", new TimeOnly(10, 0, 0), "Martinez" },
                    { 5, "Dr. David Kowalski is a cardiologist focused on hypertension treatment, cardiac stress testing, and patient education.", "david.kowalski@medicare.com", new TimeOnly(15, 30, 0), "Room 101, MediCare Center", "David", "AQAAAAIAAYagAAAAEKh2N3ZC1HkP7HhKJ7blQ7tb4jHY0mM3PwOydZx5Lq3yW6SkmtFfw4tq1LZzW9zENg==", "555666777", new TimeOnly(7, 30, 0), "Kowalski" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "ID",
                keyValue: 5);
        }
    }
}
