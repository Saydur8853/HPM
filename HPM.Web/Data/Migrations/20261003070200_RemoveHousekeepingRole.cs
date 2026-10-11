using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHousekeepingRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DECLARE @HousekeepingRoleId int = (SELECT TOP 1 [Id] FROM [AspNetRoles] WHERE [NormalizedName] = 'HOUSEKEEPING');
                DECLARE @IpdOperatorRoleId int = (SELECT TOP 1 [Id] FROM [AspNetRoles] WHERE [NormalizedName] = 'IPD OPERATOR');

                IF @HousekeepingRoleId IS NOT NULL
                BEGIN
                    IF @IpdOperatorRoleId IS NOT NULL
                    BEGIN
                        INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
                        SELECT ur.[UserId], @IpdOperatorRoleId
                        FROM [AspNetUserRoles] ur
                        WHERE ur.[RoleId] = @HousekeepingRoleId
                          AND NOT EXISTS (
                              SELECT 1 FROM [AspNetUserRoles] existing
                              WHERE existing.[UserId] = ur.[UserId] AND existing.[RoleId] = @IpdOperatorRoleId);
                    END

                    DELETE FROM [AspNetUserRoles] WHERE [RoleId] = @HousekeepingRoleId;
                    DELETE FROM [AspNetRoles] WHERE [Id] = @HousekeepingRoleId;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = 'HOUSEKEEPING')
                BEGIN
                    INSERT INTO [AspNetRoles] ([Name], [NormalizedName], [ConcurrencyStamp])
                    VALUES ('Housekeeping', 'HOUSEKEEPING', CONVERT(nvarchar(36), NEWID()))
                END
                """);
        }
    }
}
