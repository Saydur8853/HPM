using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class HardenClinicalMasters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "Ticket_Type",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "Department",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "Department",
                keyColumn: "ID",
                keyValue: 1,
                column: "Is_Active",
                value: true);

            migrationBuilder.UpdateData(
                table: "Department",
                keyColumn: "ID",
                keyValue: 2,
                column: "Is_Active",
                value: true);

            migrationBuilder.UpdateData(
                table: "Ticket_Type",
                keyColumn: "ID",
                keyValue: 1,
                column: "Is_Active",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Type_Name",
                table: "Ticket_Type",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name",
                table: "Department",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ticket_Type_Name",
                table: "Ticket_Type");

            migrationBuilder.DropIndex(
                name: "IX_Department_Name",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "Ticket_Type");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "Department");
        }
    }
}
