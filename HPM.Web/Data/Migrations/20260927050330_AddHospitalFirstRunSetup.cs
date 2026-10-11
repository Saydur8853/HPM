using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalFirstRunSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hospital_Profile",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Hospital_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Is_Setup_Complete = table.Column<bool>(type: "bit", nullable: false),
                    Setup_Completed_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Setup_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospital_Profile", x => x.ID);
                });

            // A brand-new database has no users and must go through /setup. An
            // upgraded installation already has an administrator, so preserve
            // access and give it a sensible legacy display name.
            migrationBuilder.Sql("""
                INSERT INTO [Hospital_Profile]
                    ([ID], [Hospital_Name], [Is_Setup_Complete], [Setup_Completed_At], [Setup_By])
                SELECT 1,
                       CASE WHEN EXISTS (SELECT 1 FROM [AspNetUsers]) THEN N'Prime Hospital' ELSE N'' END,
                       CASE WHEN EXISTS (SELECT 1 FROM [AspNetUsers]) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
                       CASE WHEN EXISTS (SELECT 1 FROM [AspNetUsers]) THEN SYSUTCDATETIME() ELSE NULL END,
                       CASE WHEN EXISTS (SELECT 1 FROM [AspNetUsers]) THEN N'Migration' ELSE NULL END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hospital_Profile");
        }
    }
}
