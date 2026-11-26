using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <summary>
    /// Repairs schema differences left by migrations that updated the EF model snapshot without
    /// applying the equivalent changes to every database lineage. The SQL is idempotent so it can
    /// safely converge legacy local databases and freshly-created production databases.
    /// </summary>
    [DbContext(typeof(HpmDbContext))]
    [Migration("20261127100000_RepairSchemaDrift")]
    public partial class RepairSchemaDrift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH(N'dbo.Prescription_Medicine', N'DosagePattern') IS NULL
                    ALTER TABLE [dbo].[Prescription_Medicine]
                        ADD [DosagePattern] nvarchar(50) NULL;

                IF COL_LENGTH(N'dbo.Prescription_Medicine', N'Instruction') IS NULL
                    ALTER TABLE [dbo].[Prescription_Medicine]
                        ADD [Instruction] nvarchar(100) NULL;

                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'dbo.Registration')
                      AND name = N'Date_Of_Birth'
                      AND is_nullable = 0
                )
                    ALTER TABLE [dbo].[Registration]
                        ALTER COLUMN [Date_Of_Birth] datetime NULL;

                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'dbo.Medicine_Receiving_Challan')
                      AND name = N'Invoice_No'
                      AND (system_type_id <> TYPE_ID(N'nvarchar') OR max_length <> 40 OR is_nullable <> 0)
                )
                BEGIN
                    IF EXISTS
                    (
                        SELECT 1
                        FROM [dbo].[Medicine_Receiving_Challan]
                        WHERE LEN([Invoice_No]) > 20
                    )
                        THROW 51000, 'Cannot normalize Medicine_Receiving_Challan.Invoice_No: a value exceeds 20 characters.', 1;

                    IF EXISTS
                    (
                        SELECT 1
                        FROM sys.indexes
                        WHERE object_id = OBJECT_ID(N'dbo.Medicine_Receiving_Challan')
                          AND name = N'IX_Medicine_Receiving_Challan_Supplier_Invoice_No'
                    )
                        DROP INDEX [IX_Medicine_Receiving_Challan_Supplier_Invoice_No]
                            ON [dbo].[Medicine_Receiving_Challan];

                    ALTER TABLE [dbo].[Medicine_Receiving_Challan]
                        ALTER COLUMN [Invoice_No] nvarchar(20) NOT NULL;

                    CREATE UNIQUE INDEX [IX_Medicine_Receiving_Challan_Supplier_Invoice_No]
                        ON [dbo].[Medicine_Receiving_Challan] ([Supplier], [Invoice_No]);
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "RepairSchemaDrift converges databases with different histories and cannot be safely reversed. Restore the pre-migration backup instead.");
        }
    }
}
