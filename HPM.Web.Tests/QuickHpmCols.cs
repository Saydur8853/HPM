using Microsoft.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class QuickHpmCols
{
    private readonly ITestOutputHelper _output;

    public QuickHpmCols(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Print_Hpm_Sample_Columns()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM;Trusted_Connection=True;TrustServerCertificate=True";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        var tables = new[] { "Department", "Registration", "Doctors_Profile", "Admission", "Cash_Memo", "Service" };
        foreach (var tbl in tables)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT TOP 1 * FROM [{tbl}]";
            await using var r = await cmd.ExecuteReaderAsync();
            var cols = new List<string>();
            for (int i = 0; i < r.FieldCount; i++) cols.Add(r.GetName(i));
            _output.WriteLine($"HPM Table [{tbl}]: {string.Join(", ", cols)}");
            await r.CloseAsync();
        }
    }

    [Fact]
    public async Task Create_Prescription_For_202615461()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        // 1. Check patient
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, Patient_ID, Name, Sex, Date_Of_Birth, Blood_Group FROM Registration WHERE Patient_ID = '202615461' OR Patient_ID LIKE '%202615461%'";
        string patientName = "Piash / Registered Patient";
        string patientId = "202615461";
        string sex = "Male";
        string age = "35Y";
        string bloodGroup = "B+";
        await using (var r = await cmd.ExecuteReaderAsync())
        {
            if (await r.ReadAsync())
            {
                patientId = r.GetString(1);
                patientName = r.IsDBNull(2) ? "Patient" : r.GetString(2);
                sex = r.IsDBNull(3) ? "Male" : r.GetString(3);
                if (!r.IsDBNull(4))
                {
                    var dob = r.GetDateTime(4);
                    age = dob.Year > 1900 ? $"{DateTime.Today.Year - dob.Year}Y" : "35Y";
                }
                bloodGroup = r.IsDBNull(5) ? "B+" : r.GetString(5);
                _output.WriteLine($"Found Patient: {patientName} (PID: {patientId}, Age: {age})");
            }
        }

        // 2. Doctor
        cmd.CommandText = "SELECT TOP 1 ID, Name, Title FROM Doctors_Profile WHERE Name LIKE '%Rozina%' OR ID > 0 ORDER BY ID";
        int docId = 1;
        await using (var r = await cmd.ExecuteReaderAsync())
        {
            if (await r.ReadAsync())
            {
                docId = r.GetInt32(0);
                _output.WriteLine($"Doctor: {r[1]} (ID: {docId})");
            }
        }

        // 3. Insert or update prescription
        cmd.CommandText = @"
INSERT INTO Prescription 
(Name, Age, Sex, Blood_Group, Visit_No, Visit_Date, Type, BP, Pulse, Temperature, Weight, Diagnosis, Complain, Investigation, Remark, Medicine, Referred_By, Patient_ID, Status, Created_By, Signed_At)
VALUES 
(@Name, @Age, @Sex, @BloodGroup, 1, GETDATE(), 'OPD', '125/80 mmHg', '74 bpm', '98.4 F', 68.5, 
'Evaluation of Dyslipidemia & Metabolic Screen (Suspected Non-Alcoholic Fatty Liver & Impaired Glucose Tolerance)',
'Generalized fatigue, morning lethargy, occasional dyspepsia and right hypochondriac heaviness.',
'Lipid Profile, SGPT (ALT), HbA1c',
'• Low carbohydrate & low saturated fat diet.\n• 30 minutes brisk walking daily.\n• Complete advised lab tests (Lipid Profile, SGPT, HbA1c) and report back.',
'Rosuvastatin 10mg (1+0+0 at night); Metformin 500mg (1+0+1 after meal); Silymarin 140mg (0+1+0 after meal)',
@DoctorID, @PatientID, 'Signed', 1, GETDATE());
SELECT SCOPE_IDENTITY();";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@Name", patientName);
        cmd.Parameters.AddWithValue("@Age", string.IsNullOrWhiteSpace(age) ? "35Y" : age);
        cmd.Parameters.AddWithValue("@Sex", string.IsNullOrWhiteSpace(sex) ? "Male" : sex);
        cmd.Parameters.AddWithValue("@BloodGroup", string.IsNullOrWhiteSpace(bloodGroup) ? "B+" : bloodGroup);
        cmd.Parameters.AddWithValue("@DoctorID", docId);
        cmd.Parameters.AddWithValue("@PatientID", patientId);

        var rxId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
        _output.WriteLine($"Created Prescription #{rxId} for Patient {patientName} (PID: {patientId}) with Advised Tests!");

        // Add 3 sample medicines from Service table
        cmd.CommandText = "SELECT TOP 3 ID, Name FROM Service WHERE Category_ID = 3 ORDER BY ID";
        var medServices = new List<int>();
        await using (var r = await cmd.ExecuteReaderAsync())
        {
            while (await r.ReadAsync()) medServices.Add(r.GetInt32(0));
        }

        for (int i = 0; i < medServices.Count; i++)
        {
            cmd.CommandText = "INSERT INTO Prescription_Medicine (Prescription_ID, Service_ID, Dose, Interval, Days, Quantity, SLNO) VALUES (@RxID, @SId, 1, @Interval, 30, @Qty, @SLNO)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@RxID", rxId);
            cmd.Parameters.AddWithValue("@SId", medServices[i]);
            cmd.Parameters.AddWithValue("@Interval", i == 0 ? 1 : 2);
            cmd.Parameters.AddWithValue("@Qty", i == 0 ? 30 : 60);
            cmd.Parameters.AddWithValue("@SLNO", i + 1);
            await cmd.ExecuteNonQueryAsync();
        }
        _output.WriteLine("Inserted prescription medicines successfully.");
    }
}
