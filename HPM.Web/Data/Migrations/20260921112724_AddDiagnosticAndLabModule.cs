using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosticAndLabModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lab_Sample_Collection",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Memo_ID = table.Column<long>(type: "bigint", nullable: false),
                    Specimen_Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Collected_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Collected_By = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lab_Sample_Collection", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Lab_Test_Parameter",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Parameter_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Reference_Range = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sort_Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lab_Test_Parameter", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Lab_Test_Result",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Memo_ID = table.Column<long>(type: "bigint", nullable: false),
                    Service_ID = table.Column<int>(type: "int", nullable: false),
                    Parameter_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Result_Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Reference_Range = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Flag = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Interpretation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Reported_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Reported_By = table.Column<int>(type: "int", nullable: true),
                    Is_Verified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lab_Test_Result", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lab_Sample_Collection_Memo_ID",
                table: "Lab_Sample_Collection",
                column: "Memo_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Lab_Test_Parameter_Service_ID",
                table: "Lab_Test_Parameter",
                column: "Service_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Lab_Test_Result_Memo_ID_Service_ID",
                table: "Lab_Test_Result",
                columns: new[] { "Memo_ID", "Service_ID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lab_Sample_Collection");

            migrationBuilder.DropTable(
                name: "Lab_Test_Parameter");

            migrationBuilder.DropTable(
                name: "Lab_Test_Result");
        }
    }
}
