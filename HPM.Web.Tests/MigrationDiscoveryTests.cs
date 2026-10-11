using Microsoft.Data.SqlClient;
using System.Data;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class MigrationDiscoveryTests
{
    private readonly ITestOutputHelper _output;

    public MigrationDiscoveryTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Discover_Databases_And_Tables()
    {
        var masterConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=master;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30";
        await using var conn = new SqlConnection(masterConnStr);
        await conn.OpenAsync();

        // 1. List Databases
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name FROM sys.databases WHERE name NOT IN ('master','tempdb','model','msdb')";
        await using var reader = await cmd.ExecuteReaderAsync();
        _output.WriteLine("=== DATABASES ON SERVER ===");
        var dbNames = new List<string>();
        while (await reader.ReadAsync())
        {
            var db = reader.GetString(0);
            dbNames.Add(db);
            _output.WriteLine($"DB: {db}");
        }
        await reader.CloseAsync();

        // 2. For HPM and HPM_Web, count tables and rows
        foreach (var db in dbNames)
        {
            if (db.Contains("AutomatedTests")) continue;
            _output.WriteLine($"\n=== TABLES AND ROW COUNTS IN [{db}] ===");
            await using var dbConn = new SqlConnection($"Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database={db};Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30");
            await dbConn.OpenAsync();

            await using var tCmd = dbConn.CreateCommand();
            tCmd.CommandText = @"
                SELECT t.name AS TableName, SUM(p.rows) AS [RowCount]
                FROM sys.tables t
                INNER JOIN sys.partitions p ON t.object_id = p.object_id
                WHERE p.index_id IN (0,1) AND t.name NOT LIKE '__EF%'
                GROUP BY t.name
                ORDER BY [RowCount] DESC, t.name";
            await using var tReader = await tCmd.ExecuteReaderAsync();
            while (await tReader.ReadAsync())
            {
                var tName = tReader.GetString(0);
                var rCount = tReader.GetInt64(1);
                if (rCount > 0)
                {
                    _output.WriteLine($"  {tName,-35} : {rCount} rows");
                }
            }
        }
    }
}
