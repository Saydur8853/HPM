namespace HPM.Web.Data;

/// <summary>
/// Idempotent schema work (extra columns, store tables, pharmacy payment methods, indexes, seeds) that is
/// safe to run any number of times. It runs at every startup and once from the AddLegacyDesktopTables
/// migration, so "dotnet ef database update" alone also produces a complete database.
/// </summary>
public static class StartupSchema
{
    public const string Sql = """
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_License]') AND name = 'OwnerPassword')
            BEGIN
                ALTER TABLE [Hospital_License] ADD [OwnerPassword] nvarchar(100) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Developer_Company_Name')
            BEGIN
                ALTER TABLE [Hospital_Profile] ADD [Developer_Company_Name] nvarchar(200) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Developer_Copyright')
            BEGIN
                ALTER TABLE [Hospital_Profile] ADD [Developer_Copyright] nvarchar(300) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Developer_Logo_Data')
            BEGIN
                ALTER TABLE [Hospital_Profile] ADD [Developer_Logo_Data] nvarchar(max) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Developer_Email')
            BEGIN
                ALTER TABLE [Hospital_Profile] ADD [Developer_Email] nvarchar(150) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Developer_Phone')
            BEGIN
                ALTER TABLE [Hospital_Profile] ADD [Developer_Phone] nvarchar(100) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Cash_Memo]') AND name = 'Payment_Method')
            BEGIN
                ALTER TABLE [Cash_Memo] ADD [Payment_Method] nvarchar(40) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Cash_Memo]') AND name = 'Payment_Ref')
            BEGIN
                ALTER TABLE [Cash_Memo] ADD [Payment_Ref] nvarchar(100) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]') AND name = 'Payment_Method')
            BEGIN
                ALTER TABLE [Cash_Memo_DInvestigation] ADD [Payment_Method] nvarchar(40) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]') AND name = 'Payment_Ref')
            BEGIN
                ALTER TABLE [Cash_Memo_DInvestigation] ADD [Payment_Ref] nvarchar(100) NULL;
            END
            IF OBJECT_ID(N'[Pharmacy_Payment_Method]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Pharmacy_Payment_Method] (
                    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Pharmacy_Payment_Method] PRIMARY KEY,
                    [Name] nvarchar(40) NOT NULL,
                    [Is_Active] bit NOT NULL CONSTRAINT [DF_Pharmacy_Payment_Method_Is_Active] DEFAULT (1),
                    [Sort_Order] int NOT NULL CONSTRAINT [DF_Pharmacy_Payment_Method_Sort_Order] DEFAULT (0),
                    [Is_System] bit NOT NULL CONSTRAINT [DF_Pharmacy_Payment_Method_Is_System] DEFAULT (0)
                );
                CREATE UNIQUE NONCLUSTERED INDEX [IX_Pharmacy_Payment_Method_Name] ON [Pharmacy_Payment_Method]([Name]);
            END
            IF NOT EXISTS (SELECT 1 FROM [Pharmacy_Payment_Method] WHERE [Name] = N'Cash')
                INSERT INTO [Pharmacy_Payment_Method] ([Name], [Is_Active], [Sort_Order], [Is_System]) VALUES (N'Cash', 1, 0, 1);
            IF NOT EXISTS (SELECT 1 FROM [Pharmacy_Payment_Method] WHERE [Name] = N'Bank')
                INSERT INTO [Pharmacy_Payment_Method] ([Name], [Is_Active], [Sort_Order], [Is_System]) VALUES (N'Bank', 1, 1, 1);
            IF NOT EXISTS (SELECT 1 FROM [Pharmacy_Payment_Method] WHERE [Name] = N'MFS')
                INSERT INTO [Pharmacy_Payment_Method] ([Name], [Is_Active], [Sort_Order], [Is_System]) VALUES (N'MFS', 1, 2, 1);
            -- ===== Hospital Store: sub-stores, batch stock, ledger, requisition, movements =====
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Product]') AND name = 'Is_Active')
                ALTER TABLE [Store_Product] ADD [Is_Active] bit NOT NULL CONSTRAINT [DF_Store_Product_Is_Active] DEFAULT (1);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Product]') AND name = 'Track_Expiry')
                ALTER TABLE [Store_Product] ADD [Track_Expiry] bit NOT NULL CONSTRAINT [DF_Store_Product_Track_Expiry] DEFAULT (0);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Challan]') AND name = 'Location_ID')
                ALTER TABLE [Store_Challan] ADD [Location_ID] int NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Challan]') AND name = 'Note')
                ALTER TABLE [Store_Challan] ADD [Note] nvarchar(300) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Challan_Sub]') AND name = 'Batch_No')
                ALTER TABLE [Store_Challan_Sub] ADD [Batch_No] nvarchar(50) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Challan_Sub]') AND name = 'Expire_Date')
                ALTER TABLE [Store_Challan_Sub] ADD [Expire_Date] date NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Challan_Sub]') AND name = 'Stock_ID')
                ALTER TABLE [Store_Challan_Sub] ADD [Stock_ID] int NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Issue]') AND name = 'Location_ID')
                ALTER TABLE [Store_Issue] ADD [Location_ID] int NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Issue]') AND name = 'Requisition_ID')
                ALTER TABLE [Store_Issue] ADD [Requisition_ID] bigint NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Issue]') AND name = 'Received_By')
                ALTER TABLE [Store_Issue] ADD [Received_By] nvarchar(100) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Issue]') AND name = 'Note')
                ALTER TABLE [Store_Issue] ADD [Note] nvarchar(300) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Issue_Sub]') AND name = 'Stock_ID')
                ALTER TABLE [Store_Issue_Sub] ADD [Stock_ID] int NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Issue_Sub]') AND name = 'Batch_No')
                ALTER TABLE [Store_Issue_Sub] ADD [Batch_No] nvarchar(50) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Store_Issue_Sub]') AND name = 'Expire_Date')
                ALTER TABLE [Store_Issue_Sub] ADD [Expire_Date] date NULL;

            IF OBJECT_ID(N'[Store_Location]', N'U') IS NULL
                CREATE TABLE [Store_Location] (
                    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Location] PRIMARY KEY,
                    [Name] nvarchar(60) NOT NULL,
                    [Is_Central] bit NOT NULL CONSTRAINT [DF_Store_Location_Is_Central] DEFAULT (0),
                    [Is_Active] bit NOT NULL CONSTRAINT [DF_Store_Location_Is_Active] DEFAULT (1),
                    [Sort_Order] int NOT NULL CONSTRAINT [DF_Store_Location_Sort_Order] DEFAULT (0));
            IF OBJECT_ID(N'[Store_Department]', N'U') IS NULL
                CREATE TABLE [Store_Department] (
                    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Department] PRIMARY KEY,
                    [Name] nvarchar(80) NOT NULL,
                    [Is_Active] bit NOT NULL CONSTRAINT [DF_Store_Department_Is_Active] DEFAULT (1));
            IF OBJECT_ID(N'[Store_Stock]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Store_Stock] (
                    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Stock] PRIMARY KEY,
                    [Location_ID] int NOT NULL,
                    [Product_ID] int NOT NULL,
                    [Batch_No] nvarchar(50) NULL,
                    [Expire_Date] date NULL,
                    [Balance] decimal(18,2) NOT NULL,
                    [Buy_Rate] decimal(18,2) NOT NULL,
                    [Received_Date] datetime NOT NULL CONSTRAINT [DF_Store_Stock_Received_Date] DEFAULT (GETDATE()));
                CREATE NONCLUSTERED INDEX [IX_Store_Stock_Location_Product] ON [Store_Stock]([Location_ID], [Product_ID]) INCLUDE ([Balance], [Expire_Date]);
            END
            IF OBJECT_ID(N'[Store_Stock_Ledger]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Store_Stock_Ledger] (
                    [ID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Stock_Ledger] PRIMARY KEY,
                    [Trans_Date] datetime NOT NULL,
                    [Location_ID] int NOT NULL,
                    [Product_ID] int NOT NULL,
                    [Stock_ID] int NULL,
                    [Qty_In] decimal(18,2) NOT NULL,
                    [Qty_Out] decimal(18,2) NOT NULL,
                    [Rate] decimal(18,2) NOT NULL,
                    [Ref_Type] nvarchar(20) NOT NULL,
                    [Ref_ID] bigint NOT NULL,
                    [Department_ID] int NULL,
                    [Note] nvarchar(200) NULL,
                    [Entered_By] int NOT NULL);
                CREATE NONCLUSTERED INDEX [IX_Store_Stock_Ledger_Product_Date] ON [Store_Stock_Ledger]([Product_ID], [Trans_Date]);
                CREATE NONCLUSTERED INDEX [IX_Store_Stock_Ledger_Date_Type] ON [Store_Stock_Ledger]([Trans_Date], [Ref_Type]) INCLUDE ([Department_ID], [Qty_In], [Qty_Out], [Rate], [Product_ID]);
            END
            IF OBJECT_ID(N'[Store_Requisition]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Store_Requisition] (
                    [ID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Requisition] PRIMARY KEY,
                    [Department_ID] int NOT NULL,
                    [Location_ID] int NOT NULL,
                    [Status] nvarchar(20) NOT NULL,
                    [Priority] nvarchar(10) NOT NULL,
                    [Note] nvarchar(300) NULL,
                    [Requested_By] int NOT NULL,
                    [Requested_At] datetime NOT NULL,
                    [Reviewed_By] int NULL,
                    [Reviewed_At] datetime NULL,
                    [Review_Note] nvarchar(300) NULL);
                CREATE NONCLUSTERED INDEX [IX_Store_Requisition_Status] ON [Store_Requisition]([Status], [Requested_At]);
            END
            IF OBJECT_ID(N'[Store_Requisition_Sub]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Store_Requisition_Sub] (
                    [ID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Requisition_Sub] PRIMARY KEY,
                    [Requisition_ID] bigint NOT NULL,
                    [Product_ID] int NOT NULL,
                    [Qty_Requested] decimal(18,2) NOT NULL,
                    [Qty_Approved] decimal(18,2) NULL,
                    [Qty_Issued] decimal(18,2) NOT NULL CONSTRAINT [DF_Store_Requisition_Sub_Qty_Issued] DEFAULT (0));
                CREATE NONCLUSTERED INDEX [IX_Store_Requisition_Sub_Req] ON [Store_Requisition_Sub]([Requisition_ID]);
            END
            IF OBJECT_ID(N'[Store_Movement]', N'U') IS NULL
                CREATE TABLE [Store_Movement] (
                    [ID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Movement] PRIMARY KEY,
                    [Type] nvarchar(10) NOT NULL,
                    [Location_ID] int NOT NULL,
                    [To_Location_ID] int NULL,
                    [Department_ID] int NULL,
                    [Reason] nvarchar(40) NULL,
                    [Note] nvarchar(300) NULL,
                    [Entered_By] int NOT NULL,
                    [Trans_Date] datetime NOT NULL);
            IF OBJECT_ID(N'[Store_Movement_Sub]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Store_Movement_Sub] (
                    [ID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Store_Movement_Sub] PRIMARY KEY,
                    [Movement_ID] bigint NOT NULL,
                    [Product_ID] int NOT NULL,
                    [Stock_ID] int NULL,
                    [Batch_No] nvarchar(50) NULL,
                    [Expire_Date] date NULL,
                    [Quantity] decimal(18,2) NOT NULL,
                    [Qty_Before] decimal(18,2) NULL,
                    [Rate] decimal(18,2) NOT NULL,
                    [Is_Restocked] bit NOT NULL CONSTRAINT [DF_Store_Movement_Sub_Is_Restocked] DEFAULT (0));
                CREATE NONCLUSTERED INDEX [IX_Store_Movement_Sub_Movement] ON [Store_Movement_Sub]([Movement_ID]);
            END

            -- Seed sub-stores
            IF NOT EXISTS (SELECT 1 FROM [Store_Location])
                INSERT INTO [Store_Location] ([Name], [Is_Central], [Is_Active], [Sort_Order]) VALUES
                    (N'Central Store', 1, 1, 0), (N'Laboratory Store', 0, 1, 1), (N'OT Store', 0, 1, 2);

            -- Seed departments / cost centres (clinical departments + common service units)
            IF NOT EXISTS (SELECT 1 FROM [Store_Department])
            BEGIN
                INSERT INTO [Store_Department] ([Name])
                SELECT DISTINCT LEFT(LTRIM(RTRIM([Name])), 80) FROM [Department]
                WHERE [Name] IS NOT NULL AND LTRIM(RTRIM([Name])) NOT IN (N'', N'Department');
                INSERT INTO [Store_Department] ([Name])
                SELECT v.[Name] FROM (VALUES (N'Laboratory'), (N'Radiology & Imaging'), (N'Operation Theatre'), (N'Emergency'),
                    (N'ICU'), (N'Ward / Nursing Station'), (N'Housekeeping'), (N'Administration'), (N'Reception & Front Desk'),
                    (N'Pharmacy'), (N'Kitchen & Canteen'), (N'Maintenance')) v([Name])
                WHERE NOT EXISTS (SELECT 1 FROM [Store_Department] d WHERE d.[Name] = v.[Name]);
            END

            -- Seed common item categories
            INSERT INTO [Store_Product_Category] ([Name])
            SELECT v.[Name] FROM (VALUES (N'Medical Consumables'), (N'Surgical & OT Items'), (N'Lab Reagents & Kits'),
                (N'Linen & Uniform'), (N'Housekeeping & Cleaning'), (N'Stationery & Printing'), (N'Equipment & Spare Parts')) v([Name])
            WHERE NOT EXISTS (SELECT 1 FROM [Store_Product_Category] c WHERE c.[Name] = v.[Name]);

            -- Opening stock: move legacy Store_Product.Balance into the Central Store once
            DECLARE @central int = (SELECT TOP 1 [ID] FROM [Store_Location] WHERE [Is_Central] = 1 ORDER BY [Sort_Order], [ID]);
            IF @central IS NOT NULL
            BEGIN
                DECLARE @opening TABLE ([StockID] int, [ProductID] int, [Qty] decimal(18,2), [Rate] decimal(18,2));
                INSERT INTO [Store_Stock] ([Location_ID], [Product_ID], [Batch_No], [Expire_Date], [Balance], [Buy_Rate], [Received_Date])
                OUTPUT INSERTED.[ID], INSERTED.[Product_ID], INSERTED.[Balance], INSERTED.[Buy_Rate] INTO @opening
                SELECT @central, p.[ID], N'OPENING', NULL, p.[Balance], p.[Buy_Rate], GETDATE()
                FROM [Store_Product] p
                WHERE p.[Balance] > 0 AND NOT EXISTS (SELECT 1 FROM [Store_Stock] s WHERE s.[Product_ID] = p.[ID])
                  AND NOT EXISTS (SELECT 1 FROM [Store_Stock_Ledger] l WHERE l.[Product_ID] = p.[ID]);
                INSERT INTO [Store_Stock_Ledger] ([Trans_Date], [Location_ID], [Product_ID], [Stock_ID], [Qty_In], [Qty_Out], [Rate], [Ref_Type], [Ref_ID], [Note], [Entered_By])
                SELECT GETDATE(), @central, o.[ProductID], o.[StockID], o.[Qty], 0, o.[Rate], N'OPENING', 0, N'Opening balance from legacy store', 1 FROM @opening o;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Service]') AND name = 'SKU')
            BEGIN
                ALTER TABLE [Service] ADD [SKU] nvarchar(50) NULL;
            END
            EXEC(N'UPDATE [Service] SET [SKU] = CONCAT(''MED-'', RIGHT(''00000'' + CAST([ID] AS varchar(10)), 5)) WHERE [Category_ID] = 3 AND ([SKU] IS NULL OR [SKU] = '''');');
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Service_Category_ID_SKU' AND object_id = OBJECT_ID(N'[Service]'))
                EXEC(N'CREATE UNIQUE NONCLUSTERED INDEX [IX_Service_Category_ID_SKU] ON [Service]([Category_ID], [SKU]) WHERE [SKU] IS NOT NULL AND [SKU] <> '''';');
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Service_Category_ID_Generic_ID' AND object_id = OBJECT_ID(N'[Service]'))
                CREATE NONCLUSTERED INDEX [IX_Service_Category_ID_Generic_ID] ON [Service]([Category_ID], [Generic_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Service_Category_ID_Type_ID' AND object_id = OBJECT_ID(N'[Service]'))
                CREATE NONCLUSTERED INDEX [IX_Service_Category_ID_Type_ID] ON [Service]([Category_ID], [Type_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Service_Category_ID_Supplier_ID' AND object_id = OBJECT_ID(N'[Service]'))
                CREATE NONCLUSTERED INDEX [IX_Service_Category_ID_Supplier_ID] ON [Service]([Category_ID], [Supplier_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Medicine_Store_Balance_Service_Expire' AND object_id = OBJECT_ID(N'[Medicine_Store_Balance]'))
                CREATE NONCLUSTERED INDEX [IX_Medicine_Store_Balance_Service_Expire] ON [Medicine_Store_Balance]([Service_ID], [Expire_Date]) INCLUDE ([Balance], [Medicine_Store_ID]);
            IF OBJECT_ID(N'[AspNetUsers]', N'U') IS NOT NULL
            BEGIN
                UPDATE [AspNetUsers]
                SET [DisplayName] = [UserName]
                WHERE [DisplayName] IN (N'Hospital Owner', N'App Owner', N'System Administrator', N'Administrator', N'Staff', N'Staff Member', N'')
                  AND [UserName] IS NOT NULL AND [UserName] <> N'';
            END

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
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctor_Appointment]') AND name = 'Sex')
            BEGIN
                ALTER TABLE [Doctor_Appointment] ADD [Sex] nvarchar(50) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Consultation_Time')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [Consultation_Time] nvarchar(100) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Off_Day')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [Off_Day] nvarchar(100) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Consultation_Fee')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [Consultation_Fee] numeric(18,2) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'OPD_Consultation_Fee')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [OPD_Consultation_Fee] numeric(18,2) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Service]') AND name = 'Delivery_Hours')
            BEGIN
                ALTER TABLE [Service] ADD [Delivery_Hours] int NULL;
            END

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Registration_Contact' AND object_id = OBJECT_ID(N'[Registration]'))
                CREATE INDEX [IX_Registration_Contact] ON [Registration] ([Contact]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Registration_Name' AND object_id = OBJECT_ID(N'[Registration]'))
                CREATE INDEX [IX_Registration_Name] ON [Registration] ([Name]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Registration_Trans_Date' AND object_id = OBJECT_ID(N'[Registration]'))
                CREATE INDEX [IX_Registration_Trans_Date] ON [Registration] ([Trans_Date]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctor_Appointment_Appt_Date_Status' AND object_id = OBJECT_ID(N'[Doctor_Appointment]'))
                CREATE INDEX [IX_Doctor_Appointment_Appt_Date_Status] ON [Doctor_Appointment] ([Appt_Date], [Status]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctor_Appointment_Patient_ID' AND object_id = OBJECT_ID(N'[Doctor_Appointment]'))
                CREATE INDEX [IX_Doctor_Appointment_Patient_ID] ON [Doctor_Appointment] ([Patient_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctor_Appointment_Contact_No' AND object_id = OBJECT_ID(N'[Doctor_Appointment]'))
                CREATE INDEX [IX_Doctor_Appointment_Contact_No] ON [Doctor_Appointment] ([Contact_No]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OPD_Ticket_Trans_Date' AND object_id = OBJECT_ID(N'[OPD_Ticket]'))
                CREATE INDEX [IX_OPD_Ticket_Trans_Date] ON [OPD_Ticket] ([Trans_Date]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OPD_Ticket_Reg_No' AND object_id = OBJECT_ID(N'[OPD_Ticket]'))
                CREATE INDEX [IX_OPD_Ticket_Reg_No] ON [OPD_Ticket] ([Reg_No]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OPD_Ticket_Reg_No_Doctor_ID_Trans_Date' AND object_id = OBJECT_ID(N'[OPD_Ticket]'))
                CREATE INDEX [IX_OPD_Ticket_Reg_No_Doctor_ID_Trans_Date] ON [OPD_Ticket] ([Reg_No], [Doctor_ID], [Trans_Date]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Module_ID_Patient_Type_ID' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Module_ID_Patient_Type_ID] ON [Cash_Memo_DInvestigation] ([Module_ID], [Patient_Type], [ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Module_ID_Trans_Date' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Module_ID_Trans_Date] ON [Cash_Memo_DInvestigation] ([Module_ID], [Trans_Date]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Patient_ID' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Patient_ID] ON [Cash_Memo_DInvestigation] ([Patient_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Reg_No' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Reg_No] ON [Cash_Memo_DInvestigation] ([Reg_No]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_OT_Trans_Date' AND object_id = OBJECT_ID(N'[Cash_Memo_OT]'))
                CREATE INDEX [IX_Cash_Memo_OT_Trans_Date] ON [Cash_Memo_OT] ([Trans_Date]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_OT_Patient_ID' AND object_id = OBJECT_ID(N'[Cash_Memo_OT]'))
                CREATE INDEX [IX_Cash_Memo_OT_Patient_ID] ON [Cash_Memo_OT] ([Patient_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_OT_Reg_No' AND object_id = OBJECT_ID(N'[Cash_Memo_OT]'))
                CREATE INDEX [IX_Cash_Memo_OT_Reg_No] ON [Cash_Memo_OT] ([Reg_No]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Service_Category_ID_Name' AND object_id = OBJECT_ID(N'[Service]'))
                CREATE INDEX [IX_Service_Category_ID_Name] ON [Service] ([Category_ID], [Name]);

            IF OBJECT_ID(N'[Patient_Operation]', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Patient_Operation_Operation_Date' AND object_id = OBJECT_ID(N'[Patient_Operation]'))
                    CREATE INDEX [IX_Patient_Operation_Operation_Date] ON [Patient_Operation] ([Operation_Date]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Patient_Operation_Patient_ID' AND object_id = OBJECT_ID(N'[Patient_Operation]'))
                    CREATE INDEX [IX_Patient_Operation_Patient_ID] ON [Patient_Operation] ([Patient_ID]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Patient_Operation_Reg_No' AND object_id = OBJECT_ID(N'[Patient_Operation]'))
                    CREATE INDEX [IX_Patient_Operation_Reg_No] ON [Patient_Operation] ([Reg_No]);
            END

            IF OBJECT_ID(N'[Operation_Note]', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Operation_Note_Operation_Date' AND object_id = OBJECT_ID(N'[Operation_Note]'))
                    CREATE INDEX [IX_Operation_Note_Operation_Date] ON [Operation_Note] ([Operation_Date]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Operation_Note_Patient_ID' AND object_id = OBJECT_ID(N'[Operation_Note]'))
                    CREATE INDEX [IX_Operation_Note_Patient_ID] ON [Operation_Note] ([Patient_ID]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Operation_Note_Reg_No' AND object_id = OBJECT_ID(N'[Operation_Note]'))
                    CREATE INDEX [IX_Operation_Note_Reg_No] ON [Operation_Note] ([Reg_No]);
            END

            IF OBJECT_ID(N'[Medical_Certificate]', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Medical_Certificate_Certificate_Type_Issue_Date' AND object_id = OBJECT_ID(N'[Medical_Certificate]'))
                    CREATE INDEX [IX_Medical_Certificate_Certificate_Type_Issue_Date] ON [Medical_Certificate] ([Certificate_Type], [Issue_Date]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Medical_Certificate_Patient_ID' AND object_id = OBJECT_ID(N'[Medical_Certificate]'))
                    CREATE INDEX [IX_Medical_Certificate_Patient_ID] ON [Medical_Certificate] ([Patient_ID]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Medical_Certificate_Reg_No' AND object_id = OBJECT_ID(N'[Medical_Certificate]'))
                    CREATE INDEX [IX_Medical_Certificate_Reg_No] ON [Medical_Certificate] ([Reg_No]);
            END
        """;
}
