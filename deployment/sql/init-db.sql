IF DB_ID(N'HPM_Web') IS NULL
BEGIN
    CREATE DATABASE [HPM_Web];
END
GO

IF SUSER_ID(N'HpmApp') IS NULL
BEGIN
    CREATE LOGIN [HpmApp] WITH PASSWORD = N'$(AppPassword)', CHECK_POLICY = ON;
END
GO

USE [HPM_Web];
GO

IF USER_ID(N'HpmApp') IS NULL
BEGIN
    CREATE USER [HpmApp] FOR LOGIN [HpmApp];
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.database_role_members drm
    INNER JOIN sys.database_principals role_principal ON role_principal.principal_id = drm.role_principal_id
    INNER JOIN sys.database_principals member_principal ON member_principal.principal_id = drm.member_principal_id
    WHERE role_principal.name = N'db_owner' AND member_principal.name = N'HpmApp'
)
BEGIN
    ALTER ROLE [db_owner] ADD MEMBER [HpmApp];
END
GO
