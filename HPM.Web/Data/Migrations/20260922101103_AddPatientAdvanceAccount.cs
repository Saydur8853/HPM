using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientAdvanceAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts_Chart",
                columns: new[] { "ID", "Account_Type", "Code", "Is_Active", "Name", "Parent_ID" },
                values: new object[] { 8, "Liability", "2100", true, "Patient Advance", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts_Chart",
                keyColumn: "ID",
                keyValue: 8);
        }
    }
}
