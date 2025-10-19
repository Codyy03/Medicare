using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitReasonAndAdditionalNotesToVisitSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "AdditionalNotes", "Reason" },
                values: new object[] { "Please discuss the test results in advance.", 1 });

            migrationBuilder.UpdateData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 2,
                column: "Reason",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "AdditionalNotes", "Reason" },
                values: new object[] { "Checkup after previous visit.", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                column: "Reason",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Visits",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "AdditionalNotes", "Reason" },
                values: new object[] { null, 0 });
        }
    }
}
