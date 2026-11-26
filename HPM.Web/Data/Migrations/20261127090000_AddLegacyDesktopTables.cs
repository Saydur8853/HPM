using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <summary>
    /// Creates the desktop-era tables (Store_*, Supplier_Store, Medical_Certificate, Patient_Operation,
    /// Operation_Note, Doctor_Commission_Settings) that earlier migrations assumed already existed.
    /// Every statement is "create if missing", so existing hospital databases are not changed.
    /// It then runs the same idempotent startup schema (store, pharmacy, accounting) so that
    /// "dotnet ef database update" alone builds a complete database. The model snapshot is unchanged.
    /// </summary>
    [DbContext(typeof(HpmDbContext))]
    [Migration("20261127090000_AddLegacyDesktopTables")]
    public partial class AddLegacyDesktopTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(LegacySchema.Sql);
            migrationBuilder.Sql(StartupSchema.Sql);
            migrationBuilder.Sql(AccountingSetup.MigrationSql());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: these tables hold hospital data and existed before this migration on most databases.
        }
    }
}
