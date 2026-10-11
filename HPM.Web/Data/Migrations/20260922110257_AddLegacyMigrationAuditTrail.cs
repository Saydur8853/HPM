using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLegacyMigrationAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Legacy_Migration_Run",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source_Database = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Target_Database = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Approved_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Approval_Hash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Started_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Completed_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Is_Rollback_Verification = table.Column<bool>(type: "bit", nullable: false),
                    Summary_Json = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legacy_Migration_Run", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Legacy_Migration_Quarantine",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Run_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Source_ID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Payload_Json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legacy_Migration_Quarantine", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Legacy_Migration_Quarantine_Legacy_Migration_Run_Run_ID",
                        column: x => x.Run_ID,
                        principalTable: "Legacy_Migration_Run",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Legacy_Migration_Quarantine_Run_ID_Entity_Name",
                table: "Legacy_Migration_Quarantine",
                columns: new[] { "Run_ID", "Entity_Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Legacy_Migration_Run_Started_At",
                table: "Legacy_Migration_Run",
                column: "Started_At");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Legacy_Migration_Quarantine");

            migrationBuilder.DropTable(
                name: "Legacy_Migration_Run");
        }
    }
}
