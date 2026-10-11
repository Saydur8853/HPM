using Microsoft.Data.SqlClient;
using System.Data;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class SchemaComparisonTests
{
    private readonly ITestOutputHelper _output;

    public SchemaComparisonTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Compare_HPM_and_HPM_Web_Tables()
    {
        var hpmConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30";
        var webConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30";

        await using var hpmConn = new SqlConnection(hpmConnStr);
        await using var webConn = new SqlConnection(webConnStr);
        await hpmConn.OpenAsync();
        await webConn.OpenAsync();

        // Get Web tables and columns
        var webTables = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        await using var wCmd = webConn.CreateCommand();
        wCmd.CommandText = @"
            SELECT t.name AS TableName, c.name AS ColumnName, ty.name AS DataType
            FROM sys.tables t
            INNER JOIN sys.columns c ON t.object_id = c.object_id
            INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
            WHERE t.name NOT LIKE '__EF%'";
        await using var wReader = await wCmd.ExecuteReaderAsync();
        while (await wReader.ReadAsync())
        {
            var t = wReader.GetString(0);
            var c = wReader.GetString(1);
            if (!webTables.ContainsKey(t)) webTables[t] = new List<string>();
            webTables[t].Add(c);
        }
        await wReader.CloseAsync();

        // Get HPM tables and columns
        var hpmTables = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        await using var hCmd = hpmConn.CreateCommand();
        hCmd.CommandText = @"
            SELECT t.name AS TableName, c.name AS ColumnName, ty.name AS DataType
            FROM sys.tables t
            INNER JOIN sys.columns c ON t.object_id = c.object_id
            INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id";
        await using var hReader = await hCmd.ExecuteReaderAsync();
        while (await hReader.ReadAsync())
        {
            var t = hReader.GetString(0);
            var c = hReader.GetString(1);
            if (!hpmTables.ContainsKey(t)) hpmTables[t] = new List<string>();
            hpmTables[t].Add(c);
        }
        await hReader.CloseAsync();

        _output.WriteLine("=== MATCHING TABLES IN HPM AND HPM_Web ===");
        foreach (var wt in webTables.Keys.OrderBy(x => x))
        {
            if (hpmTables.TryGetValue(wt, out var hpmCols))
            {
                var webCols = webTables[wt];
                var common = webCols.Intersect(hpmCols, StringComparer.OrdinalIgnoreCase).ToList();
                var missingInHpm = webCols.Except(hpmCols, StringComparer.OrdinalIgnoreCase).ToList();
                _output.WriteLine($"Table [{wt}]: {common.Count} common cols. Web-only cols: {string.Join(", ", missingInHpm)}");
            }
            else
            {
                _output.WriteLine($"Table [{wt}]: NOT found in HPM database directly.");
            }
        }
    }
}
