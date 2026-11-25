using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

/// <summary>
/// Creates and seeds the cost-center accounting model (cost centers, fixed assets, allocation,
/// extended chart of accounts). Every statement is idempotent, so it runs safely at each startup
/// on new and existing hospital databases, after the EF migrations.
/// </summary>
public static class AccountingSchema
{
    public static async Task EnsureAsync(HpmDbContext db, CancellationToken ct = default)
    {
        await db.Database.ExecuteSqlRawAsync(StructureSql, ct);
        await db.Database.ExecuteSqlRawAsync(ChartSql, ct);
        await db.Database.ExecuteSqlRawAsync(CostCenterSql, ct);
        AccountMap.Invalidate();
    }

    // 1. Columns and tables. Defaults that must only be applied once run through EXEC inside
    //    the block that adds the column.
    private const string StructureSql = """
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Accounts_Chart]') AND name = 'Is_System')
        BEGIN
            ALTER TABLE [Accounts_Chart] ADD [Is_System] bit NOT NULL CONSTRAINT [DF_Accounts_Chart_Is_System] DEFAULT 0;
        END
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Accounts_Chart]') AND name = 'Requires_Cost_Center')
        BEGIN
            ALTER TABLE [Accounts_Chart] ADD [Requires_Cost_Center] bit NOT NULL CONSTRAINT [DF_Accounts_Chart_Requires_CC] DEFAULT 0;
            EXEC(N'UPDATE [Accounts_Chart] SET [Requires_Cost_Center] = 1 WHERE [Account_Type] IN (N''Income'', N''Expense'')');
        END
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Accounts_Voucher_Line]') AND name = 'Cost_Center_ID')
        BEGIN
            ALTER TABLE [Accounts_Voucher_Line] ADD [Cost_Center_ID] int NULL;
            EXEC(N'CREATE INDEX [IX_Accounts_Voucher_Line_Cost_Center] ON [Accounts_Voucher_Line] ([Cost_Center_ID], [Account_ID])');
        END
        IF OBJECT_ID(N'[Service_Category]', N'U') IS NOT NULL
           AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Service_Category]') AND name = 'Cost_Center_ID')
        BEGIN
            ALTER TABLE [Service_Category] ADD [Cost_Center_ID] int NULL;
        END
        IF OBJECT_ID(N'[Bed_Category]', N'U') IS NOT NULL
           AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Bed_Category]') AND name = 'Cost_Center_ID')
        BEGIN
            ALTER TABLE [Bed_Category] ADD [Cost_Center_ID] int NULL;
        END

        IF OBJECT_ID(N'[Accounts_Cost_Center]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Cost_Center] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Cost_Center] PRIMARY KEY,
                [Code] nvarchar(20) NOT NULL,
                [Name] nvarchar(100) NOT NULL,
                [Center_Type] nvarchar(20) NOT NULL,
                [Incharge] nvarchar(100) NULL,
                [Area_Sq_Ft] numeric(18,2) NULL,
                [Is_Active] bit NOT NULL CONSTRAINT [DF_Accounts_Cost_Center_Active] DEFAULT 1,
                [Is_System] bit NOT NULL CONSTRAINT [DF_Accounts_Cost_Center_System] DEFAULT 0);
            CREATE UNIQUE INDEX [IX_Accounts_Cost_Center_Code] ON [Accounts_Cost_Center] ([Code]);
        END

        IF OBJECT_ID(N'[Accounts_Fixed_Asset]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Fixed_Asset] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Fixed_Asset] PRIMARY KEY,
                [Asset_Code] nvarchar(30) NOT NULL,
                [Name] nvarchar(150) NOT NULL,
                [Serial_No] nvarchar(100) NULL,
                [Asset_Account_ID] int NOT NULL,
                [Cost_Center_ID] int NOT NULL,
                [Location] nvarchar(150) NULL,
                [Supplier] nvarchar(150) NULL,
                [Invoice_No] nvarchar(50) NULL,
                [Purchase_Date] date NOT NULL,
                [In_Service_Date] date NOT NULL,
                [Cost] numeric(18,2) NOT NULL,
                [Salvage_Value] numeric(18,2) NOT NULL,
                [Useful_Life_Years] int NOT NULL,
                [Opening_Acc_Depreciation] numeric(18,2) NOT NULL,
                [Status] nvarchar(20) NOT NULL,
                [Warranty_End] date NULL,
                [Disposed_Date] date NULL,
                [Disposal_Amount] numeric(18,2) NULL,
                [Purchase_Voucher_ID] bigint NULL,
                [Disposal_Voucher_ID] bigint NULL,
                [Entered_By] int NOT NULL,
                [Trans_Date] datetime NOT NULL);
            CREATE UNIQUE INDEX [IX_Accounts_Fixed_Asset_Code] ON [Accounts_Fixed_Asset] ([Asset_Code]);
        END

        IF OBJECT_ID(N'[Accounts_Fixed_Asset_Transfer]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Fixed_Asset_Transfer] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Fixed_Asset_Transfer] PRIMARY KEY,
                [Asset_ID] int NOT NULL,
                [From_Cost_Center_ID] int NOT NULL,
                [To_Cost_Center_ID] int NOT NULL,
                [Transfer_Date] date NOT NULL,
                [Reason] nvarchar(300) NULL,
                [Entered_By] int NOT NULL);
        END

        IF OBJECT_ID(N'[Accounts_Fixed_Asset_Depreciation]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Fixed_Asset_Depreciation] (
                [ID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Fixed_Asset_Depreciation] PRIMARY KEY,
                [Asset_ID] int NOT NULL,
                [Period_Month] date NOT NULL,
                [Amount] numeric(18,2) NOT NULL,
                [Cost_Center_ID] int NOT NULL,
                [Voucher_ID] bigint NOT NULL);
            CREATE UNIQUE INDEX [IX_Accounts_FA_Depreciation_Asset_Month] ON [Accounts_Fixed_Asset_Depreciation] ([Asset_ID], [Period_Month]);
        END

        IF OBJECT_ID(N'[Accounts_Allocation_Rule]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Allocation_Rule] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Allocation_Rule] PRIMARY KEY,
                [Source_Cost_Center_ID] int NOT NULL,
                [Basis] nvarchar(20) NOT NULL,
                [Target_Cost_Center_ID] int NULL,
                [Weight] numeric(18,4) NULL,
                [Is_Active] bit NOT NULL CONSTRAINT [DF_Accounts_Allocation_Rule_Active] DEFAULT 1);
        END

        IF OBJECT_ID(N'[Accounts_Allocation_Run]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Allocation_Run] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Allocation_Run] PRIMARY KEY,
                [Period_Month] date NOT NULL,
                [Voucher_ID] bigint NOT NULL,
                [Run_By] int NOT NULL,
                [Run_At] datetime NOT NULL);
            CREATE UNIQUE INDEX [IX_Accounts_Allocation_Run_Month] ON [Accounts_Allocation_Run] ([Period_Month]);
        END
        """;

