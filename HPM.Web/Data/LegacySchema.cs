using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

/// <summary>
/// Tables the app maps but that no EF migration creates, because they came from the original desktop
/// database. On an existing hospital database they already exist and nothing happens; on a brand-new
/// (empty) database they are created here so the rest of the startup schema work can run.
/// Definitions were scripted from a working database.
/// </summary>
public static class LegacySchema
{
    public static Task EnsureAsync(HpmDbContext db) => db.Database.ExecuteSqlRawAsync(Sql);

    public const string Sql = """
        IF OBJECT_ID(N'[Store_Product_Category]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Store_Product_Category] (
                [ID] int IDENTITY(1,1) NOT NULL,
                [Name] nvarchar(50) NOT NULL,
                CONSTRAINT [PK_Store_Product_Category] PRIMARY KEY CLUSTERED ([ID]));
        END
        IF OBJECT_ID(N'[Supplier_Store]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Supplier_Store] (
                [ID] int IDENTITY(1,1) NOT NULL,
                [Name] nvarchar(50) NOT NULL,
                [Address] nvarchar(100) NULL,
                [Contact] nvarchar(50) NULL,
                [Amount] numeric(18,2) NOT NULL CONSTRAINT [DF_Supplier_Store_Amount] DEFAULT ((0)),
                [Trans_Date] datetime NULL,
                CONSTRAINT [PK_Supplier_Store] PRIMARY KEY CLUSTERED ([ID]));
        END
        IF OBJECT_ID(N'[Store_Product]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Store_Product] (
                [ID] int IDENTITY(1,1) NOT NULL,
                [Name] nvarchar(50) NOT NULL,
                [Unit] nvarchar(50) NOT NULL,
                [Category] int NOT NULL,
                [Supplier] int NOT NULL,
                [Re_Order_Level] int NOT NULL CONSTRAINT [DF_Store_Product_Re_Order_Level] DEFAULT ((0)),
                [Balance] decimal(18,2) NOT NULL CONSTRAINT [DF_Store_Product_Balance] DEFAULT ((0)),
                [Buy_Rate] decimal(18,2) NOT NULL CONSTRAINT [DF_Store_Product_Buy_Rate] DEFAULT ((0)),
                [Sale_Rate] decimal(18,2) NOT NULL CONSTRAINT [DF_Store_Product_Sale_Rate] DEFAULT ((0)),
                [Entered_By] int NOT NULL CONSTRAINT [DF_Store_Product_Entered_By] DEFAULT ((1)),
                [Trans_Date] datetime NOT NULL CONSTRAINT [DF_Store_Product_Trans_Date] DEFAULT (getdate()),
                [Is_Active] bit NOT NULL CONSTRAINT [DF_Store_Product_Is_Active] DEFAULT ((1)),
                [Track_Expiry] bit NOT NULL CONSTRAINT [DF_Store_Product_Track_Expiry] DEFAULT ((0)),
                CONSTRAINT [PK_Store_Product] PRIMARY KEY CLUSTERED ([ID]));
        END
        IF OBJECT_ID(N'[Store_Challan]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Store_Challan] (
                [ID] bigint IDENTITY(1,1) NOT NULL,
                [Supplier] int NOT NULL,
                [Invoice_No] nvarchar(20) NOT NULL,
                [Invoice_Date] datetime NOT NULL,
                [Is_Posted] bit NOT NULL CONSTRAINT [DF_Store_Challan_Is_Posted] DEFAULT ((0)),
                [Total] numeric(18,2) NOT NULL CONSTRAINT [DF_Store_Challan_Total] DEFAULT ((0)),
                [Entered_By] int NOT NULL CONSTRAINT [DF_Store_Challan_Entered_By] DEFAULT ((1)),
                [Trans_Date] datetime NOT NULL CONSTRAINT [DF_Store_Challan_Trans_Date] DEFAULT (getdate()),
                [Location_ID] int NULL,
                [Note] nvarchar(300) NULL,
                CONSTRAINT [PK_Store_Challan] PRIMARY KEY CLUSTERED ([ID]));
        END
        IF OBJECT_ID(N'[Store_Challan_Sub]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Store_Challan_Sub] (
                [ID] bigint IDENTITY(1,1) NOT NULL,
                [Challan_No] bigint NOT NULL,
                [Product_ID] int NOT NULL,
                [SLNO] int NOT NULL,
                [Total] numeric(18,2) NOT NULL,
                [Buy_Rate] numeric(18,2) NOT NULL,
                [Quantity] numeric(18,2) NOT NULL,
                [Batch_No] nvarchar(50) NULL,
                [Expire_Date] date NULL,
                [Stock_ID] int NULL,
                CONSTRAINT [PK_Store_Challan_Sub] PRIMARY KEY CLUSTERED ([ID]));
        END
        IF OBJECT_ID(N'[Store_Issue]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Store_Issue] (
                [ID] bigint IDENTITY(1,1) NOT NULL,
                [Issue_Department] int NOT NULL,
                [IndentNo] nvarchar(10) NOT NULL,
                [Total] numeric(18,2) NULL,
                [Entered_By] int NOT NULL,
                [Trans_Date] datetime NOT NULL CONSTRAINT [DF_Store_Issue_Trans_Date] DEFAULT (getdate()),
                [Location_ID] int NULL,
                [Requisition_ID] bigint NULL,
                [Received_By] nvarchar(100) NULL,
                [Note] nvarchar(300) NULL,
                CONSTRAINT [PK_Store_Issue] PRIMARY KEY CLUSTERED ([ID]));
        END
        IF OBJECT_ID(N'[Store_Issue_Sub]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Store_Issue_Sub] (
                [ID] bigint IDENTITY(1,1) NOT NULL,
                [Issue_No] bigint NOT NULL,
                [Product_ID] int NULL,
                [SLNO] int NULL,
                [Buy_Rate] numeric(18,2) NULL,
                [Quantity] numeric(18,2) NULL,
                [Total] numeric(18,2) NULL,
                [Stock_ID] int NULL,
                [Batch_No] nvarchar(50) NULL,
                [Expire_Date] date NULL,
                CONSTRAINT [PK_Store_Issue_Sub] PRIMARY KEY CLUSTERED ([ID]));
        END
        IF OBJECT_ID(N'[Medical_Certificate]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Medical_Certificate] (
                [ID] bigint IDENTITY(1,1) NOT NULL,
                [Certificate_No] nvarchar(50) NOT NULL,
                [Certificate_Type] nvarchar(30) NOT NULL,
                [Patient_ID] nvarchar(30) NOT NULL,
                [Reg_No] nvarchar(20) NULL,
                [Issue_Date] datetime NOT NULL,
                [Doctor_ID] int NOT NULL,
                [Details_Json] nvarchar(max) NULL,
                [Remarks] nvarchar(1000) NULL,
                [Issued_By] int NOT NULL CONSTRAINT [DF_Medical_Certificate_Issued_By] DEFAULT ((1)),
                [Created_Date] datetime NOT NULL CONSTRAINT [DF_Medical_Certificate_Created_Date] DEFAULT (getdate()),
                CONSTRAINT [PK_Medical_Certificate] PRIMARY KEY CLUSTERED ([ID]));
            CREATE INDEX [IX_Medical_Certificate_Certificate_Type_Issue_Date] ON [Medical_Certificate] ([Certificate_Type], [Issue_Date]);
            CREATE INDEX [IX_Medical_Certificate_Date] ON [Medical_Certificate] ([Issue_Date]);
            CREATE INDEX [IX_Medical_Certificate_Patient_ID] ON [Medical_Certificate] ([Patient_ID]);
            CREATE INDEX [IX_Medical_Certificate_PatientID] ON [Medical_Certificate] ([Patient_ID]);
            CREATE INDEX [IX_Medical_Certificate_Reg_No] ON [Medical_Certificate] ([Reg_No]);
            CREATE INDEX [IX_Medical_Certificate_RegNo] ON [Medical_Certificate] ([Reg_No]);
            CREATE INDEX [IX_Medical_Certificate_Type] ON [Medical_Certificate] ([Certificate_Type]);
            CREATE UNIQUE INDEX [UQ_Medical_Certificate_Certificate_No] ON [Medical_Certificate] ([Certificate_No]);
        END
        IF OBJECT_ID(N'[Patient_Operation]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Patient_Operation] (
                [ID] bigint IDENTITY(1,1) NOT NULL,
                [Reg_No] nvarchar(20) NOT NULL,
                [Patient_ID] nvarchar(30) NOT NULL,
                [Procedure_Name] nvarchar(300) NOT NULL,
                [Anaesthesia_Type] nvarchar(100) NULL,
                [Operator_ID] int NOT NULL,
                [Anasthetist_ID] int NULL,
                [Assistant_Doctor_IDs] nvarchar(500) NULL,
                [Theatre_Room] nvarchar(100) NULL,
                [Operation_Date] datetime NOT NULL,
                [Drug] nvarchar(1000) NULL,
                [Remarks] nvarchar(1000) NULL,
                [Status] nvarchar(30) NOT NULL CONSTRAINT [DF_Patient_Operation_Status] DEFAULT ('Scheduled'),
                [Entered_By] int NOT NULL CONSTRAINT [DF_Patient_Operation_Entered_By] DEFAULT ((1)),
                [Trans_Date] datetime NOT NULL CONSTRAINT [DF_Patient_Operation_Trans_Date] DEFAULT (getdate()),
                CONSTRAINT [PK_Patient_Operation] PRIMARY KEY CLUSTERED ([ID]));
            CREATE INDEX [IX_Patient_Operation_Date] ON [Patient_Operation] ([Operation_Date]);
            CREATE INDEX [IX_Patient_Operation_Operation_Date] ON [Patient_Operation] ([Operation_Date]);
            CREATE INDEX [IX_Patient_Operation_Operator] ON [Patient_Operation] ([Operator_ID]);
            CREATE INDEX [IX_Patient_Operation_Patient_ID] ON [Patient_Operation] ([Patient_ID]);
            CREATE INDEX [IX_Patient_Operation_PatientID] ON [Patient_Operation] ([Patient_ID]);
            CREATE INDEX [IX_Patient_Operation_Reg_No] ON [Patient_Operation] ([Reg_No]);
            CREATE INDEX [IX_Patient_Operation_RegNo] ON [Patient_Operation] ([Reg_No]);
            CREATE INDEX [IX_Patient_Operation_Status] ON [Patient_Operation] ([Status]);
        END
        IF OBJECT_ID(N'[Operation_Note]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Operation_Note] (
                [ID] int IDENTITY(1,1) NOT NULL,
                [Operation_ID] bigint NULL,
                [Reg_No] nvarchar(20) NOT NULL,
                [Patient_ID] nvarchar(30) NOT NULL,
                [Operation_Name] nvarchar(300) NOT NULL,
                [Indication] nvarchar(500) NULL,
                [Operation_Date] datetime NOT NULL,
                [Surgeon_ID] int NOT NULL,
                [Assistant1_ID] int NULL,
                [Assistant2_ID] int NULL,
                [Assistant3_ID] int NULL,
                [Anaesthetist_ID] int NULL,
                [Anaesthesia_Type] nvarchar(100) NULL,
                [Pre_Op_Diagnosis] nvarchar(500) NULL,
                [Post_Op_Diagnosis] nvarchar(500) NULL,
                [Procedure_And_Findings] nvarchar(max) NULL,
                [Incision] nvarchar(500) NULL,
                [Specimen_Biopsy] nvarchar(500) NULL,
                [Blood_Loss_Estimate] nvarchar(100) NULL,
                [Drains_Packs] nvarchar(500) NULL,
                [Post_Op_Orders] nvarchar(2000) NULL,
                [Entered_By] int NOT NULL CONSTRAINT [DF_Operation_Note_Entered_By] DEFAULT ((1)),
                [Trans_Date] datetime NOT NULL CONSTRAINT [DF_Operation_Note_Trans_Date] DEFAULT (getdate()),
                CONSTRAINT [PK_Operation_Note] PRIMARY KEY CLUSTERED ([ID]));
            CREATE INDEX [IX_Operation_Note_Date] ON [Operation_Note] ([Operation_Date]);
            CREATE INDEX [IX_Operation_Note_Operation_Date] ON [Operation_Note] ([Operation_Date]);
            CREATE INDEX [IX_Operation_Note_Patient_ID] ON [Operation_Note] ([Patient_ID]);
            CREATE INDEX [IX_Operation_Note_PatientID] ON [Operation_Note] ([Patient_ID]);
            CREATE INDEX [IX_Operation_Note_Reg_No] ON [Operation_Note] ([Reg_No]);
            CREATE INDEX [IX_Operation_Note_RegNo] ON [Operation_Note] ([Reg_No]);
            CREATE INDEX [IX_Operation_Note_Surgeon] ON [Operation_Note] ([Surgeon_ID]);
        END
        IF OBJECT_ID(N'[Doctor_Commission_Settings]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Doctor_Commission_Settings] (
                [ID] int IDENTITY(1,1) NOT NULL,
                [DoctorID] int NOT NULL,
                [CategoryID] int NOT NULL,
                [CommissionPercent] decimal(5,2) NOT NULL CONSTRAINT [DF_Doctor_Commission_Settings_CommissionPercent] DEFAULT ((0.00)),
                [UpdatedDate] datetime NOT NULL CONSTRAINT [DF_Doctor_Commission_Settings_UpdatedDate] DEFAULT (getdate()),
                [UpdatedBy] int NULL,
                CONSTRAINT [PK_Doctor_Commission_Settings] PRIMARY KEY CLUSTERED ([ID]));
            CREATE UNIQUE INDEX [UQ_Doctor_Category] ON [Doctor_Commission_Settings] ([DoctorID], [CategoryID]);
        END
        """;
}
