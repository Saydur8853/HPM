using Microsoft.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;
using HPM.Web.Services;

namespace HPM.Web.Tests;

public sealed class QuickHpmCols
{
    private readonly ITestOutputHelper _output;

    public QuickHpmCols(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Add_Weight_To_Doctor_Appointment()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Registration]') AND name = 'Weight')
BEGIN
    ALTER TABLE [Registration] ADD [Weight] nvarchar(20) NULL;
END
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Registration]') AND name = 'Photo_Data')
BEGIN
    ALTER TABLE [Registration] ADD [Photo_Data] nvarchar(max) NULL;
END
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctor_Appointment]') AND name = 'Weight')
BEGIN
    ALTER TABLE [Doctor_Appointment] ADD [Weight] nvarchar(20) NULL;
END";
        await cmd.ExecuteNonQueryAsync();
        _output.WriteLine("Successfully ensured Weight and Photo_Data on Registration, and Weight on Doctor_Appointment!");
    }

    [Fact]
    public async Task Ensure_Prescription_Medicine_Columns()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Prescription_Medicine]') AND name = 'DosagePattern')
BEGIN
    ALTER TABLE [Prescription_Medicine] ADD [DosagePattern] nvarchar(50) NULL;
END
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Prescription_Medicine]') AND name = 'Instruction')
BEGIN
    ALTER TABLE [Prescription_Medicine] ADD [Instruction] nvarchar(100) NULL;