    // 2. Chart of accounts: move the original heads to their new codes (only while they still carry
    //    the original code and the new code is free), then add every missing head of the model.
    private const string ChartSql = """
        IF EXISTS (SELECT 1 FROM [Accounts_Chart] WHERE [ID] = 2 AND [Code] = N'4000')
           AND NOT EXISTS (SELECT 1 FROM [Accounts_Chart] WHERE [Code] = N'4090')
            UPDATE [Accounts_Chart] SET [Code] = N'4090', [Name] = N'Legacy Service Revenue' WHERE [ID] = 2;
        IF EXISTS (SELECT 1 FROM [Accounts_Chart] WHERE [ID] = 3 AND [Code] = N'4010')
           AND NOT EXISTS (SELECT 1 FROM [Accounts_Chart] WHERE [Code] = N'4400')
            UPDATE [Accounts_Chart] SET [Code] = N'4400' WHERE [ID] = 3;
        IF EXISTS (SELECT 1 FROM [Accounts_Chart] WHERE [ID] = 4 AND [Code] = N'5000' AND [Name] = N'Medicine Purchase')
            UPDATE [Accounts_Chart] SET [Name] = N'Cost of Medicine Sold' WHERE [ID] = 4;
        IF EXISTS (SELECT 1 FROM [Accounts_Chart] WHERE [ID] = 5 AND [Code] = N'5100')
           AND NOT EXISTS (SELECT 1 FROM [Accounts_Chart] WHERE [Code] = N'6900')
            UPDATE [Accounts_Chart] SET [Code] = N'6900', [Name] = N'Other Admin Expense' WHERE [ID] = 5;

        DECLARE @heads TABLE ([Code] nvarchar(20), [Name] nvarchar(100), [Type] nvarchar(20), [NeedCC] bit, [Sys] bit);
        INSERT INTO @heads VALUES
            (N'1010', N'Petty Cash', N'Asset', 0, 0),
            (N'1020', N'Bank Account', N'Asset', 0, 0),
            (N'1030', N'bKash / Nagad / Card Clearing', N'Asset', 0, 0),
            (N'1110', N'Corporate / Insurance Receivable', N'Asset', 0, 0),
            (N'1200', N'Medicine Inventory', N'Asset', 0, 1),
            (N'1210', N'Lab Reagent & Consumable Inventory', N'Asset', 0, 0),
            (N'1300', N'Advance & Deposit', N'Asset', 0, 0),
            (N'1500', N'Land & Building', N'Asset', 1, 1),
            (N'1510', N'Medical Equipment', N'Asset', 1, 1),
            (N'1520', N'Furniture & Fixture', N'Asset', 1, 1),
            (N'1530', N'Computer & IT Equipment', N'Asset', 1, 1),
            (N'1540', N'Vehicle & Ambulance', N'Asset', 1, 1),
            (N'1550', N'Electrical & Generator', N'Asset', 1, 1),
            (N'1600', N'Acc. Depreciation - Building', N'Asset', 1, 1),
            (N'1610', N'Acc. Depreciation - Medical Equipment', N'Asset', 1, 1),
            (N'1620', N'Acc. Depreciation - Furniture & Fixture', N'Asset', 1, 1),
            (N'1630', N'Acc. Depreciation - Computer & IT', N'Asset', 1, 1),
            (N'1640', N'Acc. Depreciation - Vehicle & Ambulance', N'Asset', 1, 1),
            (N'1650', N'Acc. Depreciation - Electrical & Generator', N'Asset', 1, 1),
            (N'2200', N'Salary Payable', N'Liability', 0, 0),
            (N'2210', N'Doctor Fee Payable', N'Liability', 0, 0),
            (N'2300', N'VAT & Tax Payable', N'Liability', 0, 0),
            (N'2500', N'Bank Loan', N'Liability', 0, 0),
            (N'3000', N'Owner''s Capital', N'Equity', 0, 1),
            (N'3100', N'Drawings', N'Equity', 0, 0),
            (N'3200', N'Retained Earnings', N'Equity', 0, 1),
            (N'4000', N'Consultation Income', N'Income', 1, 1),
            (N'4100', N'Diagnostic Income', N'Income', 1, 1),
            (N'4200', N'Bed & Admission Income', N'Income', 1, 1),
            (N'4300', N'Procedure & OT Income', N'Income', 1, 1),
            (N'4400', N'Pharmacy Sales', N'Income', 1, 1),
            (N'4500', N'Other Income', N'Income', 1, 1),
            (N'4900', N'Discount Allowed', N'Income', 1, 1),
            (N'5100', N'Reagent & Consumable Used', N'Expense', 1, 0),
            (N'5200', N'Doctor Fee / Commission', N'Expense', 1, 0),
            (N'5300', N'Referral Commission', N'Expense', 1, 0),
            (N'5400', N'Outsourced Test Cost', N'Expense', 1, 0),
            (N'6000', N'Salary & Allowance', N'Expense', 1, 0),
            (N'6100', N'Electricity, Water & Gas', N'Expense', 1, 0),
            (N'6200', N'Rent', N'Expense', 1, 0),
            (N'6300', N'Repair & Maintenance', N'Expense', 1, 0),
            (N'6400', N'Medical Gas (Oxygen)', N'Expense', 1, 0),
            (N'6500', N'Cleaning & Laundry', N'Expense', 1, 0),
            (N'6600', N'Patient Food', N'Expense', 1, 0),
            (N'6700', N'Stationery & Printing', N'Expense', 1, 0),
            (N'6800', N'Marketing', N'Expense', 1, 0),
            (N'6900', N'Other Admin Expense', N'Expense', 1, 1),
            (N'7000', N'Depreciation Expense', N'Expense', 1, 1),
            (N'7100', N'Bank Charge & Interest', N'Expense', 1, 0);

        INSERT INTO [Accounts_Chart] ([Code], [Name], [Account_Type], [Is_Active], [Is_System], [Requires_Cost_Center])
        SELECT h.[Code], h.[Name], h.[Type], 1, h.[Sys], h.[NeedCC]
        FROM @heads h
        WHERE NOT EXISTS (SELECT 1 FROM [Accounts_Chart] a WHERE a.[Code] = h.[Code]);

        -- Heads the posting engine uses are system heads (type locked, cannot be deactivated),
        -- but only where the existing head really is of the expected type.
        UPDATE a SET a.[Is_System] = 1, a.[Is_Active] = 1
        FROM [Accounts_Chart] a JOIN @heads h ON h.[Code] = a.[Code] AND h.[Type] = a.[Account_Type]
        WHERE h.[Sys] = 1 AND a.[Is_System] = 0;
        UPDATE [Accounts_Chart] SET [Is_System] = 1 WHERE [ID] BETWEEN 1 AND 8 AND [Is_System] = 0;
        """;

