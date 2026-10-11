using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBedTurnoverWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Bed_Cabin",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Available");

            migrationBuilder.CreateTable(
                name: "Bed_Turnover",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bed_Cabin_ID = table.Column<int>(type: "int", nullable: false),
                    Admission_ID = table.Column<long>(type: "bigint", nullable: true),
                    Reg_No = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Requested_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cleaning_Completed_By = table.Column<int>(type: "int", nullable: true),
                    Cleaning_Completed_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cleaning_Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Released_By = table.Column<int>(type: "int", nullable: true),
                    Released_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bed_Turnover", x => x.ID);
                    table.CheckConstraint("CK_Bed_Turnover_Status", "[Status] IN ('Cleaning','AwaitingApproval','Completed','Cancelled')");
                    table.ForeignKey(
                        name: "FK_Bed_Turnover_Admission_Admission_ID",
                        column: x => x.Admission_ID,
                        principalTable: "Admission",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bed_Turnover_Bed_Cabin_Bed_Cabin_ID",
                        column: x => x.Bed_Cabin_ID,
                        principalTable: "Bed_Cabin",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Bed_Cabin",
                keyColumn: "ID",
                keyValue: 1,
                column: "Status",
                value: "Available");

            migrationBuilder.Sql(
                "UPDATE b SET [Status] = 'Occupied' FROM [Bed_Cabin] b " +
                "WHERE b.[Is_Allocated] = 1 OR EXISTS " +
                "(SELECT 1 FROM [Admission] a WHERE a.[Bed_Cabin_ID] = b.[ID] AND a.[Is_Admitted] = 1)");

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Cabin_Status",
                table: "Bed_Cabin",
                column: "Status");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Bed_Cabin_Status",
                table: "Bed_Cabin",
                sql: "[Status] IN ('Available','Occupied','Cleaning','Maintenance')");

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Turnover_Admission_ID",
                table: "Bed_Turnover",
                column: "Admission_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Bed_Turnover_Status_Requested_At",
                table: "Bed_Turnover",
                columns: new[] { "Status", "Requested_At" });

            migrationBuilder.CreateIndex(
                name: "UX_Bed_Turnover_Open_Bed",
                table: "Bed_Turnover",
                column: "Bed_Cabin_ID",
                unique: true,
                filter: "[Status] IN ('Cleaning','AwaitingApproval')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bed_Turnover");

            migrationBuilder.DropIndex(
                name: "IX_Bed_Cabin_Status",
                table: "Bed_Cabin");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Bed_Cabin_Status",
                table: "Bed_Cabin");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bed_Cabin");
        }
    }
}