END";
        await cmd.ExecuteNonQueryAsync();

        // Print columns
        cmd.CommandText = "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Prescription_Medicine' ORDER BY ORDINAL_POSITION";
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            _output.WriteLine($"Prescription_Medicine Column: {r[0]} ({r[1]} {r[2]})");
        }
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
    public async Task Migrate_Diagnostic_Master_Tables()
    {
        var hpmConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM;Trusted_Connection=True;TrustServerCertificate=True";
        var webConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";

        await using var hpmConn = new SqlConnection(hpmConnStr);
        await using var webConn = new SqlConnection(webConnStr);
        await hpmConn.OpenAsync();
        await webConn.OpenAsync();

        // 1. Pathology_Group_Head
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Pathology_Group_Head', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Pathology_Group_Head] (
        [ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(250) NOT NULL
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // Copy Pathology_Group_Head
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(*) FROM [Pathology_Group_Head]";
            if ((int)(await cmd.ExecuteScalarAsync() ?? 0) == 0)
            {
                await using var readCmd = hpmConn.CreateCommand();
                readCmd.CommandText = "SELECT ID, Name FROM [Pathology_Group_Head] ORDER BY ID";
                await using var r = await readCmd.ExecuteReaderAsync();

                cmd.CommandText = "SET IDENTITY_INSERT [Pathology_Group_Head] ON";
                await cmd.ExecuteNonQueryAsync();

                while (await r.ReadAsync())
                {
                    await using var ins = webConn.CreateCommand();
                    ins.CommandText = "INSERT INTO [Pathology_Group_Head] (ID, Name) VALUES (@id, @name)";
                    ins.Parameters.AddWithValue("@id", r.GetInt32(0));
                    ins.Parameters.AddWithValue("@name", r.GetString(1));
                    await ins.ExecuteNonQueryAsync();
                }

                cmd.CommandText = "SET IDENTITY_INSERT [Pathology_Group_Head] OFF";
                await cmd.ExecuteNonQueryAsync();
                _output.WriteLine("Migrated Pathology_Group_Head successfully.");
            }
        }

        // 2. Lab_Consultent
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Lab_Consultent', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Lab_Consultent] (
        [ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(250) NOT NULL,
        [Degree] NVARCHAR(300) NULL,
        [Others] NVARCHAR(300) NULL,
        [Institution] NVARCHAR(300) NULL,
        [Module] INT NULL,
        [Consultent2] INT NULL,
        [Signature] VARBINARY(MAX) NULL,
        [Is_Active] BIT NOT NULL DEFAULT 1,
        [Title1] NVARCHAR(200) NULL,
        [Title2] NVARCHAR(200) NULL
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // Copy Lab_Consultent
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(*) FROM [Lab_Consultent]";
            if ((int)(await cmd.ExecuteScalarAsync() ?? 0) == 0)
            {
                await using var readCmd = hpmConn.CreateCommand();
                readCmd.CommandText = "SELECT ID, Name, Degree, Others, Institution, Module, Consultent2, Signature, Is_Active, Title1, Title2 FROM [Lab_Consultent] ORDER BY ID";
                await using var r = await readCmd.ExecuteReaderAsync();

                cmd.CommandText = "SET IDENTITY_INSERT [Lab_Consultent] ON";
                await cmd.ExecuteNonQueryAsync();

                while (await r.ReadAsync())
                {
                    await using var ins = webConn.CreateCommand();
                    ins.CommandText = @"INSERT INTO [Lab_Consultent] (ID, Name, Degree, Others, Institution, Module, Consultent2, Signature, Is_Active, Title1, Title2) 
VALUES (@id, @name, @deg, @oth, @inst, @mod, @c2, @sig, @act, @t1, @t2)";
                    ins.Parameters.AddWithValue("@id", r.GetInt32(0));
                    ins.Parameters.AddWithValue("@name", r.GetValue(1) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@deg", r.GetValue(2) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@oth", r.GetValue(3) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@inst", r.GetValue(4) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@mod", r.GetValue(5) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@c2", r.GetValue(6) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@sig", r.GetValue(7) is byte[] b ? (object)b : DBNull.Value);
                    ins.Parameters.AddWithValue("@act", r.GetValue(8) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@t1", r.GetValue(9) ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@t2", r.GetValue(10) ?? DBNull.Value);
                    await ins.ExecuteNonQueryAsync();
                }

                cmd.CommandText = "SET IDENTITY_INSERT [Lab_Consultent] OFF";
                await cmd.ExecuteNonQueryAsync();
                _output.WriteLine("Migrated Lab_Consultent successfully.");
            }
        }
    }

    [Fact]
    public async Task Print_Canteen_Tables()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM;Trusted_Connection=True;TrustServerCertificate=True";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%Canteen%' ORDER BY TABLE_NAME";
        await using var r = await cmd.ExecuteReaderAsync();
        var tables = new List<string>();
        while (await r.ReadAsync())
        {
            tables.Add(r.GetString(0));
        }
        await r.CloseAsync();

        _output.WriteLine($"=== Canteen Tables in HPM_Web: {string.Join(", ", tables)} ===");

        foreach (var tbl in tables)
        {
            await using var countCmd = conn.CreateCommand();
            countCmd.CommandText = $"SELECT COUNT(*) FROM [{tbl}]";
            var count = await countCmd.ExecuteScalarAsync();
            _output.WriteLine($"Table [{tbl}]: {count} rows in HPM");
        }
    }

    [Fact]
    public async Task Migrate_Canteen_Tables_To_HpmWeb()
    {
        var webConnStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";
        await using var webConn = new SqlConnection(webConnStr);
        await webConn.OpenAsync();

        // 1. Canteen_Category
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Category', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Category](
        [ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] [nvarchar](50) NOT NULL
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // 2. Canteen_Products
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Products', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Products](
        [ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] [nvarchar](100) NOT NULL,
        [Category_ID] [int] NOT NULL,
        [Sale_Rate] [decimal](18, 2) NULL DEFAULT (0),
        [Buy_Rate] [decimal](18, 2) NULL DEFAULT (0),
        [Balance] [decimal](18, 2) NULL DEFAULT (0),
        [Reorder_Level] [int] NULL DEFAULT (0),
        [Unit] [nvarchar](50) NULL,
        [IsKitchen_Item] [bit] NULL DEFAULT (0)
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // 3. Canteen_Sales
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Sales', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Sales](
        [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Customer_Name] [nvarchar](50) NOT NULL,
        [Contact] [nvarchar](20) NULL,
        [Customer_Type] [nvarchar](10) NOT NULL,
        [Total] [numeric](18, 2) NULL DEFAULT (0),
        [Service_Charge] [numeric](18, 2) NULL DEFAULT (0),
        [Discount] [numeric](18, 2) NULL DEFAULT (0),
        [Paid] [numeric](18, 2) NULL DEFAULT (0),
        [Token_No] [nvarchar](5) NULL,
        [Entered_By] [int] NOT NULL,
        [Trans_Date] [datetime] NOT NULL,
        [VAT] [numeric](18, 2) NULL DEFAULT (0)
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // 4. Canteen_Sales_Sub
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Sales_Sub', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Sales_Sub](
        [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Bill_No] [bigint] NOT NULL,
        [Service_ID] [int] NULL,
        [SLNO] [int] NULL,
        [Rate] [decimal](18, 2) NULL,
        [Buy_Rate] [decimal](18, 2) NULL,
        [Quantity] [decimal](18, 2) NULL,
        [Total] [decimal](18, 2) NULL
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // 5. Canteen_Challan
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Challan', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Challan](
        [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Supplier] [int] NOT NULL,
        [Invoice_No] [nvarchar](20) NOT NULL,
        [Invoice_Date] [datetime] NOT NULL,
        [Is_Posted] [bit] NOT NULL DEFAULT (0),
        [Total] [numeric](18, 2) NOT NULL,
        [Discount] [numeric](18, 2) NOT NULL DEFAULT (0),
        [VAT] [numeric](18, 2) NOT NULL DEFAULT (0),
        [Entered_By] [int] NOT NULL,
        [Trans_Date] [datetime] NOT NULL
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // 6. Canteen_Challan_Sub
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Challan_Sub', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Challan_Sub](
        [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Challan_No] [bigint] NOT NULL,
        [Service_ID] [int] NOT NULL,
        [SLNO] [int] NOT NULL,
        [Total] [numeric](18, 2) NOT NULL,
        [Buy_Rate] [numeric](18, 2) NOT NULL,
        [Quantity] [numeric](18, 2) NOT NULL,
        [Discount] [numeric](18, 2) NOT NULL DEFAULT (0),
        [VAT] [numeric](18, 2) NOT NULL DEFAULT (0)
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // 7. Canteen_Debtor
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Debtor', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Debtor](
        [ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] [nvarchar](50) NOT NULL,
        [Designation] [nvarchar](50) NOT NULL,
        [Balance] [decimal](18, 2) NOT NULL DEFAULT (0)
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // 8. Canteen_Debtor_Ledger
        await using (var cmd = webConn.CreateCommand())
        {
            cmd.CommandText = @"
IF OBJECT_ID('Canteen_Debtor_Ledger', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Canteen_Debtor_Ledger](
        [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Debtor_ID] [int] NOT NULL,
        [Bill_No] [bigint] NOT NULL,
        [Debit] [decimal](18, 2) NOT NULL,
        [Credit] [decimal](18, 2) NOT NULL,
        [Balance] [decimal](18, 2) NOT NULL,
        [Trans_Date] [datetime] NOT NULL
    );
END";
            await cmd.ExecuteNonQueryAsync();
        }

        // Copy existing rows from HPM into HPM_Web if empty
        var copyTables = new[]
        {
            ("Canteen_Category", "ID, Name"),
            ("Canteen_Products", "ID, Name, Category_ID, Sale_Rate, Buy_Rate, Balance, Reorder_Level, Unit, IsKitchen_Item"),
            ("Canteen_Debtor", "ID, Name, Designation, Balance"),
            ("Canteen_Debtor_Ledger", "ID, Debtor_ID, Bill_No, Debit, Credit, Balance, Trans_Date"),
            ("Canteen_Challan", "ID, Supplier, Invoice_No, Invoice_Date, Is_Posted, Total, Discount, VAT, Entered_By, Trans_Date"),
            ("Canteen_Challan_Sub", "ID, Challan_No, Service_ID, SLNO, Total, Buy_Rate, Quantity, Discount, VAT"),
            ("Canteen_Sales", "ID, Customer_Name, Contact, Customer_Type, Total, Service_Charge, Discount, Paid, Token_No, Entered_By, Trans_Date, VAT"),
            ("Canteen_Sales_Sub", "ID, Bill_No, Service_ID, SLNO, Rate, Buy_Rate, Quantity, Total")
        };

        foreach (var (tbl, cols) in copyTables)
        {
            await using var chkCmd = webConn.CreateCommand();
            chkCmd.CommandText = $"SELECT COUNT(*) FROM [{tbl}]";
            var count = Convert.ToInt32(await chkCmd.ExecuteScalarAsync() ?? 0);
            if (count == 0)
            {
                await using var insCmd = webConn.CreateCommand();
                insCmd.CommandText = $@"
SET IDENTITY_INSERT [dbo].[{tbl}] ON;
INSERT INTO [dbo].[{tbl}] ({cols})
SELECT {cols} FROM [HPM].[dbo].[{tbl}];
SET IDENTITY_INSERT [dbo].[{tbl}] OFF;";
                var inserted = await insCmd.ExecuteNonQueryAsync();
                _output.WriteLine($"Copied {inserted} rows into {tbl}");
            }
            else
            {
                _output.WriteLine($"{tbl} already has {count} rows in HPM_Web");
            }
        }
    }

    [Fact]
    public async Task Inspect_Doctor_Names_With_Dots()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT ID, Title, Name 
