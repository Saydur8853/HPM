using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlaintextOwnerPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF COL_LENGTH(N'dbo.Hospital_License', N'OwnerPassword') IS NOT NULL ALTER TABLE dbo.Hospital_License DROP COLUMN OwnerPassword;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Plaintext credentials must never be restored by a rollback.
        }
    }
}
