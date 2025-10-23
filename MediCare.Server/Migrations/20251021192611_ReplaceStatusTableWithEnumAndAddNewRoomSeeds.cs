using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceStatusTableWithEnumAndAddNewRoomSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_VisitStatuses_StatusID",
                table: "Visits");

            migrationBuilder.DropTable(
                name: "VisitStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Visits_StatusID",
                table: "Visits");

            migrationBuilder.RenameColumn(
                name: "StatusID",
                table: "Visits",
                newName: "Status");

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "ID", "RoomNumber", "RoomType" },
                values: new object[,]
                {
                    { 5, 104, "Cardiology Consultation Room" },
                    { 6, 105, "Cardiology Consultation Room" },
                    { 7, 106, "Orthopedic Consultation Room" },
                    { 8, 107, "Dermatology Consultation Room" },
                    { 9, 202, "Operating Room" },
                    { 10, 203, "Operating Room" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "ID",
                keyValue: 10);

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Visits",
                newName: "StatusID");

            migrationBuilder.CreateTable(
                name: "VisitStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitStatuses", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "VisitStatuses",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Scheduled" },
                    { 2, "Completed" },
                    { 3, "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_StatusID",
                table: "Visits",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_VisitStatuses_StatusID",
                table: "Visits",
                column: "StatusID",
                principalTable: "VisitStatuses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
