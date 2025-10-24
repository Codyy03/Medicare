using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecializationToVisit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StartHour = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndHour = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Facility = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DoctorDescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NewsItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsItems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PESEL = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomNumber = table.Column<int>(type: "integer", nullable: false),
                    RoomType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpecializationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SpecializationDescription = table.Column<string>(type: "text", nullable: false),
                    SpecializationHighlight = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    PatientID = table.Column<int>(type: "integer", nullable: true),
                    DoctorID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Doctors_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Patients_PatientID",
                        column: x => x.PatientID,
                        principalTable: "Patients",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DoctorSpecialization",
                columns: table => new
                {
                    DoctorID = table.Column<int>(type: "integer", nullable: false),
                    SpecializationID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSpecialization", x => new { x.DoctorID, x.SpecializationID });
                    table.ForeignKey(
                        name: "FK_DoctorSpecialization_Doctors_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorSpecialization_Specializations_SpecializationID",
                        column: x => x.SpecializationID,
                        principalTable: "Specializations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecializationRooms",
                columns: table => new
                {
                    SpecializationID = table.Column<int>(type: "integer", nullable: false),
                    RoomID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializationRooms", x => new { x.SpecializationID, x.RoomID });
                    table.ForeignKey(
                        name: "FK_SpecializationRooms_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecializationRooms_Specializations_SpecializationID",
                        column: x => x.SpecializationID,
                        principalTable: "Specializations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VisitDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VisitTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    DoctorID = table.Column<int>(type: "integer", nullable: false),
                    PatientID = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "text", nullable: true),
                    RoomID = table.Column<int>(type: "integer", nullable: false),
                    SpecializationID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Visits_Doctors_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visits_Patients_PatientID",
                        column: x => x.PatientID,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visits_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visits_Specializations_SpecializationID",
                        column: x => x.SpecializationID,
                        principalTable: "Specializations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "ID", "DoctorDescription", "Email", "EndHour", "Facility", "Name", "PasswordHash", "PhoneNumber", "StartHour", "Surname" },
                values: new object[,]
                {
                    { 1, "Dr. John Smith is an experienced cardiologist and surgeon with over 15 years of practice. He specializes in preventive cardiology, minimally invasive surgery, and patient-centered care.", "john.smith@medicare.com", new TimeOnly(16, 0, 0), "Room 203, MediCare Center", "John", "AQAAAAIAAYagAAAAENK5qXUBaMBuUFBpttYV0aR626yy171wqlX3Fr6lZ3A63GhTGmRFWptH6uZm1Eu9Og==", "123456789", new TimeOnly(8, 0, 0), "Smith" },
                    { 2, "Dr. Emily Johnson is a dedicated neurologist with over 10 years of experience. She focuses on patient-centered care, neurological diagnostics, and innovative treatment methods.", "emily.johnson@medicare.com", new TimeOnly(17, 0, 0), "Building A, Floor 2, MediCare Center", "Emily", "AQAAAAIAAYagAAAAEK2wr62/vPT1IadjOSNuOLLQ9ECj5CKYZbod4yvHThIexqGnCcp5Yry6PpFG9WRYYw==", "987654321", new TimeOnly(9, 0, 0), "Johnson" }
                });

            migrationBuilder.InsertData(
                table: "NewsItems",
                columns: new[] { "ID", "Date", "Description", "ImageURL", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Join us for a free blood pressure check and consultation with our cardiology team.", "https://i.ibb.co/k2hBfcpL/blood-pressure.jpg", "Free Blood Pressure Screening" },
                    { 2, new DateTime(2025, 11, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Get your flu shot before the season starts. No appointment needed.", "https://i.ibb.co/BHxNtvLj/vaccination.jpg", "Flu Vaccination Campaign" },
                    { 3, new DateTime(2025, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Learn how to prepare balanced meals with our nutritionist. Free entry.", "https://i.ibb.co/HTVch19N/healthy-eating.jpg", "Healthy Eating Workshop" },
                    { 4, new DateTime(2025, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Educational lectures and free glucose testing for all visitors.", "https://i.ibb.co/1VTGg5c/diabetes.jpg", "World Diabetes Day Awareness" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "ID", "Birthday", "Email", "Name", "PESEL", "PasswordHash", "PhoneNumber", "Surname" },
                values: new object[,]
                {
                    { 1, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "michael.brown@example.com", "Michael", "90010112345", "AQAAAAIAAYagAAAAELg5sSlJ9Z8pG1rKefpy1Pbfql3D+S2J7bXiM77uUOfyfdvsBcOdh75oC42ktQ8h3w==", "555111222", "Brown" },
                    { 2, new DateTime(1985, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "sarah.williams@example.com", "Sarah", "85050567890", "AQAAAAIAAYagAAAAELo4ftbkWmWkCzg4YbZhATzJFIRg6s7HBY322tgk4mquh9bHdQy3NraDhJLvnhjJEQ==", "555333444", "Williams" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "ID", "RoomNumber", "RoomType" },
                values: new object[,]
                {
                    { 1, 101, "Cardiology Consultation Room" },
                    { 2, 102, "Orthopedic Consultation Room" },
                    { 3, 103, "Dermatology Consultation Room" },
                    { 4, 201, "Operating Room" },
                    { 5, 104, "Cardiology Consultation Room" },
                    { 6, 105, "Cardiology Consultation Room" },
                    { 7, 106, "Orthopedic Consultation Room" },
                    { 8, 107, "Dermatology Consultation Room" },
                    { 9, 202, "Operating Room" },
                    { 10, 203, "Operating Room" }
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "ID", "Link", "SpecializationDescription", "SpecializationHighlight", "SpecializationName" },
                values: new object[,]
                {
                    { 1, "#", "Specialist in heart diseases", "Protect your heart with expert cardiovascular care and diagnostics.", "Cardiologist" },
                    { 2, "#", "Specialist in musculoskeletal system injuries and disorders", "Restore mobility and strength with advanced orthopedic solutions", "Orthopedic Surgeon" },
                    { 3, "#", "Specialist in skin conditions", "Healthy skin starts here comprehensive dermatological treatments for all ages.", "Dermatologist" }
                });

            migrationBuilder.InsertData(
                table: "DoctorSpecialization",
                columns: new[] { "DoctorID", "SpecializationID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "SpecializationRooms",
                columns: new[] { "RoomID", "SpecializationID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 2, 2 },
                    { 4, 2 },
                    { 7, 2 },
                    { 9, 2 },
                    { 10, 2 },
                    { 3, 3 },
                    { 4, 3 },
                    { 8, 3 },
                    { 9, 3 },
                    { 10, 3 }
                });

            migrationBuilder.InsertData(
                table: "Visits",
                columns: new[] { "ID", "AdditionalNotes", "DoctorID", "PatientID", "Reason", "RoomID", "SpecializationID", "Status", "VisitDate", "VisitTime" },
                values: new object[,]
                {
                    { 1, "Please discuss the test results in advance.", 1, 1, 1, 1, 1, 1, new DateOnly(2025, 10, 20), new TimeOnly(10, 30, 0) },
                    { 2, null, 2, 2, 3, 2, 2, 1, new DateOnly(2025, 10, 21), new TimeOnly(13, 0, 0) },
                    { 3, "Checkup after previous visit.", 1, 2, 2, 1, 1, 2, new DateOnly(2025, 10, 22), new TimeOnly(9, 30, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_Email",
                table: "Doctors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSpecialization_SpecializationID",
                table: "DoctorSpecialization",
                column: "SpecializationID");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                table: "Patients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PESEL",
                table: "Patients",
                column: "PESEL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_DoctorID",
                table: "RefreshTokens",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PatientID",
                table: "RefreshTokens",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomNumber",
                table: "Rooms",
                column: "RoomNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationRooms_RoomID",
                table: "SpecializationRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorID",
                table: "Visits",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PatientID",
                table: "Visits",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_RoomID",
                table: "Visits",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_SpecializationID",
                table: "Visits",
                column: "SpecializationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorSpecialization");

            migrationBuilder.DropTable(
                name: "NewsItems");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SpecializationRooms");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Specializations");
        }
    }
}