    // 3. Cost centers and the default month-end allocation rules (spread every service and admin
    //    department over the revenue departments by their income, until the hospital sets its own).
    private const string CostCenterSql = """
        DECLARE @cc TABLE ([Code] nvarchar(20), [Name] nvarchar(100), [Type] nvarchar(20));
        INSERT INTO @cc VALUES
            (N'CC-101', N'OPD (Outpatient)', N'Revenue'),
            (N'CC-102', N'Emergency', N'Revenue'),
            (N'CC-103', N'IPD Ward & Cabin', N'Revenue'),
            (N'CC-104', N'ICU / CCU / NICU', N'Revenue'),
            (N'CC-105', N'Operation Theatre (OT)', N'Revenue'),
            (N'CC-106', N'Pathology (Laboratory)', N'Revenue'),
            (N'CC-107', N'Radiology & Imaging', N'Revenue'),
            (N'CC-108', N'Cardiology & Procedures', N'Revenue'),
            (N'CC-109', N'Pharmacy', N'Revenue'),
            (N'CC-110', N'Blood Bank', N'Revenue'),
            (N'CC-111', N'Ambulance', N'Revenue'),
            (N'CC-112', N'Canteen', N'Revenue'),
            (N'CC-201', N'Reception & Billing', N'Service'),
            (N'CC-202', N'Nursing Administration', N'Service'),
            (N'CC-203', N'Housekeeping & Laundry', N'Service'),
            (N'CC-204', N'Maintenance & Biomedical', N'Service'),
            (N'CC-205', N'Store & Procurement', N'Service'),
            (N'CC-206', N'Kitchen (Patient Food)', N'Service'),
            (N'CC-301', N'Administration & Management', N'Admin'),
            (N'CC-302', N'Accounts & Finance', N'Admin'),
            (N'CC-303', N'HR', N'Admin'),
            (N'CC-304', N'IT', N'Admin'),
            (N'CC-399', N'General (Shared Cost)', N'Admin');

        INSERT INTO [Accounts_Cost_Center] ([Code], [Name], [Center_Type], [Is_Active], [Is_System])
        SELECT c.[Code], c.[Name], c.[Type], 1, 1
        FROM @cc c
        WHERE NOT EXISTS (SELECT 1 FROM [Accounts_Cost_Center] x WHERE x.[Code] = c.[Code]);

        IF NOT EXISTS (SELECT 1 FROM [Accounts_Allocation_Rule])
            INSERT INTO [Accounts_Allocation_Rule] ([Source_Cost_Center_ID], [Basis], [Is_Active])
            SELECT [ID], N'Revenue', 1 FROM [Accounts_Cost_Center] WHERE [Center_Type] IN (N'Service', N'Admin');
        """;
}
