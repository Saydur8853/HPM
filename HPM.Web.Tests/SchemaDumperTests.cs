using Microsoft.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class SchemaDumperTests
{
    private readonly ITestOutputHelper _output;

    public SchemaDumperTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Dump_All_Columns_Single_Query()
    {
        var hpmConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=30";
        var webConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=30";

        var hCols = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var wCols = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        await using (var hConn = new SqlConnection(hpmConnStr))
        {
            await hConn.OpenAsync();
            await using var cmd = hConn.CreateCommand();
            cmd.CommandText = "SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_NAME, ORDINAL_POSITION";
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                var t = r.GetString(0);
                var c = r.GetString(1);
                if (!hCols.ContainsKey(t)) hCols[t] = new List<string>();
                hCols[t].Add(c);
            }
        }

        await using (var wConn = new SqlConnection(webConnStr))
        {
            await wConn.OpenAsync();
            await using var cmd = wConn.CreateCommand();
            cmd.CommandText = "SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME NOT LIKE '__EF%' ORDER BY TABLE_NAME, ORDINAL_POSITION";
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                var t = r.GetString(0);
                var c = r.GetString(1);
                if (!wCols.ContainsKey(t)) wCols[t] = new List<string>();
                wCols[t].Add(c);
            }
        }

        foreach (var t in wCols.Keys.OrderBy(x => x))
        {
            if (hCols.TryGetValue(t, out var hc))
            {
                _output.WriteLine($"TABLE [{t}]:");
                _output.WriteLine($"  HPM_Web: {string.Join(", ", wCols[t])}");
                _output.WriteLine($"  HPM:     {string.Join(", ", hc)}");
            }
        }
    }
}
