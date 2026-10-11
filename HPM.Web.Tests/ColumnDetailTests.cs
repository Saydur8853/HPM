using Microsoft.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class ColumnDetailTests
{
    private readonly ITestOutputHelper _output;

    public ColumnDetailTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Print_Table_Columns()
    {
        var hpmConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM;Trusted_Connection=True;TrustServerCertificate=True";
        var webConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";

        var tables = new[]
        {
            "Department", "IPD_Department", "Ticket_Type", "Service_Category", "Service", "Doctors_Profile",
            "Bed_Category", "Bed_Cabin", "Generic_Group", "Medicine_Type", "Supplier_Pharmacy",
            "Medicine_Store", "Medicine_Store_Balance", "Registration", "Doctor_Appointment", "OPD_Ticket",
            "Admission", "Admission_Ext", "Bed_Cabin_Transection", "Patient_Transaction", "Patient_Package_Transaction",
            "Patient_Payment", "Money_Receipt", "Central_Cash_Collection", "Cash_Memo", "Cash_Memo_Sub",
            "Cash_Memo_DInvestigation", "Cash_Memo_Sub_DInvestigation", "Cash_Memo_OT", "Cash_Memo_Sub_OT",
            "Medicine_Receiving_Challan", "Medicine_Receiving_Challan_Sub", "Supplier_Ledger", "Supplier_Payment",
            "Medicine_Return", "Medicine_Return_Sub", "Prescription", "Prescription_Medicine"
        };

        await using var hConn = new SqlConnection(hpmConnStr);
        await using var wConn = new SqlConnection(webConnStr);
        await hConn.OpenAsync();
        await wConn.OpenAsync();

        foreach (var tbl in tables)
        {
            var hCols = await GetColumnsAsync(hConn, tbl);
            var wCols = await GetColumnsAsync(wConn, tbl);

            _output.WriteLine($"\n==================== TABLE: [{tbl}] ====================");
            _output.WriteLine($"HPM_Web ({wCols.Count} cols): {string.Join(", ", wCols)}");
            _output.WriteLine($"HPM     ({hCols.Count} cols): {string.Join(", ", hCols)}");
        }
    }

    private static async Task<List<string>> GetColumnsAsync(SqlConnection conn, string tableName)
    {
        var list = new List<string>();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT c.name FROM sys.tables t INNER JOIN sys.columns c ON t.object_id = c.object_id WHERE t.name = '{tableName}' ORDER BY c.column_id";
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(r.GetString(0));
        }
        return list;
    }
}
