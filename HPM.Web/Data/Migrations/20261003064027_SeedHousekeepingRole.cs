using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedHousekeepingRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = 'HOUSEKEEPING')
                BEGIN
                    INSERT INTO [AspNetRoles] ([Name], [NormalizedName], [ConcurrencyStamp])
                    VALUES ('Housekeeping', 'HOUSEKEEPING', CONVERT(nvarchar(36), NEWID()))
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE r FROM [AspNetRoles] r
                WHERE r.[NormalizedName] = 'HOUSEKEEPING'
                  AND NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] ur WHERE ur.[RoleId] = r.[Id])
                """);
        }
    }
}
