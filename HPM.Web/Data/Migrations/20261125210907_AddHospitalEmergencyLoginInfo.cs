using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalEmergencyLoginInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Ambulance_Title')
                    ALTER TABLE [Hospital_Profile] ADD [Ambulance_Title] nvarchar(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Ambulance_Phone')
                    ALTER TABLE [Hospital_Profile] ADD [Ambulance_Phone] nvarchar(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Fire_Service_Phone')
                    ALTER TABLE [Hospital_Profile] ADD [Fire_Service_Phone] nvarchar(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Emergency_Hotline')
                    ALTER TABLE [Hospital_Profile] ADD [Emergency_Hotline] nvarchar(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Blood_Bank_Phone')
                    ALTER TABLE [Hospital_Profile] ADD [Blood_Bank_Phone] nvarchar(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Police_Station_Name')
                    ALTER TABLE [Hospital_Profile] ADD [Police_Station_Name] nvarchar(150) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Police_Station_Phone')
                    ALTER TABLE [Hospital_Profile] ADD [Police_Station_Phone] nvarchar(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Hospital_Branches_Info')
                    ALTER TABLE [Hospital_Profile] ADD [Hospital_Branches_Info] nvarchar(2000) NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Hospital_Branches_Info')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Hospital_Branches_Info];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Police_Station_Phone')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Police_Station_Phone];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Police_Station_Name')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Police_Station_Name];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Blood_Bank_Phone')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Blood_Bank_Phone];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Emergency_Hotline')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Emergency_Hotline];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Fire_Service_Phone')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Fire_Service_Phone];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Ambulance_Phone')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Ambulance_Phone];
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Ambulance_Title')
                    ALTER TABLE [Hospital_Profile] DROP COLUMN [Ambulance_Title];
                """);
        }
    }
}
