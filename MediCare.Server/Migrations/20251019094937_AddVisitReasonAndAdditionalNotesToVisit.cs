using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitReasonAndAdditionalNotesToVisit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "Visits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reason",
                table: "Visits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "AdditionalNotes", "Reason" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "AdditionalNotes", "Reason" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "AdditionalNotes", "Reason" },
                values: new object[] { null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Visits");
        }
    }
}
