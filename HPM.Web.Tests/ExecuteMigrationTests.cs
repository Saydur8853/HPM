using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class ExecuteMigrationTests
{
    private readonly ITestOutputHelper _output;

    public ExecuteMigrationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Migrate_All_Legacy_Data_From_HPM_To_HPM_Web()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=600";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        var sw = Stopwatch.StartNew();

        _output.WriteLine("=== STARTING ROBUST DATA MIGRATION: HPM -> HPM_Web ===");

        // 1. Disable all foreign key constraints on HPM_Web
        await ExecuteSqlAsync(conn, "EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
        _output.WriteLine("[OK] Disabled all foreign key constraints.");

        // 2. Clear old test data in HPM_Web
        var tablesToMigrate = new[]
        {
            // Masters
            "Department", "IPD_Department", "Ticket_Type", "Service_Category", "Service", "Doctors_Profile",
            "Bed_Category", "Bed_Cabin", "Generic_Group", "Medicine_Type", "Supplier_Pharmacy",
            "Medicine_Store_Balance",
            // Patient & OPD
            "Registration", "Doctor_Appointment", "OPD_Ticket",
            // IPD
            "Admission", "Admission_Ext", "Bed_Cabin_Transection", "Money_Receipt",
            "Patient_Transaction", "Patient_Package_Transaction", "Patient_Payment", "Central_Cash_Collection",
            // Cash Memos & Pharmacy
            "Cash_Memo", "Cash_Memo_Sub",
            "Cash_Memo_DInvestigation", "Cash_Memo_Sub_DInvestigation",
            "Cash_Memo_OT", "Cash_Memo_Sub_OT",
            "Medicine_Receiving_Challan", "Medicine_Receiving_Challan_Sub",
            "Supplier_Ledger", "Supplier_Payment",
            "Medicine_Return", "Medicine_Return_Sub",
            // Prescriptions
            "Prescription", "Prescription_Medicine"
        };

        foreach (var tbl in tablesToMigrate.Reverse())
        {
            try
            {
                await ExecuteSqlAsync(conn, $"DELETE FROM [HPM_Web].[dbo].[{tbl}]");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"[WARN] Could not clear [{tbl}]: {ex.Message}");
            }
        }
        _output.WriteLine("[OK] Cleared temporary test data.");

        // 3. Pre-fetch Identity columns map for HPM_Web
        var identityTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using (var idCmd = conn.CreateCommand())
        {
            idCmd.CommandText = @"
                SELECT t.name
                FROM [HPM_Web].sys.tables t
                INNER JOIN [HPM_Web].sys.columns c ON t.object_id = c.object_id
                WHERE c.is_identity = 1";
            await using var idReader = await idCmd.ExecuteReaderAsync();
            while (await idReader.ReadAsync())
            {
                identityTables.Add(idReader.GetString(0));
            }
        }

        // 4. Custom SQL mappings where deduplication or special defaults are required
        var customSqls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Bed_Cabin"] = @"
                WITH DedupBeds AS (
                    SELECT [ID],
                           CASE WHEN ROW_NUMBER() OVER(PARTITION BY [Name] ORDER BY [ID]) > 1 
                                THEN RTRIM([Name]) + '-' + CAST([ID] AS NVARCHAR(10))
                                ELSE [Name]
                           END AS [Name],
                           [Category_ID], [Is_Allocated], [Status]
                    FROM [HPM].[dbo].[Bed_Cabin]
                )
                INSERT INTO [HPM_Web].[dbo].[Bed_Cabin] ([ID], [Name], [Category_ID], [Is_Allocated], [Status])
                SELECT [ID], [Name], [Category_ID], ISNULL([Is_Allocated], 0), ISNULL([Status], 'Available')
                FROM DedupBeds",

            ["Admission"] = @"
                WITH DedupAdmissions AS (
                    SELECT [ID],
                           CASE WHEN ROW_NUMBER() OVER(PARTITION BY [Reg_No] ORDER BY [ID]) > 1 
                                THEN RTRIM([Reg_No]) + '-' + CAST([ID] AS NVARCHAR(10))
                                ELSE [Reg_No]
                           END AS [Reg_No],
                           [Patient_ID], [Department], [Diagnosis], [Age], [Referred_By], [Consultant],
                           [Admitted_By], [Is_Referred], [Bed_Cabin_ID], [Admission_Fee], [Admission_Date],
                           [Release_Date], [Is_Admitted], [Bed_Admission_Date], [Comments], [Bill_No],
                           [Bill_Generated_By], [MPO_ID], [Is_Package], [Trans_Date], [Entered_By],
                           [Ambulance], [Outcome], [Patient_Category], [Patient_Received_By], [Patient_Received_Date]
                    FROM [HPM].[dbo].[Admission]
                )
                INSERT INTO [HPM_Web].[dbo].[Admission] ([ID], [Reg_No], [Patient_ID], [Department], [Diagnosis], [Age], [Referred_By], [Consultant], [Admitted_By], [Is_Referred], [Bed_Cabin_ID], [Admission_Fee], [Admission_Date], [Release_Date], [Is_Admitted], [Bed_Admission_Date], [Comments], [Bill_No], [Bill_Generated_By], [MPO_ID], [Is_Package], [Trans_Date], [Entered_By], [Ambulance], [Outcome], [Patient_Category], [Patient_Received_By], [Patient_Received_Date])
                SELECT [ID], [Reg_No], [Patient_ID], [Department], [Diagnosis], [Age], [Referred_By], [Consultant], [Admitted_By], [Is_Referred], [Bed_Cabin_ID], [Admission_Fee], [Admission_Date], [Release_Date], [Is_Admitted], [Bed_Admission_Date], [Comments], [Bill_No], [Bill_Generated_By], [MPO_ID], [Is_Package], [Trans_Date], [Entered_By], [Ambulance], [Outcome], [Patient_Category], [Patient_Received_By], [Patient_Received_Date]
                FROM DedupAdmissions",

            ["Admission_Ext"] = @"
                WITH DedupExt AS (
                    SELECT [ID],
                           CASE WHEN ROW_NUMBER() OVER(PARTITION BY [Reg_No] ORDER BY [ID]) > 1 
                                THEN RTRIM([Reg_No]) + '-' + CAST([ID] AS NVARCHAR(10))
                                ELSE [Reg_No]
                           END AS [Reg_No],
                           [Service_Charge], [Food]
                    FROM [HPM].[dbo].[Admission_Ext]
                )
                INSERT INTO [HPM_Web].[dbo].[Admission_Ext] ([ID], [Reg_No], [Service_Charge], [Food])
                SELECT [ID], [Reg_No], [Service_Charge], [Food]
                FROM DedupExt"
        };

        long totalMigratedRows = 0;

        foreach (var tbl in tablesToMigrate)
        {
            var stepSw = Stopwatch.StartNew();
            var hasIdentity = identityTables.Contains(tbl);

            string sql;
            if (customSqls.TryGetValue(tbl, out var customSql))
            {
                sql = customSql;
            }
            else
            {
                var webCols = await GetColumnsAsync(conn, "HPM_Web", tbl);
                var hpmCols = await GetColumnsAsync(conn, "HPM", tbl);
                var commonCols = webCols.Intersect(hpmCols, StringComparer.OrdinalIgnoreCase).ToList();
                if (commonCols.Count == 0)
                {
                    _output.WriteLine($"[SKIP] Table [{tbl}] has no common columns.");
                    continue;
                }
                var colList = string.Join(", ", commonCols.Select(c => $"[{c}]"));
                sql = $@"
                    INSERT INTO [HPM_Web].[dbo].[{tbl}] ({colList})
                    SELECT {colList}
                    FROM [HPM].[dbo].[{tbl}]";
            }

            try
            {
                if (hasIdentity)
                {
                    await ExecuteSqlAsync(conn, $"SET IDENTITY_INSERT [HPM_Web].[dbo].[{tbl}] ON;");
                }

                await ExecuteSqlAsync(conn, sql);

                if (hasIdentity)
                {
                    await ExecuteSqlAsync(conn, $"SET IDENTITY_INSERT [HPM_Web].[dbo].[{tbl}] OFF;");
                }

                await using var cCmd = conn.CreateCommand();
                cCmd.CommandText = $"SELECT COUNT_BIG(*) FROM [HPM_Web].[dbo].[{tbl}]";
                var count = (long)(await cCmd.ExecuteScalarAsync() ?? 0L);
                totalMigratedRows += count;

                _output.WriteLine($"[SUCCESS] Migrated [{tbl,-32}]: {count,10:N0} rows ({stepSw.ElapsedMilliseconds} ms)");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"[ERROR] Failed to migrate [{tbl}]: {ex.Message}");
                if (hasIdentity)
                {
                    try { await ExecuteSqlAsync(conn, $"SET IDENTITY_INSERT [HPM_Web].[dbo].[{tbl}] OFF;"); } catch { }
                }
            }
        }

        // 5. Update Bed_Cabin Occupied status based on active admissions
        await ExecuteSqlAsync(conn, @"
            UPDATE b
            SET b.[Status] = 'Occupied', b.[Is_Allocated] = 1
            FROM [HPM_Web].[dbo].[Bed_Cabin] b
            INNER JOIN [HPM_Web].[dbo].[Admission] a ON b.[ID] = a.[Bed_Cabin_ID]
            WHERE a.[Is_Admitted] = 1 AND a.[Release_Date] IS NULL");
        _output.WriteLine("[OK] Updated active occupied beds.");

        _output.WriteLine($"\n============================================================");
        _output.WriteLine($"=== TOTAL MIGRATED RECORDS: {totalMigratedRows:N0} ROWS IN {sw.Elapsed.TotalSeconds:F2} SECONDS ===");
        _output.WriteLine($"============================================================");
    }

    private static async Task<List<string>> GetColumnsAsync(SqlConnection conn, string dbName, string tableName)
    {
        var list = new List<string>();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
            SELECT c.name
            FROM [{dbName}].sys.tables t
            INNER JOIN [{dbName}].sys.columns c ON t.object_id = c.object_id
            WHERE t.name = '{tableName}'
            ORDER BY c.column_id";
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(r.GetString(0));
        }
        return list;
    }

    private static async Task ExecuteSqlAsync(SqlConnection conn, string sql)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = 600;
        await cmd.ExecuteNonQueryAsync();
    }
}
