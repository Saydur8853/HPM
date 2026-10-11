IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Cash_Memo_DInvestigation] (
        [ID] bigint NOT NULL IDENTITY,
        [Patient_Name] nvarchar(100) NOT NULL,
        [Patient_Type] nvarchar(1) NOT NULL,
        [Bed] nvarchar(10) NULL,
        [Age] nvarchar(50) NULL,
        [Sex] nvarchar(10) NULL,
        [Contact] nvarchar(200) NULL,
        [Total] numeric(18,2) NOT NULL,
        [Discount] numeric(18,2) NOT NULL,
        [Discount_By] nvarchar(50) NULL,
        [Paid] numeric(18,2) NOT NULL,
        [Reg_No] nvarchar(50) NULL,
        [Trans_Date] datetime NOT NULL,
        [Patient_ID] nvarchar(11) NULL,
        [Doctor_ID] int NULL,
        [Module_ID] int NULL,
        [Entered_By] int NOT NULL,
        [Remarks] nvarchar(200) NULL,
        CONSTRAINT [PK_Cash_Memo_DInvestigation] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Cash_Memo_OT] (
        [ID] bigint NOT NULL IDENTITY,
        [Patient_Name] nvarchar(100) NOT NULL,
        [Patient_Type] nvarchar(1) NOT NULL,
        [Bed] nvarchar(10) NULL,
        [Age] nvarchar(50) NULL,
        [Sex] nvarchar(10) NULL,
        [Contact] nvarchar(200) NULL,
        [Patient_ID] nvarchar(11) NULL,
        [Reg_No] nvarchar(50) NULL,
        [Doctor_ID] int NULL,
        [OT] int NULL,
        [Total] numeric(18,2) NOT NULL,
        [Discount] numeric(18,2) NOT NULL,
        [Discount_By] nvarchar(50) NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Cash_Memo_OT] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Department] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Department_Category_ID] int NULL,
        CONSTRAINT [PK_Department] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Doctor_Appointment] (
        [ID] int NOT NULL IDENTITY,
        [Doctor_ID] int NOT NULL,
        [Appt_SLNO] int NOT NULL,
        [Appt_Date] date NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Age] nvarchar(50) NULL,
        [Contact_No] nvarchar(20) NULL,
        [Patient_ID] nvarchar(11) NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Doctor_Appointment] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Doctors_Profile] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NULL,
        [Title] nvarchar(50) NULL,
        [Degree] nvarchar(150) NULL,
        [Attachment] nvarchar(10) NULL DEFAULT N'Temporary',
        [PO_Serial] int NULL,
        [Area_Code] int NULL,
        [Balance] numeric(18,0) NULL,
        [Opening_Balance] numeric(18,0) NULL,
        [Contact] nvarchar(50) NULL,
        [Email] nvarchar(50) NULL,
        [Room] nvarchar(10) NULL,
        [Department] int NULL,
        [Entered_By] int NULL,
        [Trans_Date] datetime NULL,
        [Address] nvarchar(200) NULL,
        [Referral_Category_ID] int NULL,
        CONSTRAINT [PK_Doctors_Profile] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [OPD_Ticket] (
        [ID] bigint NOT NULL IDENTITY,
        [Trans_Date] datetime NOT NULL,
        [Reg_No] nvarchar(50) NULL,
        [Ticket_Rate] nvarchar(50) NULL,
        [Name] nvarchar(50) NOT NULL,
        [Age] nvarchar(50) NULL,
        [Sex] nvarchar(50) NULL,
        [OPD] int NOT NULL,
        [Vat] numeric(18,0) NOT NULL,
        [Doctor_ID] int NOT NULL,
        [Dept_ID] int NOT NULL,
        [Room] nvarchar(50) NULL,
        [Comments] nvarchar(200) NULL,
        [Total] numeric(18,0) NOT NULL,
        [Posted] bit NOT NULL,
        [Entered_By] int NOT NULL,
        CONSTRAINT [PK_OPD_Ticket] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Registration] (
        [ID] bigint NOT NULL IDENTITY,
        [Patient_ID] nvarchar(11) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Date_Of_Birth] datetime NOT NULL,
        [Sex] nvarchar(10) NOT NULL,
        [Blood_Group] nvarchar(10) NULL,
        [Father_Name] nvarchar(100) NULL,
        [Mother_Name] nvarchar(100) NULL,
        [Care_Of] nvarchar(50) NULL,
        [Permanent_Village] nvarchar(200) NULL,
        [Permanent_PO] nvarchar(100) NULL,
        [Permanent_PS] nvarchar(100) NULL,
        [Permanent_District] nvarchar(50) NULL,
        [Present_Village] nvarchar(200) NULL,
        [Present_PO] nvarchar(100) NULL,
        [Present_PS] nvarchar(100) NULL,
        [Present_District] nvarchar(50) NULL,
        [Contact] nvarchar(50) NULL,
        [Email] nvarchar(50) NULL,
        [Occupation] nvarchar(50) NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Registration] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Service] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NULL,
        [Sale_Rate] numeric(18,2) NULL,
        [Buy_Rate] numeric(18,2) NULL,
        [Re_Order_Level] int NULL,
        [Is_VAT_App] bit NULL,
        [Category_ID] int NULL,
        [Type_ID] int NULL,
        [Generic_ID] int NULL,
        [Supplier_ID] int NULL,
        [Investigation_Module_ID] int NULL,
        CONSTRAINT [PK_Service] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Service_Category] (
        [ID] int NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Sub_ID] int NULL,
        [Module_ID] int NULL,
        [Service_Charge] decimal(18,2) NULL,
        [Is_Percentage] bit NULL,
        [Group_Sub_ID] int NULL,
        [Investigation_SLNO] int NULL,
        CONSTRAINT [PK_Service_Category] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Ticket_Type] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Amount] numeric(18,2) NOT NULL,
        CONSTRAINT [PK_Ticket_Type] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Cash_Memo_Sub_DInvestigation] (
        [ID] bigint NOT NULL IDENTITY,
        [Memo_ID] bigint NOT NULL,
        [SLNo] int NOT NULL,
        [Test_ID] int NOT NULL,
        [Rate] numeric(18,0) NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_Cash_Memo_Sub_DInvestigation] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Cash_Memo_Sub_DInvestigation_Cash_Memo_DInvestigation_Memo_ID] FOREIGN KEY ([Memo_ID]) REFERENCES [Cash_Memo_DInvestigation] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE TABLE [Cash_Memo_Sub_OT] (
        [ID] bigint NOT NULL IDENTITY,
        [Memo_ID] bigint NOT NULL,
        [SLNO] int NOT NULL,
        [Service_ID] int NOT NULL,
        [Rate] numeric(18,2) NOT NULL,
        [Quantity] int NOT NULL,
        [Doctor_ID] int NULL,
        CONSTRAINT [PK_Cash_Memo_Sub_OT] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Cash_Memo_Sub_OT_Cash_Memo_OT_Memo_ID] FOREIGN KEY ([Memo_ID]) REFERENCES [Cash_Memo_OT] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Department_Category_ID', N'Name') AND [object_id] = OBJECT_ID(N'[Department]'))
        SET IDENTITY_INSERT [Department] ON;
    EXEC(N'INSERT INTO [Department] ([ID], [Department_Category_ID], [Name])
    VALUES (1, NULL, N''Emergency''),
    (2, NULL, N''Medicine'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Department_Category_ID', N'Name') AND [object_id] = OBJECT_ID(N'[Department]'))
        SET IDENTITY_INSERT [Department] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Address', N'Area_Code', N'Attachment', N'Balance', N'Contact', N'Degree', N'Department', N'Email', N'Entered_By', N'Name', N'Opening_Balance', N'PO_Serial', N'Referral_Category_ID', N'Room', N'Title', N'Trans_Date') AND [object_id] = OBJECT_ID(N'[Doctors_Profile]'))
        SET IDENTITY_INSERT [Doctors_Profile] ON;
    EXEC(N'INSERT INTO [Doctors_Profile] ([ID], [Address], [Area_Code], [Attachment], [Balance], [Contact], [Degree], [Department], [Email], [Entered_By], [Name], [Opening_Balance], [PO_Serial], [Referral_Category_ID], [Room], [Title], [Trans_Date])
    VALUES (1, NULL, NULL, N''Temporary'', NULL, NULL, N''MBBS'', 1, NULL, 1, N''Duty Doctor'', NULL, NULL, NULL, N''ER-01'', N''Dr.'', ''2026-01-01T00:00:00.000'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Address', N'Area_Code', N'Attachment', N'Balance', N'Contact', N'Degree', N'Department', N'Email', N'Entered_By', N'Name', N'Opening_Balance', N'PO_Serial', N'Referral_Category_ID', N'Room', N'Title', N'Trans_Date') AND [object_id] = OBJECT_ID(N'[Doctors_Profile]'))
        SET IDENTITY_INSERT [Doctors_Profile] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Buy_Rate', N'Category_ID', N'Generic_ID', N'Investigation_Module_ID', N'Is_VAT_App', N'Name', N'Re_Order_Level', N'Sale_Rate', N'Supplier_ID', N'Type_ID') AND [object_id] = OBJECT_ID(N'[Service]'))
        SET IDENTITY_INSERT [Service] ON;
    EXEC(N'INSERT INTO [Service] ([ID], [Buy_Rate], [Category_ID], [Generic_ID], [Investigation_Module_ID], [Is_VAT_App], [Name], [Re_Order_Level], [Sale_Rate], [Supplier_ID], [Type_ID])
    VALUES (1, NULL, 6, NULL, NULL, NULL, N''Emergency consultation'', NULL, 500.0, NULL, NULL),
    (2, NULL, 6, NULL, NULL, NULL, N''Nebulization'', NULL, 300.0, NULL, NULL),
    (3, NULL, 1, NULL, NULL, NULL, N''Surgeon charge'', NULL, 10000.0, NULL, NULL),
    (4, NULL, 1, NULL, NULL, NULL, N''Anaesthesia charge'', NULL, 2000.0, NULL, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Buy_Rate', N'Category_ID', N'Generic_ID', N'Investigation_Module_ID', N'Is_VAT_App', N'Name', N'Re_Order_Level', N'Sale_Rate', N'Supplier_ID', N'Type_ID') AND [object_id] = OBJECT_ID(N'[Service]'))
        SET IDENTITY_INSERT [Service] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Group_Sub_ID', N'Investigation_SLNO', N'Is_Percentage', N'Module_ID', N'Name', N'Service_Charge', N'Sub_ID') AND [object_id] = OBJECT_ID(N'[Service_Category]'))
        SET IDENTITY_INSERT [Service_Category] ON;
    EXEC(N'INSERT INTO [Service_Category] ([ID], [Group_Sub_ID], [Investigation_SLNO], [Is_Percentage], [Module_ID], [Name], [Service_Charge], [Sub_ID])
    VALUES (1, NULL, NULL, NULL, NULL, N''Team Charge/Doctor Consultation'', NULL, 1),
    (6, NULL, NULL, NULL, NULL, N''OPD Emergency'', NULL, 0),
    (54, NULL, NULL, NULL, NULL, N''OT Charge'', NULL, 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Group_Sub_ID', N'Investigation_SLNO', N'Is_Percentage', N'Module_ID', N'Name', N'Service_Charge', N'Sub_ID') AND [object_id] = OBJECT_ID(N'[Service_Category]'))
        SET IDENTITY_INSERT [Service_Category] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Amount', N'Name') AND [object_id] = OBJECT_ID(N'[Ticket_Type]'))
        SET IDENTITY_INSERT [Ticket_Type] ON;
    EXEC(N'INSERT INTO [Ticket_Type] ([ID], [Amount], [Name])
    VALUES (1, 50.0, N''Consultation Fee'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Amount', N'Name') AND [object_id] = OBJECT_ID(N'[Ticket_Type]'))
        SET IDENTITY_INSERT [Ticket_Type] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cash_Memo_Sub_DInvestigation_Memo_ID_SLNo] ON [Cash_Memo_Sub_DInvestigation] ([Memo_ID], [SLNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cash_Memo_Sub_OT_Memo_ID_SLNO] ON [Cash_Memo_Sub_OT] ([Memo_ID], [SLNO]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Doctor_Appointment_Doctor_ID_Appt_Date_Appt_SLNO] ON [Doctor_Appointment] ([Doctor_ID], [Appt_Date], [Appt_SLNO]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Registration_Patient_ID] ON [Registration] ([Patient_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921072408_InitialOpdEmergency'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921072408_InitialOpdEmergency', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] int NOT NULL IDENTITY,
        [DisplayName] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [Audit_Log] (
        [ID] bigint NOT NULL IDENTITY,
        [User_ID] int NULL,
        [User_Name] nvarchar(100) NOT NULL,
        [Action] nvarchar(20) NOT NULL,
        [Entity_Name] nvarchar(100) NOT NULL,
        [Entity_ID] nvarchar(100) NULL,
        [Changed_Fields] nvarchar(2000) NULL,
        [Event_Time] datetime2 NOT NULL,
        CONSTRAINT [PK_Audit_Log] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE INDEX [IX_Audit_Log_Entity_Name_Entity_ID] ON [Audit_Log] ([Entity_Name], [Entity_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    CREATE INDEX [IX_Audit_Log_Event_Time] ON [Audit_Log] ([Event_Time]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921100250_AddIdentityAndAudit'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921100250_AddIdentityAndAudit', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE TABLE [Admission] (
        [ID] bigint NOT NULL IDENTITY,
        [Reg_No] nvarchar(10) NOT NULL,
        [Patient_ID] nvarchar(11) NULL,
        [Department] int NULL,
        [Diagnosis] nvarchar(200) NULL,
        [Age] nvarchar(20) NULL,
        [Referred_By] int NULL,
        [Consultant] int NULL,
        [Admitted_By] int NULL,
        [Is_Referred] bit NULL,
        [Bed_Cabin_ID] int NULL,
        [Admission_Fee] numeric(18,2) NULL,
        [Admission_Date] datetime NULL,
        [Release_Date] datetime NULL,
        [Is_Admitted] bit NULL,
        [Bed_Admission_Date] datetime NULL,
        [Comments] nvarchar(200) NULL,
        [Bill_No] bigint NULL,
        [Bill_Generated_By] int NULL,
        [MPO_ID] int NULL,
        [Is_Package] bit NULL,
        [Trans_Date] datetime NULL,
        [Entered_By] int NULL,
        [Ambulance] numeric(18,2) NULL,
        [Outcome] nvarchar(50) NULL,
        [Patient_Category] nvarchar(50) NULL,
        [Patient_Received_By] int NULL,
        [Patient_Received_Date] datetime NULL,
        CONSTRAINT [PK_Admission] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE TABLE [Admission_Ext] (
        [ID] bigint NOT NULL IDENTITY,
        [Reg_No] nvarchar(10) NOT NULL,
        [Service_Charge] numeric(18,2) NULL,
        [Food] numeric(18,2) NULL,
        CONSTRAINT [PK_Admission_Ext] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE TABLE [IPD_Department] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_IPD_Department] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE TABLE [Bed_Category] (
        [ID] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Rate] numeric(18,2) NULL,
        [Specification] nvarchar(200) NULL,
        [IPD_Dept_ID] int NULL,
        [Admission_Fee] numeric(18,2) NULL,
        [Service_Charge] numeric(18,2) NULL,
        [Food] numeric(18,2) NULL,
        CONSTRAINT [PK_Bed_Category] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Bed_Category_IPD_Department_IPD_Dept_ID] FOREIGN KEY ([IPD_Dept_ID]) REFERENCES [IPD_Department] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE TABLE [Bed_Cabin] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Category_ID] int NOT NULL,
        [Is_Allocated] bit NOT NULL,
        CONSTRAINT [PK_Bed_Cabin] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Bed_Cabin_Bed_Category_Category_ID] FOREIGN KEY ([Category_ID]) REFERENCES [Bed_Category] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE TABLE [Bed_Cabin_Transection] (
        [ID] bigint NOT NULL IDENTITY,
        [Bed_Cabin_ID] int NOT NULL,
        [Reg_No] nvarchar(10) NOT NULL,
        [Patient_ID] nvarchar(11) NULL,
        [Rate] numeric(18,2) NULL,
        [Patient_Transaction_ID] bigint NULL,
        [Start_Date] datetime NOT NULL,
        [End_Date] datetime NULL,
        CONSTRAINT [PK_Bed_Cabin_Transection] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Bed_Cabin_Transection_Bed_Cabin_Bed_Cabin_ID] FOREIGN KEY ([Bed_Cabin_ID]) REFERENCES [Bed_Cabin] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Name') AND [object_id] = OBJECT_ID(N'[IPD_Department]'))
        SET IDENTITY_INSERT [IPD_Department] ON;
    EXEC(N'INSERT INTO [IPD_Department] ([ID], [Name])
    VALUES (1, N''General IPD'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Name') AND [object_id] = OBJECT_ID(N'[IPD_Department]'))
        SET IDENTITY_INSERT [IPD_Department] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Admission_Fee', N'Food', N'IPD_Dept_ID', N'Name', N'Rate', N'Service_Charge', N'Specification') AND [object_id] = OBJECT_ID(N'[Bed_Category]'))
        SET IDENTITY_INSERT [Bed_Category] ON;
    EXEC(N'INSERT INTO [Bed_Category] ([ID], [Admission_Fee], [Food], [IPD_Dept_ID], [Name], [Rate], [Service_Charge], [Specification])
    VALUES (1, 500.0, 0.0, 1, N''General Bed'', 1000.0, 0.0, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Admission_Fee', N'Food', N'IPD_Dept_ID', N'Name', N'Rate', N'Service_Charge', N'Specification') AND [object_id] = OBJECT_ID(N'[Bed_Category]'))
        SET IDENTITY_INSERT [Bed_Category] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Category_ID', N'Is_Allocated', N'Name') AND [object_id] = OBJECT_ID(N'[Bed_Cabin]'))
        SET IDENTITY_INSERT [Bed_Cabin] ON;
    EXEC(N'INSERT INTO [Bed_Cabin] ([ID], [Category_ID], [Is_Allocated], [Name])
    VALUES (1, 1, CAST(0 AS bit), N''G-01'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Category_ID', N'Is_Allocated', N'Name') AND [object_id] = OBJECT_ID(N'[Bed_Cabin]'))
        SET IDENTITY_INSERT [Bed_Cabin] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE INDEX [IX_Admission_Is_Admitted_Admission_Date] ON [Admission] ([Is_Admitted], [Admission_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE INDEX [IX_Admission_Patient_ID] ON [Admission] ([Patient_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Admission_Reg_No] ON [Admission] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Admission_Ext_Reg_No] ON [Admission_Ext] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE INDEX [IX_Bed_Cabin_Category_ID] ON [Bed_Cabin] ([Category_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bed_Cabin_Name] ON [Bed_Cabin] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE INDEX [IX_Bed_Cabin_Transection_Bed_Cabin_ID_End_Date] ON [Bed_Cabin_Transection] ([Bed_Cabin_ID], [End_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE INDEX [IX_Bed_Cabin_Transection_Reg_No] ON [Bed_Cabin_Transection] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    CREATE INDEX [IX_Bed_Category_IPD_Dept_ID] ON [Bed_Category] ([IPD_Dept_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921110023_AddIpdAdmissionAndBeds'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921110023_AddIpdAdmissionAndBeds', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    CREATE TABLE [IPD_Money_Receipt] (
        [ID] bigint NOT NULL IDENTITY,
        [Receipt_No] nvarchar(20) NOT NULL,
        [Reg_No] nvarchar(10) NOT NULL,
        [Patient_ID] nvarchar(11) NULL,
        [Amount] numeric(18,2) NOT NULL,
        [Payment_Mode] nvarchar(30) NULL,
        [Trans_Date] datetime NOT NULL,
        [Entered_By] int NULL,
        [Remarks] nvarchar(200) NULL,
        CONSTRAINT [PK_IPD_Money_Receipt] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    CREATE TABLE [IPD_Patient_Service_Item] (
        [ID] bigint NOT NULL IDENTITY,
        [Reg_No] nvarchar(10) NOT NULL,
        [Patient_ID] nvarchar(11) NULL,
        [Service_ID] int NOT NULL,
        [Service_Name] nvarchar(200) NOT NULL,
        [Rate] numeric(18,2) NOT NULL,
        [Quantity] int NOT NULL,
        [Total] numeric(18,2) NOT NULL,
        [Doctor_ID] int NULL,
        [Service_Date] datetime NOT NULL,
        [Entered_By] int NULL,
        [Remarks] nvarchar(200) NULL,
        CONSTRAINT [PK_IPD_Patient_Service_Item] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    CREATE UNIQUE INDEX [IX_IPD_Money_Receipt_Receipt_No] ON [IPD_Money_Receipt] ([Receipt_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    CREATE INDEX [IX_IPD_Money_Receipt_Reg_No] ON [IPD_Money_Receipt] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    CREATE INDEX [IX_IPD_Money_Receipt_Trans_Date] ON [IPD_Money_Receipt] ([Trans_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    CREATE INDEX [IX_IPD_Patient_Service_Item_Reg_No] ON [IPD_Patient_Service_Item] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    CREATE INDEX [IX_IPD_Patient_Service_Item_Service_Date] ON [IPD_Patient_Service_Item] ([Service_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112256_AddIpdReceiptsAndServices'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921112256_AddIpdReceiptsAndServices', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112724_AddDiagnosticAndLabModule'
)
BEGIN
    CREATE TABLE [Lab_Sample_Collection] (
        [ID] bigint NOT NULL IDENTITY,
        [Memo_ID] bigint NOT NULL,
        [Specimen_Type] nvarchar(100) NULL,
        [Collected_At] datetime NULL,
        [Collected_By] int NULL,
        [Status] nvarchar(30) NOT NULL,
        [Remarks] nvarchar(200) NULL,
        CONSTRAINT [PK_Lab_Sample_Collection] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112724_AddDiagnosticAndLabModule'
)
BEGIN
    CREATE TABLE [Lab_Test_Parameter] (
        [ID] int NOT NULL IDENTITY,
        [Service_ID] int NOT NULL,
        [Parameter_Name] nvarchar(100) NOT NULL,
        [Unit] nvarchar(50) NULL,
        [Reference_Range] nvarchar(200) NULL,
        [Sort_Order] int NOT NULL,
        CONSTRAINT [PK_Lab_Test_Parameter] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112724_AddDiagnosticAndLabModule'
)
BEGIN
    CREATE TABLE [Lab_Test_Result] (
        [ID] bigint NOT NULL IDENTITY,
        [Memo_ID] bigint NOT NULL,
        [Service_ID] int NOT NULL,
        [Parameter_Name] nvarchar(100) NOT NULL,
        [Result_Value] nvarchar(100) NOT NULL,
        [Unit] nvarchar(50) NULL,
        [Reference_Range] nvarchar(200) NULL,
        [Flag] nvarchar(30) NULL,
        [Interpretation] nvarchar(200) NULL,
        [Reported_At] datetime NULL,
        [Reported_By] int NULL,
        [Is_Verified] bit NOT NULL,
        CONSTRAINT [PK_Lab_Test_Result] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112724_AddDiagnosticAndLabModule'
)
BEGIN
    CREATE INDEX [IX_Lab_Sample_Collection_Memo_ID] ON [Lab_Sample_Collection] ([Memo_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112724_AddDiagnosticAndLabModule'
)
BEGIN
    CREATE INDEX [IX_Lab_Test_Parameter_Service_ID] ON [Lab_Test_Parameter] ([Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112724_AddDiagnosticAndLabModule'
)
BEGIN
    CREATE INDEX [IX_Lab_Test_Result_Memo_ID_Service_ID] ON [Lab_Test_Result] ([Memo_ID], [Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921112724_AddDiagnosticAndLabModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921112724_AddDiagnosticAndLabModule', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    ALTER TABLE [IPD_Money_Receipt] DROP CONSTRAINT [PK_IPD_Money_Receipt];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    DROP INDEX [IX_IPD_Money_Receipt_Receipt_No] ON [IPD_Money_Receipt];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[IPD_Money_Receipt]') AND [c].[name] = N'Patient_ID');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [IPD_Money_Receipt] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [IPD_Money_Receipt] DROP COLUMN [Patient_ID];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[IPD_Money_Receipt]') AND [c].[name] = N'Payment_Mode');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [IPD_Money_Receipt] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [IPD_Money_Receipt] DROP COLUMN [Payment_Mode];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[IPD_Money_Receipt]') AND [c].[name] = N'Receipt_No');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [IPD_Money_Receipt] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [IPD_Money_Receipt] DROP COLUMN [Receipt_No];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[IPD_Money_Receipt]') AND [c].[name] = N'Remarks');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [IPD_Money_Receipt] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [IPD_Money_Receipt] DROP COLUMN [Remarks];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    EXEC sp_rename N'[IPD_Money_Receipt]', N'Money_Receipt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    EXEC sp_rename N'[Money_Receipt].[IX_IPD_Money_Receipt_Trans_Date]', N'IX_Money_Receipt_Trans_Date', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    EXEC sp_rename N'[Money_Receipt].[IX_IPD_Money_Receipt_Reg_No]', N'IX_Money_Receipt_Reg_No', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Money_Receipt]') AND [c].[name] = N'Entered_By');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Money_Receipt] DROP CONSTRAINT [' + @var4 + '];');
    EXEC(N'UPDATE [Money_Receipt] SET [Entered_By] = 0 WHERE [Entered_By] IS NULL');
    ALTER TABLE [Money_Receipt] ALTER COLUMN [Entered_By] int NOT NULL;
    ALTER TABLE [Money_Receipt] ADD DEFAULT 0 FOR [Entered_By];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    ALTER TABLE [Money_Receipt] ADD [Against] nvarchar(50) NOT NULL DEFAULT N'Advance';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    ALTER TABLE [Money_Receipt] ADD CONSTRAINT [PK_Money_Receipt] PRIMARY KEY ([ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE TABLE [Central_Cash_Collection] (
        [ID] bigint NOT NULL IDENTITY,
        [Amount] numeric(18,2) NOT NULL,
        [Module_Flag] int NOT NULL,
        [Head_ID] int NOT NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        [Trans_Date_CL] datetime NOT NULL,
        [Reference_ID] bigint NULL,
        CONSTRAINT [PK_Central_Cash_Collection] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE TABLE [Patient_Package_Transaction] (
        [ID] bigint NOT NULL IDENTITY,
        [Reg_No] nvarchar(10) NOT NULL,
        [Service_ID] int NOT NULL,
        [Service_Rate] numeric(18,2) NOT NULL,
        [Quantity] numeric(18,2) NOT NULL,
        [IPD_Dept_ID] int NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Patient_Package_Transaction] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE TABLE [Patient_Payment] (
        [ID] bigint NOT NULL IDENTITY,
        [MR_ID] bigint NOT NULL,
        [Reg_No] nvarchar(10) NOT NULL,
        [Amount] numeric(18,2) NOT NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        [IS_Hospital_Payment] bit NOT NULL,
        CONSTRAINT [PK_Patient_Payment] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE TABLE [Patient_Transaction] (
        [ID] bigint NOT NULL IDENTITY,
        [Reg_No] nvarchar(10) NOT NULL,
        [Service_ID] int NOT NULL,
        [Service_Rate] numeric(18,2) NOT NULL,
        [Quantity] numeric(18,2) NOT NULL,
        [IPD_Dept_ID] int NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Patient_Transaction] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE INDEX [IX_Central_Cash_Collection_Reference_ID] ON [Central_Cash_Collection] ([Reference_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE INDEX [IX_Patient_Package_Transaction_Reg_No] ON [Patient_Package_Transaction] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE INDEX [IX_Patient_Payment_MR_ID] ON [Patient_Payment] ([MR_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE INDEX [IX_Patient_Payment_Reg_No] ON [Patient_Payment] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    CREATE INDEX [IX_Patient_Transaction_Reg_No] ON [Patient_Transaction] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921200318_AlignIpdTransferAndMoneyReceiptWithLegacy', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE TABLE [Generic_Group] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NULL,
        [Entered_By] int NULL,
        [Trans_Date] datetime NULL,
        CONSTRAINT [PK_Generic_Group] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE TABLE [Medicine_Store] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Is_Active] bit NOT NULL,
        [Is_Buffer_Store] bit NOT NULL,
        CONSTRAINT [PK_Medicine_Store] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE TABLE [Medicine_Type] (
        [ID] int NOT NULL IDENTITY,
        [Type] nvarchar(100) NOT NULL,
        [Entered_By] int NULL,
        [Trans_Date] datetime NULL,
        CONSTRAINT [PK_Medicine_Type] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE TABLE [Supplier_Pharmacy] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NULL,
        [Address] nvarchar(100) NULL,
        [Contact] nvarchar(50) NULL,
        [Amount] numeric(18,2) NULL,
        [Trans_Date] datetime NULL,
        CONSTRAINT [PK_Supplier_Pharmacy] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE TABLE [Medicine_Store_Balance] (
        [ID] int NOT NULL IDENTITY,
        [Service_ID] int NOT NULL,
        [Medicine_Store_ID] int NOT NULL,
        [Balance] numeric(18,2) NOT NULL,
        [Expire_Date] date NULL,
        [Medicine_Receiving_Challan_Sub_ID] bigint NULL,
        [Batch_No] nvarchar(50) NULL,
        CONSTRAINT [PK_Medicine_Store_Balance] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Medicine_Store_Balance_Medicine_Store_Medicine_Store_ID] FOREIGN KEY ([Medicine_Store_ID]) REFERENCES [Medicine_Store] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Medicine_Store_Balance_Service_Service_ID] FOREIGN KEY ([Service_ID]) REFERENCES [Service] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Entered_By', N'Name', N'Trans_Date') AND [object_id] = OBJECT_ID(N'[Generic_Group]'))
        SET IDENTITY_INSERT [Generic_Group] ON;
    EXEC(N'INSERT INTO [Generic_Group] ([ID], [Entered_By], [Name], [Trans_Date])
    VALUES (1, NULL, N''Others'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Entered_By', N'Name', N'Trans_Date') AND [object_id] = OBJECT_ID(N'[Generic_Group]'))
        SET IDENTITY_INSERT [Generic_Group] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Is_Active', N'Is_Buffer_Store', N'Name') AND [object_id] = OBJECT_ID(N'[Medicine_Store]'))
        SET IDENTITY_INSERT [Medicine_Store] ON;
    EXEC(N'INSERT INTO [Medicine_Store] ([ID], [Is_Active], [Is_Buffer_Store], [Name])
    VALUES (1, CAST(1 AS bit), CAST(0 AS bit), N''Main Pharmacy'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Is_Active', N'Is_Buffer_Store', N'Name') AND [object_id] = OBJECT_ID(N'[Medicine_Store]'))
        SET IDENTITY_INSERT [Medicine_Store] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Entered_By', N'Trans_Date', N'Type') AND [object_id] = OBJECT_ID(N'[Medicine_Type]'))
        SET IDENTITY_INSERT [Medicine_Type] ON;
    EXEC(N'INSERT INTO [Medicine_Type] ([ID], [Entered_By], [Trans_Date], [Type])
    VALUES (1, NULL, NULL, N''CAPSULE'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Entered_By', N'Trans_Date', N'Type') AND [object_id] = OBJECT_ID(N'[Medicine_Type]'))
        SET IDENTITY_INSERT [Medicine_Type] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Group_Sub_ID', N'Investigation_SLNO', N'Is_Percentage', N'Module_ID', N'Name', N'Service_Charge', N'Sub_ID') AND [object_id] = OBJECT_ID(N'[Service_Category]'))
        SET IDENTITY_INSERT [Service_Category] ON;
    EXEC(N'INSERT INTO [Service_Category] ([ID], [Group_Sub_ID], [Investigation_SLNO], [Is_Percentage], [Module_ID], [Name], [Service_Charge], [Sub_ID])
    VALUES (3, NULL, NULL, NULL, NULL, N''Medicine'', NULL, 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Group_Sub_ID', N'Investigation_SLNO', N'Is_Percentage', N'Module_ID', N'Name', N'Service_Charge', N'Sub_ID') AND [object_id] = OBJECT_ID(N'[Service_Category]'))
        SET IDENTITY_INSERT [Service_Category] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE INDEX [IX_Generic_Group_Name] ON [Generic_Group] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Medicine_Store_Name] ON [Medicine_Store] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE INDEX [IX_Medicine_Store_Balance_Medicine_Store_ID_Service_ID_Batch_No_Expire_Date] ON [Medicine_Store_Balance] ([Medicine_Store_ID], [Service_ID], [Batch_No], [Expire_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE INDEX [IX_Medicine_Store_Balance_Service_ID] ON [Medicine_Store_Balance] ([Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE INDEX [IX_Medicine_Type_Type] ON [Medicine_Type] ([Type]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    CREATE INDEX [IX_Supplier_Pharmacy_Name] ON [Supplier_Pharmacy] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921202925_AddPharmacyMastersAndStockFoundation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921202925_AddPharmacyMastersAndStockFoundation', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE TABLE [Cash_Memo] (
        [ID] bigint NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Contact] nvarchar(20) NULL,
        [Reg_No] nvarchar(10) NULL,
        [Patient_Type] nvarchar(1) NOT NULL,
        [Total] numeric(18,2) NULL,
        [Discount] numeric(18,2) NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        [Is_Package] bit NOT NULL,
        [Is_Credit] bit NULL,
        [Buffer_Store] int NULL,
        CONSTRAINT [PK_Cash_Memo] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE TABLE [Medicine_Receiving_Challan] (
        [ID] bigint NOT NULL IDENTITY,
        [Supplier] int NOT NULL,
        [Invoice_No] nvarchar(20) NOT NULL,
        [Invoice_Date] datetime NOT NULL,
        [Is_Posted] bit NOT NULL,
        [Total] numeric(18,2) NOT NULL,
        [Discount] numeric(18,2) NOT NULL,
        [VAT] numeric(18,2) NOT NULL,
        [Store_ID] int NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Medicine_Receiving_Challan] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE TABLE [Supplier_Ledger] (
        [ID] bigint NOT NULL IDENTITY,
        [Supplier_ID] int NOT NULL,
        [Challan_No] bigint NULL,
        [Supplier_Payment_ID] bigint NULL,
        [Debit] decimal(18,2) NULL,
        [Credit] decimal(18,2) NULL,
        [Balance] decimal(18,2) NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Supplier_Ledger] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE TABLE [Cash_Memo_Sub] (
        [ID] bigint NOT NULL IDENTITY,
        [Bill_No] bigint NOT NULL,
        [Service_ID] int NULL,
        [SLNO] int NULL,
        [Rate] numeric(18,2) NULL,
        [Quantity] numeric(18,2) NULL,
        [Buy_Rate] numeric(18,2) NULL,
        CONSTRAINT [PK_Cash_Memo_Sub] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Cash_Memo_Sub_Cash_Memo_Bill_No] FOREIGN KEY ([Bill_No]) REFERENCES [Cash_Memo] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE TABLE [Medicine_Receiving_Challan_Sub] (
        [ID] bigint NOT NULL IDENTITY,
        [Challan_No] bigint NOT NULL,
        [Service_ID] int NOT NULL,
        [SLNO] int NOT NULL,
        [Total] numeric(18,2) NOT NULL,
        [Buy_Rate] numeric(18,2) NOT NULL,
        [Quantity] numeric(18,2) NOT NULL,
        [Discount] numeric(18,2) NOT NULL,
        [VAT] numeric(18,2) NOT NULL,
        [Expire_Date] date NULL,
        [Batch_No] nvarchar(50) NULL,
        [Sale_Rate] numeric(18,2) NULL,
        CONSTRAINT [PK_Medicine_Receiving_Challan_Sub] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Medicine_Receiving_Challan_Sub_Medicine_Receiving_Challan_Challan_No] FOREIGN KEY ([Challan_No]) REFERENCES [Medicine_Receiving_Challan] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE INDEX [IX_Cash_Memo_Trans_Date] ON [Cash_Memo] ([Trans_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Cash_Memo_Sub_Bill_No_SLNO] ON [Cash_Memo_Sub] ([Bill_No], [SLNO]) WHERE [SLNO] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Medicine_Receiving_Challan_Supplier_Invoice_No] ON [Medicine_Receiving_Challan] ([Supplier], [Invoice_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Medicine_Receiving_Challan_Sub_Challan_No_SLNO] ON [Medicine_Receiving_Challan_Sub] ([Challan_No], [SLNO]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    CREATE INDEX [IX_Supplier_Ledger_Supplier_ID_Trans_Date] ON [Supplier_Ledger] ([Supplier_ID], [Trans_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921210632_AddPharmacyPurchaseStockAndPos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921210632_AddPharmacyPurchaseStockAndPos', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE TABLE [Accounts_Chart] (
        [ID] int NOT NULL IDENTITY,
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Account_Type] nvarchar(20) NOT NULL,
        [Parent_ID] int NULL,
        [Is_Active] bit NOT NULL,
        CONSTRAINT [PK_Accounts_Chart] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE TABLE [Accounts_Voucher] (
        [ID] bigint NOT NULL IDENTITY,
        [Voucher_No] nvarchar(20) NOT NULL,
        [Voucher_Type] nvarchar(20) NOT NULL,
        [Voucher_Date] date NOT NULL,
        [Narration] nvarchar(500) NULL,
        [Reference] nvarchar(100) NULL,
        [Is_Posted] bit NOT NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Accounts_Voucher] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE TABLE [Cashier_Handover] (
        [ID] bigint NOT NULL IDENTITY,
        [Business_Date] date NOT NULL,
        [Cashier_ID] int NOT NULL,
        [System_Amount] numeric(18,2) NOT NULL,
        [Handed_Over_Amount] numeric(18,2) NOT NULL,
        [Variance] numeric(18,2) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Cashier_Handover] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE TABLE [Accounts_Voucher_Line] (
        [ID] bigint NOT NULL IDENTITY,
        [Voucher_ID] bigint NOT NULL,
        [Account_ID] int NOT NULL,
        [Debit] numeric(18,2) NOT NULL,
        [Credit] numeric(18,2) NOT NULL,
        CONSTRAINT [PK_Accounts_Voucher_Line] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Accounts_Voucher_Line_Accounts_Chart_Account_ID] FOREIGN KEY ([Account_ID]) REFERENCES [Accounts_Chart] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Accounts_Voucher_Line_Accounts_Voucher_Voucher_ID] FOREIGN KEY ([Voucher_ID]) REFERENCES [Accounts_Voucher] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Account_Type', N'Code', N'Is_Active', N'Name', N'Parent_ID') AND [object_id] = OBJECT_ID(N'[Accounts_Chart]'))
        SET IDENTITY_INSERT [Accounts_Chart] ON;
    EXEC(N'INSERT INTO [Accounts_Chart] ([ID], [Account_Type], [Code], [Is_Active], [Name], [Parent_ID])
    VALUES (1, N''Asset'', N''1000'', CAST(1 AS bit), N''Cash in Hand'', NULL),
    (2, N''Income'', N''4000'', CAST(1 AS bit), N''Hospital Service Revenue'', NULL),
    (3, N''Income'', N''4010'', CAST(1 AS bit), N''Pharmacy Sales'', NULL),
    (4, N''Expense'', N''5000'', CAST(1 AS bit), N''Medicine Purchase'', NULL),
    (5, N''Expense'', N''5100'', CAST(1 AS bit), N''General Expense'', NULL),
    (6, N''Liability'', N''2000'', CAST(1 AS bit), N''Supplier Payable'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Account_Type', N'Code', N'Is_Active', N'Name', N'Parent_ID') AND [object_id] = OBJECT_ID(N'[Accounts_Chart]'))
        SET IDENTITY_INSERT [Accounts_Chart] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Accounts_Chart_Code] ON [Accounts_Chart] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE INDEX [IX_Accounts_Voucher_Voucher_Date] ON [Accounts_Voucher] ([Voucher_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Accounts_Voucher_Voucher_No] ON [Accounts_Voucher] ([Voucher_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE INDEX [IX_Accounts_Voucher_Line_Account_ID_Voucher_ID] ON [Accounts_Voucher_Line] ([Account_ID], [Voucher_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE INDEX [IX_Accounts_Voucher_Line_Voucher_ID] ON [Accounts_Voucher_Line] ([Voucher_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cashier_Handover_Business_Date_Cashier_ID] ON [Cashier_Handover] ([Business_Date], [Cashier_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921211220_AddAccountsAndCashierHandover'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921211220_AddAccountsAndCashierHandover', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    CREATE TABLE [Medicine_Return] (
        [ID] int NOT NULL IDENTITY,
        [Supplier_ID] int NOT NULL,
        [Store_ID] int NOT NULL,
        [Is_Posted] bit NOT NULL,
        [Trans_Date] datetime NOT NULL,
        [Entered_By] int NOT NULL,
        CONSTRAINT [PK_Medicine_Return] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    CREATE TABLE [Supplier_Payment] (
        [ID] bigint NOT NULL IDENTITY,
        [Supplier_ID] int NOT NULL,
        [Amount] numeric(18,2) NOT NULL,
        [Payment_Mode] nvarchar(50) NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Supplier_Payment] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    CREATE TABLE [Medicine_Return_Sub] (
        [ID] int NOT NULL IDENTITY,
        [Medicine_Return_ID] int NOT NULL,
        [Service_ID] int NOT NULL,
        [SLNO] int NOT NULL,
        [Quantity] numeric(18,2) NOT NULL,
        [Buy_Rate] numeric(18,2) NOT NULL,
        CONSTRAINT [PK_Medicine_Return_Sub] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Medicine_Return_Sub_Medicine_Return_Medicine_Return_ID] FOREIGN KEY ([Medicine_Return_ID]) REFERENCES [Medicine_Return] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Account_Type', N'Code', N'Is_Active', N'Name', N'Parent_ID') AND [object_id] = OBJECT_ID(N'[Accounts_Chart]'))
        SET IDENTITY_INSERT [Accounts_Chart] ON;
    EXEC(N'INSERT INTO [Accounts_Chart] ([ID], [Account_Type], [Code], [Is_Active], [Name], [Parent_ID])
    VALUES (7, N''Asset'', N''1100'', CAST(1 AS bit), N''Patient Receivable'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Account_Type', N'Code', N'Is_Active', N'Name', N'Parent_ID') AND [object_id] = OBJECT_ID(N'[Accounts_Chart]'))
        SET IDENTITY_INSERT [Accounts_Chart] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    CREATE INDEX [IX_Medicine_Return_Supplier_ID_Trans_Date] ON [Medicine_Return] ([Supplier_ID], [Trans_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Medicine_Return_Sub_Medicine_Return_ID_SLNO] ON [Medicine_Return_Sub] ([Medicine_Return_ID], [SLNO]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    CREATE INDEX [IX_Supplier_Payment_Supplier_ID_Trans_Date] ON [Supplier_Payment] ([Supplier_ID], [Trans_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922032811_AddPharmacyReturnsSupplierPaymentsAndAutoAccounting', N'8.0.13');
END;
GO

COMMIT;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    ALTER DATABASE CURRENT SET AUTO_CLOSE OFF;
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Medicine_Stock_Adjustment] (
        [ID] bigint NOT NULL IDENTITY,
        [Store_ID] int NOT NULL,
        [Adjustment_Type] nvarchar(30) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Is_Posted] bit NOT NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Medicine_Stock_Adjustment] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Medicine_Stock_Adjustment_Medicine_Store_Store_ID] FOREIGN KEY ([Store_ID]) REFERENCES [Medicine_Store] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Medicine_Stock_Opening] (
        [ID] bigint NOT NULL IDENTITY,
        [Store_ID] int NOT NULL,
        [Service_ID] int NOT NULL,
        [Opening_Balance] numeric(18,2) NOT NULL,
        [Captured_At] datetime NOT NULL,
        CONSTRAINT [PK_Medicine_Stock_Opening] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Medicine_Stock_Opening_Medicine_Store_Store_ID] FOREIGN KEY ([Store_ID]) REFERENCES [Medicine_Store] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Medicine_Stock_Opening_Service_Service_ID] FOREIGN KEY ([Service_ID]) REFERENCES [Service] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Pharmacy_Sales_Return] (
        [ID] bigint NOT NULL IDENTITY,
        [Cash_Memo_ID] bigint NOT NULL,
        [Store_ID] int NOT NULL,
        [Refund_Amount] numeric(18,2) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Is_Posted] bit NOT NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Pharmacy_Sales_Return] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Pharmacy_Sales_Return_Cash_Memo_Cash_Memo_ID] FOREIGN KEY ([Cash_Memo_ID]) REFERENCES [Cash_Memo] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Pharmacy_Sales_Return_Medicine_Store_Store_ID] FOREIGN KEY ([Store_ID]) REFERENCES [Medicine_Store] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Prescription] (
        [ID] bigint NOT NULL IDENTITY,
        [Name] nvarchar(100) NULL,
        [Age] nvarchar(20) NULL,
        [Sex] nvarchar(10) NULL,
        [Blood_Group] nvarchar(10) NULL,
        [Visit_No] int NULL,
        [Visit_Date] datetime NULL,
        [Type] nvarchar(10) NULL,
        [Height] decimal(18,2) NULL,
        [Weight] decimal(18,2) NULL,
        [BMI] nvarchar(10) NULL,
        [Prem] nvarchar(10) NULL,
        [Pulse] nvarchar(20) NULL,
        [Restp] nvarchar(20) NULL,
        [BP] nvarchar(20) NULL,
        [Temperature] nvarchar(10) NULL,
        [Waist] nvarchar(10) NULL,
        [Hip] nvarchar(10) NULL,
        [Complain] nvarchar(max) NULL,
        [Medical_History] nvarchar(max) NULL,
        [Findings] nvarchar(max) NULL,
        [Diagnosis] nvarchar(max) NULL,
        [Investigation] nvarchar(max) NULL,
        [Remark] nvarchar(max) NULL,
        [Medicine] nvarchar(max) NULL,
        [Referred_By] int NULL,
        CONSTRAINT [PK_Prescription] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Prescription_Doctors_Profile_Referred_By] FOREIGN KEY ([Referred_By]) REFERENCES [Doctors_Profile] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Medicine_Stock_Adjustment_Sub] (
        [ID] bigint NOT NULL IDENTITY,
        [Adjustment_ID] bigint NOT NULL,
        [Stock_Balance_ID] int NOT NULL,
        [Service_ID] int NOT NULL,
        [Batch_No] nvarchar(50) NULL,
        [Expire_Date] date NULL,
        [Before_Quantity] numeric(18,2) NOT NULL,
        [Change_Quantity] numeric(18,2) NOT NULL,
        [After_Quantity] numeric(18,2) NOT NULL,
        [Unit_Cost] numeric(18,2) NOT NULL,
        CONSTRAINT [PK_Medicine_Stock_Adjustment_Sub] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Medicine_Stock_Adjustment_Sub_Medicine_Stock_Adjustment_Adjustment_ID] FOREIGN KEY ([Adjustment_ID]) REFERENCES [Medicine_Stock_Adjustment] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Medicine_Stock_Adjustment_Sub_Medicine_Store_Balance_Stock_Balance_ID] FOREIGN KEY ([Stock_Balance_ID]) REFERENCES [Medicine_Store_Balance] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Medicine_Stock_Adjustment_Sub_Service_Service_ID] FOREIGN KEY ([Service_ID]) REFERENCES [Service] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Pharmacy_Sales_Return_Sub] (
        [ID] bigint NOT NULL IDENTITY,
        [Sales_Return_ID] bigint NOT NULL,
        [Cash_Memo_Sub_ID] bigint NOT NULL,
        [Service_ID] int NOT NULL,
        [Stock_Balance_ID] int NULL,
        [Quantity] numeric(18,2) NOT NULL,
        [Rate] numeric(18,2) NOT NULL,
        [Buy_Rate] numeric(18,2) NOT NULL,
        [Is_Restocked] bit NOT NULL,
        CONSTRAINT [PK_Pharmacy_Sales_Return_Sub] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Pharmacy_Sales_Return_Sub_Cash_Memo_Sub_Cash_Memo_Sub_ID] FOREIGN KEY ([Cash_Memo_Sub_ID]) REFERENCES [Cash_Memo_Sub] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Pharmacy_Sales_Return_Sub_Medicine_Store_Balance_Stock_Balance_ID] FOREIGN KEY ([Stock_Balance_ID]) REFERENCES [Medicine_Store_Balance] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Pharmacy_Sales_Return_Sub_Pharmacy_Sales_Return_Sales_Return_ID] FOREIGN KEY ([Sales_Return_ID]) REFERENCES [Pharmacy_Sales_Return] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Pharmacy_Prescription_Dispense] (
        [ID] bigint NOT NULL IDENTITY,
        [Prescription_ID] bigint NOT NULL,
        [Cash_Memo_ID] bigint NOT NULL,
        [Entered_By] int NOT NULL,
        [Trans_Date] datetime NOT NULL,
        CONSTRAINT [PK_Pharmacy_Prescription_Dispense] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Pharmacy_Prescription_Dispense_Cash_Memo_Cash_Memo_ID] FOREIGN KEY ([Cash_Memo_ID]) REFERENCES [Cash_Memo] ([ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Pharmacy_Prescription_Dispense_Prescription_Prescription_ID] FOREIGN KEY ([Prescription_ID]) REFERENCES [Prescription] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Prescription_Medicine] (
        [ID] int NOT NULL IDENTITY,
        [Prescription_ID] bigint NOT NULL,
        [Service_ID] int NOT NULL,
        [Interval] decimal(18,2) NULL,
        [Dose] decimal(18,2) NULL,
        [Days] int NULL,
        [Quantity] int NOT NULL,
        [SLNO] int NOT NULL,
        CONSTRAINT [PK_Prescription_Medicine] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Prescription_Medicine_Prescription_Prescription_ID] FOREIGN KEY ([Prescription_ID]) REFERENCES [Prescription] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Prescription_Medicine_Service_Service_ID] FOREIGN KEY ([Service_ID]) REFERENCES [Service] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE TABLE [Pharmacy_Prescription_Dispense_Sub] (
        [ID] bigint NOT NULL IDENTITY,
        [Dispense_ID] bigint NOT NULL,
        [Prescription_Medicine_ID] int NOT NULL,
        [Service_ID] int NOT NULL,
        [Quantity] numeric(18,2) NOT NULL,
        CONSTRAINT [PK_Pharmacy_Prescription_Dispense_Sub] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Pharmacy_Prescription_Dispense_Sub_Pharmacy_Prescription_Dispense_Dispense_ID] FOREIGN KEY ([Dispense_ID]) REFERENCES [Pharmacy_Prescription_Dispense] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Medicine_Stock_Adjustment_Store_ID_Trans_Date] ON [Medicine_Stock_Adjustment] ([Store_ID], [Trans_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Medicine_Stock_Adjustment_Sub_Adjustment_ID] ON [Medicine_Stock_Adjustment_Sub] ([Adjustment_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Medicine_Stock_Adjustment_Sub_Service_ID] ON [Medicine_Stock_Adjustment_Sub] ([Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Medicine_Stock_Adjustment_Sub_Stock_Balance_ID] ON [Medicine_Stock_Adjustment_Sub] ([Stock_Balance_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Medicine_Stock_Opening_Service_ID] ON [Medicine_Stock_Opening] ([Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Medicine_Stock_Opening_Store_ID_Service_ID] ON [Medicine_Stock_Opening] ([Store_ID], [Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Pharmacy_Prescription_Dispense_Cash_Memo_ID] ON [Pharmacy_Prescription_Dispense] ([Cash_Memo_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Prescription_Dispense_Prescription_ID] ON [Pharmacy_Prescription_Dispense] ([Prescription_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Prescription_Dispense_Sub_Dispense_ID] ON [Pharmacy_Prescription_Dispense_Sub] ([Dispense_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Prescription_Dispense_Sub_Prescription_Medicine_ID] ON [Pharmacy_Prescription_Dispense_Sub] ([Prescription_Medicine_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Sales_Return_Cash_Memo_ID_Trans_Date] ON [Pharmacy_Sales_Return] ([Cash_Memo_ID], [Trans_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Sales_Return_Store_ID] ON [Pharmacy_Sales_Return] ([Store_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Sales_Return_Sub_Cash_Memo_Sub_ID] ON [Pharmacy_Sales_Return_Sub] ([Cash_Memo_Sub_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Sales_Return_Sub_Sales_Return_ID] ON [Pharmacy_Sales_Return_Sub] ([Sales_Return_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Sales_Return_Sub_Stock_Balance_ID] ON [Pharmacy_Sales_Return_Sub] ([Stock_Balance_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Prescription_Referred_By] ON [Prescription] ([Referred_By]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Prescription_Visit_Date] ON [Prescription] ([Visit_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Prescription_Medicine_Prescription_ID_SLNO] ON [Prescription_Medicine] ([Prescription_ID], [SLNO]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    CREATE INDEX [IX_Prescription_Medicine_Service_ID] ON [Prescription_Medicine] ([Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    DECLARE @CapturedAt datetime = GETDATE();
    INSERT INTO dbo.Medicine_Stock_Opening
        (Store_ID, Service_ID, Opening_Balance, Captured_At)
    SELECT Medicine_Store_ID, Service_ID, SUM(Balance), @CapturedAt
    FROM dbo.Medicine_Store_Balance
    GROUP BY Medicine_Store_ID, Service_ID;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922040932_CompletePharmacySalesReturnPrescriptionStockControl', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Cashier_Handover] ADD [Closed_At] datetime NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Cashier_Handover] ADD [Is_Locked] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Cashier_Handover] ADD [Reopen_Reason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Cashier_Handover] ADD [Reopened_At] datetime NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Cashier_Handover] ADD [Reopened_By] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Accounts_Voucher] ADD [Approved_At] datetime NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Accounts_Voucher] ADD [Approved_By] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Accounts_Voucher] ADD [Cancellation_Reason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Accounts_Voucher] ADD [Cancelled_At] datetime NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Accounts_Voucher] ADD [Cancelled_By] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Accounts_Voucher] ADD [Reversal_Of_Voucher_ID] bigint NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    ALTER TABLE [Accounts_Voucher] ADD [Status] nvarchar(20) NOT NULL DEFAULT N'Pending';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    UPDATE [Accounts_Voucher]
    SET [Status] = CASE WHEN [Is_Posted] = 1 THEN 'Approved' ELSE 'Pending' END,
        [Approved_By] = CASE WHEN [Is_Posted] = 1 THEN [Entered_By] ELSE NULL END,
        [Approved_At] = CASE WHEN [Is_Posted] = 1 THEN [Trans_Date] ELSE NULL END;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    CREATE TABLE [Financial_Period_Lock] (
        [ID] int NOT NULL IDENTITY,
        [From_Date] date NOT NULL,
        [To_Date] date NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Is_Active] bit NOT NULL,
        [Locked_By] int NOT NULL,
        [Locked_At] datetime NOT NULL,
        [Unlocked_By] int NULL,
        [Unlocked_At] datetime NULL,
        [Unlock_Reason] nvarchar(500) NULL,
        CONSTRAINT [PK_Financial_Period_Lock] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    CREATE TABLE [Financial_Reversal] (
        [ID] bigint NOT NULL IDENTITY,
        [Entity_Type] nvarchar(50) NOT NULL,
        [Entity_ID] bigint NOT NULL,
        [Amount] numeric(18,2) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Reversal_Voucher_ID] bigint NULL,
        [Reversed_By] int NOT NULL,
        [Reversed_At] datetime NOT NULL,
        CONSTRAINT [PK_Financial_Reversal] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    CREATE INDEX [IX_Accounts_Voucher_Reference] ON [Accounts_Voucher] ([Reference]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Accounts_Voucher_Reversal_Of_Voucher_ID] ON [Accounts_Voucher] ([Reversal_Of_Voucher_ID]) WHERE [Reversal_Of_Voucher_ID] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    CREATE INDEX [IX_Accounts_Voucher_Status] ON [Accounts_Voucher] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    CREATE INDEX [IX_Financial_Period_Lock_Is_Active_From_Date_To_Date] ON [Financial_Period_Lock] ([Is_Active], [From_Date], [To_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Financial_Reversal_Entity_Type_Entity_ID] ON [Financial_Reversal] ([Entity_Type], [Entity_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922095555_AddAccountsControlsAndReversals'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922095555_AddAccountsControlsAndReversals', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922101103_AddPatientAdvanceAccount'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Account_Type', N'Code', N'Is_Active', N'Name', N'Parent_ID') AND [object_id] = OBJECT_ID(N'[Accounts_Chart]'))
        SET IDENTITY_INSERT [Accounts_Chart] ON;
    EXEC(N'INSERT INTO [Accounts_Chart] ([ID], [Account_Type], [Code], [Is_Active], [Name], [Parent_ID])
    VALUES (8, N''Liability'', N''2100'', CAST(1 AS bit), N''Patient Advance'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Account_Type', N'Code', N'Is_Active', N'Name', N'Parent_ID') AND [object_id] = OBJECT_ID(N'[Accounts_Chart]'))
        SET IDENTITY_INSERT [Accounts_Chart] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922101103_AddPatientAdvanceAccount'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922101103_AddPatientAdvanceAccount', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922110257_AddLegacyMigrationAuditTrail'
)
BEGIN
    CREATE TABLE [Legacy_Migration_Run] (
        [ID] uniqueidentifier NOT NULL,
        [Source_Database] nvarchar(200) NOT NULL,
        [Target_Database] nvarchar(200) NOT NULL,
        [Approved_By] nvarchar(100) NOT NULL,
        [Approval_Hash] nvarchar(64) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [Started_At] datetime2 NOT NULL,
        [Completed_At] datetime2 NULL,
        [Is_Rollback_Verification] bit NOT NULL,
        [Summary_Json] nvarchar(max) NULL,
        CONSTRAINT [PK_Legacy_Migration_Run] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922110257_AddLegacyMigrationAuditTrail'
)
BEGIN
    CREATE TABLE [Legacy_Migration_Quarantine] (
        [ID] bigint NOT NULL IDENTITY,
        [Run_ID] uniqueidentifier NOT NULL,
        [Entity_Name] nvarchar(100) NOT NULL,
        [Source_ID] nvarchar(100) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Payload_Json] nvarchar(max) NULL,
        [Created_At] datetime2 NOT NULL,
        CONSTRAINT [PK_Legacy_Migration_Quarantine] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_Legacy_Migration_Quarantine_Legacy_Migration_Run_Run_ID] FOREIGN KEY ([Run_ID]) REFERENCES [Legacy_Migration_Run] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922110257_AddLegacyMigrationAuditTrail'
)
BEGIN
    CREATE INDEX [IX_Legacy_Migration_Quarantine_Run_ID_Entity_Name] ON [Legacy_Migration_Quarantine] ([Run_ID], [Entity_Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922110257_AddLegacyMigrationAuditTrail'
)
BEGIN
    CREATE INDEX [IX_Legacy_Migration_Run_Started_At] ON [Legacy_Migration_Run] ([Started_At]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922110257_AddLegacyMigrationAuditTrail'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922110257_AddLegacyMigrationAuditTrail', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924071217_AddIpdFinalBillPersistence'
)
BEGIN
    CREATE TABLE [IPD_Final_Bill] (
        [Bill_No] bigint NOT NULL IDENTITY,
        [Admission_ID] bigint NOT NULL,
        [Reg_No] nvarchar(10) NOT NULL,
        [Admission_Fee] numeric(18,2) NOT NULL,
        [Bed_Charges] numeric(18,2) NOT NULL,
        [Service_Charges] numeric(18,2) NOT NULL,
        [Hospital_Charges] numeric(18,2) NOT NULL,
        [Ambulance_Charges] numeric(18,2) NOT NULL,
        [Gross_Amount] numeric(18,2) NOT NULL,
        [Discount_Amount] numeric(18,2) NOT NULL,
        [Discount_Authorized_By] nvarchar(100) NULL,
        [Net_Amount] numeric(18,2) NOT NULL,
        [Receipt_Amount] numeric(18,2) NOT NULL,
        [Applied_Advance_Amount] numeric(18,2) NOT NULL,
        [Balance_Due] numeric(18,2) NOT NULL,
        [Excess_Payment] numeric(18,2) NOT NULL,
        [Settlement_Status] nvarchar(20) NOT NULL,
        [Discharge_Date] datetime NOT NULL,
        [Outcome] nvarchar(50) NOT NULL,
        [Discharge_Notes] nvarchar(1000) NULL,
        [Generated_By] int NOT NULL,
        [Generated_At] datetime NOT NULL,
        CONSTRAINT [PK_IPD_Final_Bill] PRIMARY KEY ([Bill_No]),
        CONSTRAINT [CK_IPD_Final_Bill_Amounts] CHECK ([Gross_Amount] >= 0 AND [Discount_Amount] >= 0 AND [Discount_Amount] <= [Gross_Amount] AND [Net_Amount] = [Gross_Amount] - [Discount_Amount] AND [Receipt_Amount] >= 0 AND [Applied_Advance_Amount] >= 0 AND [Balance_Due] >= 0 AND [Excess_Payment] >= 0),
        CONSTRAINT [FK_IPD_Final_Bill_Admission_Admission_ID] FOREIGN KEY ([Admission_ID]) REFERENCES [Admission] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924071217_AddIpdFinalBillPersistence'
)
BEGIN
    CREATE TABLE [IPD_Final_Bill_Receipt] (
        [ID] bigint NOT NULL IDENTITY,
        [Bill_No] bigint NOT NULL,
        [Money_Receipt_ID] bigint NOT NULL,
        [Amount] numeric(18,2) NOT NULL,
        [Against] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_IPD_Final_Bill_Receipt] PRIMARY KEY ([ID]),
        CONSTRAINT [CK_IPD_Final_Bill_Receipt_Amount] CHECK ([Amount] > 0),
        CONSTRAINT [FK_IPD_Final_Bill_Receipt_IPD_Final_Bill_Bill_No] FOREIGN KEY ([Bill_No]) REFERENCES [IPD_Final_Bill] ([Bill_No]) ON DELETE CASCADE,
        CONSTRAINT [FK_IPD_Final_Bill_Receipt_Money_Receipt_Money_Receipt_ID] FOREIGN KEY ([Money_Receipt_ID]) REFERENCES [Money_Receipt] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924071217_AddIpdFinalBillPersistence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_IPD_Final_Bill_Admission_ID] ON [IPD_Final_Bill] ([Admission_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924071217_AddIpdFinalBillPersistence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_IPD_Final_Bill_Reg_No] ON [IPD_Final_Bill] ([Reg_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924071217_AddIpdFinalBillPersistence'
)
BEGIN
    CREATE INDEX [IX_IPD_Final_Bill_Receipt_Bill_No] ON [IPD_Final_Bill_Receipt] ([Bill_No]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924071217_AddIpdFinalBillPersistence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_IPD_Final_Bill_Receipt_Money_Receipt_ID] ON [IPD_Final_Bill_Receipt] ([Money_Receipt_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924071217_AddIpdFinalBillPersistence'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924071217_AddIpdFinalBillPersistence', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924073556_AddClinicalBillingAmountGuards'
)
BEGIN
    EXEC(N'ALTER TABLE [Cash_Memo_Sub_OT] ADD CONSTRAINT [CK_Cash_Memo_Sub_OT_Amounts] CHECK ([Rate] >= 0 AND [Quantity] > 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924073556_AddClinicalBillingAmountGuards'
)
BEGIN
    EXEC(N'ALTER TABLE [Cash_Memo_Sub_DInvestigation] ADD CONSTRAINT [CK_Cash_Memo_Sub_DInvestigation_Amounts] CHECK ([Rate] >= 0 AND [Quantity] > 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924073556_AddClinicalBillingAmountGuards'
)
BEGIN
    EXEC(N'ALTER TABLE [Cash_Memo_OT] ADD CONSTRAINT [CK_Cash_Memo_OT_Amounts] CHECK ([Total] >= 0 AND [Discount] >= 0 AND [Discount] <= [Total])');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924073556_AddClinicalBillingAmountGuards'
)
BEGIN
    EXEC(N'ALTER TABLE [Cash_Memo_DInvestigation] ADD CONSTRAINT [CK_Cash_Memo_DInvestigation_Amounts] CHECK ([Total] >= 0 AND [Discount] >= 0 AND [Discount] <= [Total] AND [Paid] >= 0 AND [Paid] <= [Total] - [Discount])');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924073556_AddClinicalBillingAmountGuards'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924073556_AddClinicalBillingAmountGuards', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924082748_AddOperationalConcurrencyGuards'
)
BEGIN
    DROP INDEX [IX_Prescription_Visit_Date] ON [Prescription];
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescription]') AND [c].[name] = N'Visit_Date');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Prescription] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Prescription] ALTER COLUMN [Visit_Date] date NULL;
    CREATE INDEX [IX_Prescription_Visit_Date] ON [Prescription] ([Visit_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924082748_AddOperationalConcurrencyGuards'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Prescription_Visit_Date_Visit_No] ON [Prescription] ([Visit_Date], [Visit_No]) WHERE [Visit_Date] IS NOT NULL AND [Visit_No] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924082748_AddOperationalConcurrencyGuards'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Bed_Cabin_Transection_Open_Bed] ON [Bed_Cabin_Transection] ([Bed_Cabin_ID]) WHERE [End_Date] IS NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924082748_AddOperationalConcurrencyGuards'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Bed_Cabin_Transection_Open_RegNo] ON [Bed_Cabin_Transection] ([Reg_No]) WHERE [End_Date] IS NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924082748_AddOperationalConcurrencyGuards'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924082748_AddOperationalConcurrencyGuards', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924084752_AddSecurityAndPharmacyBatchTrace'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [MustChangePassword] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924084752_AddSecurityAndPharmacyBatchTrace'
)
BEGIN
    UPDATE [AspNetUsers] SET [MustChangePassword] = 1 WHERE [NormalizedUserName] = 'ADMIN'
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924084752_AddSecurityAndPharmacyBatchTrace'
)
BEGIN
    CREATE TABLE [Pharmacy_Sale_Batch_Allocation] (
        [ID] bigint NOT NULL IDENTITY,
        [Cash_Memo_Sub_ID] bigint NOT NULL,
        [Stock_Balance_ID] int NOT NULL,
        [Quantity] numeric(18,2) NOT NULL,
        [Buy_Rate] numeric(18,2) NOT NULL,
        [Batch_No] nvarchar(50) NULL,
        [Expire_Date] date NULL,
        CONSTRAINT [PK_Pharmacy_Sale_Batch_Allocation] PRIMARY KEY ([ID]),
        CONSTRAINT [CK_Pharmacy_Sale_Batch_Allocation_Amounts] CHECK ([Quantity] > 0 AND [Buy_Rate] >= 0),
        CONSTRAINT [FK_Pharmacy_Sale_Batch_Allocation_Cash_Memo_Sub_Cash_Memo_Sub_ID] FOREIGN KEY ([Cash_Memo_Sub_ID]) REFERENCES [Cash_Memo_Sub] ([ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Pharmacy_Sale_Batch_Allocation_Medicine_Store_Balance_Stock_Balance_ID] FOREIGN KEY ([Stock_Balance_ID]) REFERENCES [Medicine_Store_Balance] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924084752_AddSecurityAndPharmacyBatchTrace'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Pharmacy_Sale_Batch_Allocation_Cash_Memo_Sub_ID_Stock_Balance_ID] ON [Pharmacy_Sale_Batch_Allocation] ([Cash_Memo_Sub_ID], [Stock_Balance_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924084752_AddSecurityAndPharmacyBatchTrace'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_Sale_Batch_Allocation_Stock_Balance_ID_Expire_Date] ON [Pharmacy_Sale_Batch_Allocation] ([Stock_Balance_ID], [Expire_Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924084752_AddSecurityAndPharmacyBatchTrace'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924084752_AddSecurityAndPharmacyBatchTrace', N'8.0.13');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    DROP INDEX [IX_Lab_Sample_Collection_Memo_ID] ON [Lab_Sample_Collection];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    ALTER TABLE [Lab_Sample_Collection] ADD [Service_ID] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    UPDATE sample
    SET [Service_ID] = first_item.[Test_ID]
    FROM [Lab_Sample_Collection] sample
    CROSS APPLY (
        SELECT TOP (1) item.[Test_ID]
        FROM [Cash_Memo_Sub_DInvestigation] item
        WHERE item.[Memo_ID] = sample.[Memo_ID]
        ORDER BY item.[SLNo], item.[ID]
    ) first_item;

    ;WITH ranked AS (
        SELECT [ID], ROW_NUMBER() OVER (
            PARTITION BY [Memo_ID], [Service_ID]
            ORDER BY CASE [Status] WHEN 'Completed' THEN 0 WHEN 'InProcess' THEN 1 ELSE 2 END, [ID]
        ) AS rn
        FROM [Lab_Sample_Collection]
        WHERE [Service_ID] IS NOT NULL
    )
    DELETE sample
    FROM [Lab_Sample_Collection] sample
    INNER JOIN ranked ON ranked.[ID] = sample.[ID]
    WHERE ranked.rn > 1;

    INSERT INTO [Lab_Sample_Collection]
        ([Memo_ID], [Service_ID], [Specimen_Type], [Collected_At], [Collected_By], [Status], [Remarks])
    SELECT ordered.[Memo_ID], ordered.[Test_ID],
        COALESCE(template.[Specimen_Type], 'Blood / Specimen'), template.[Collected_At], template.[Collected_By],
        COALESCE(template.[Status], 'Pending'), template.[Remarks]
    FROM (
        SELECT DISTINCT item.[Memo_ID], item.[Test_ID]
        FROM [Cash_Memo_Sub_DInvestigation] item
        INNER JOIN [Cash_Memo_DInvestigation] memo ON memo.[ID] = item.[Memo_ID]
        WHERE memo.[Module_ID] = 2
    ) ordered
    OUTER APPLY (
        SELECT TOP (1) sample.[Specimen_Type], sample.[Collected_At], sample.[Collected_By], sample.[Status], sample.[Remarks]
        FROM [Lab_Sample_Collection] sample
        WHERE sample.[Memo_ID] = ordered.[Memo_ID]
        ORDER BY CASE sample.[Status] WHEN 'Completed' THEN 0 WHEN 'InProcess' THEN 1 ELSE 2 END, sample.[ID]
    ) template
    WHERE NOT EXISTS (
        SELECT 1 FROM [Lab_Sample_Collection] existing
        WHERE existing.[Memo_ID] = ordered.[Memo_ID] AND existing.[Service_ID] = ordered.[Test_ID]
    );

    DELETE FROM [Lab_Sample_Collection] WHERE [Service_ID] IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Lab_Sample_Collection]') AND [c].[name] = N'Service_ID');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Lab_Sample_Collection] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Lab_Sample_Collection] ALTER COLUMN [Service_ID] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Lab_Sample_Collection_Memo_ID_Service_ID] ON [Lab_Sample_Collection] ([Memo_ID], [Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    CREATE INDEX [IX_Lab_Sample_Collection_Service_ID] ON [Lab_Sample_Collection] ([Service_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    ALTER TABLE [Lab_Sample_Collection] ADD CONSTRAINT [FK_Lab_Sample_Collection_Service_Service_ID] FOREIGN KEY ([Service_ID]) REFERENCES [Service] ([ID]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924090611_AddPerTestDiagnosticSamples'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924090611_AddPerTestDiagnosticSamples', N'8.0.13');
END;
GO

COMMIT;
GO

