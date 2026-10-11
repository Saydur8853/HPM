using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalBrandingAndContactSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "Hospital_Profile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Hospital_Profile",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City_District",
                table: "Hospital_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Hospital_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hotline",
                table: "Hospital_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Emergency_Contact",
                table: "Hospital_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Hospital_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Hospital_Profile",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License_No",
                table: "Hospital_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo_Data",
                table: "Hospital_Profile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lab_Header_Dept",
                table: "Hospital_Profile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Receipt_Header_Note",
                table: "Hospital_Profile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Receipt_Footer_Note",
                table: "Hospital_Profile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated_At",
                table: "Hospital_Profile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Updated_By",
                table: "Hospital_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Tagline", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Address", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "City_District", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Phone", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Hotline", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Emergency_Contact", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Email", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Website", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "License_No", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Logo_Data", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Lab_Header_Dept", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Receipt_Header_Note", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Receipt_Footer_Note", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Updated_At", table: "Hospital_Profile");
            migrationBuilder.DropColumn(name: "Updated_By", table: "Hospital_Profile");
        }
    }
}