FROM Doctors_Profile 
WHERE Name LIKE '%..%' 
   OR Title LIKE '%..%' 
   OR Name LIKE '.%' 
   OR Title LIKE '.%'
ORDER BY ID";
        await using var r = await cmd.ExecuteReaderAsync();
        int tested = 0;
        while (await r.ReadAsync())
        {
            tested++;
            int id = r.GetInt32(0);
            string? rawTitle = r.IsDBNull(1) ? null : r.GetString(1);
            string? rawName = r.IsDBNull(2) ? null : r.GetString(2);

                string cleanTitle = DoctorNameFormatter.Sanitize(rawTitle);
                string cleanName = DoctorNameFormatter.Sanitize(rawName);

                if (tested <= 35 || rawName?.Contains("Sahla") == true || rawName?.Contains("AMITAV") == true || rawName?.Contains("Salek") == true)
                {
                    _output.WriteLine($"[{id}] BEFORE: Title='{rawTitle}' Name='{rawName}'");
                    _output.WriteLine($"     AFTER:  Title='{cleanTitle}' Name='{cleanName}' | Full='{DoctorNameFormatter.FormatFullName(cleanTitle, cleanName)}'");
                }
            }
            _output.WriteLine($"Tested {tested} records.");
        }

        [Fact]
        public async Task Clean_Doctor_Dots_In_Database()
        {
            var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True";
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            // 1. Fetch all doctors that need cleaning
            await using var selectCmd = conn.CreateCommand();
            selectCmd.CommandText = @"
SELECT ID, Title, Name 
FROM Doctors_Profile 
WHERE Name LIKE '%..%' 
   OR Title LIKE '%..%' 
   OR Name LIKE '.%' 
   OR Title LIKE '.%' 
   OR Title = '.'";
            
            var updates = new List<(int Id, string CleanTitle, string CleanName)>();
            await using (var r = await selectCmd.ExecuteReaderAsync())
            {
                while (await r.ReadAsync())
                {
                    int id = r.GetInt32(0);
                    string? rawTitle = r.IsDBNull(1) ? null : r.GetString(1);
                    string? rawName = r.IsDBNull(2) ? null : r.GetString(2);

                    string cleanTitle = DoctorNameFormatter.Sanitize(rawTitle);
                    string cleanName = DoctorNameFormatter.Sanitize(rawName);

                    if (cleanTitle != (rawTitle ?? "") || cleanName != (rawName ?? ""))
                    {
                        updates.Add((id, cleanTitle, cleanName));
                    }
                }
            }

            _output.WriteLine($"Found {updates.Count} doctor records requiring dot cleanup in HPM_Web.");

            // 2. Perform updates in transaction
            await using var tx = conn.BeginTransaction();
            int updatedCount = 0;
            foreach (var (id, cleanTitle, cleanName) in updates)
            {
                await using var updCmd = conn.CreateCommand();
                updCmd.Transaction = tx;
                updCmd.CommandText = "UPDATE Doctors_Profile SET Title = @Title, Name = @Name WHERE ID = @ID";
                updCmd.Parameters.AddWithValue("@Title", string.IsNullOrWhiteSpace(cleanTitle) ? (object)DBNull.Value : cleanTitle);
                updCmd.Parameters.AddWithValue("@Name", cleanName);
                updCmd.Parameters.AddWithValue("@ID", id);
                await updCmd.ExecuteNonQueryAsync();
                updatedCount++;
            }

            await tx.CommitAsync();
            _output.WriteLine($"Successfully updated {updatedCount} doctors in HPM_Web database!");
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


