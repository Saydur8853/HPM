using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceDeliveryHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Delivery_Hours",
                table: "Service",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date_Of_Birth",
                table: "Registration",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<string>(
                name: "Consultation_Time",
                table: "Doctors_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Off_Day",
                table: "Doctors_Profile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Doctors_Profile",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Consultation_Time", "Off_Day" },
                values: new object[] { null, "Friday" });

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 1,
                column: "Delivery_Hours",
                value: null);

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 2,
                column: "Delivery_Hours",
                value: null);

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 3,
                column: "Delivery_Hours",
                value: null);

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "ID",
                keyValue: 4,
                column: "Delivery_Hours",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delivery_Hours",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Consultation_Time",
                table: "Doctors_Profile");

            migrationBuilder.DropColumn(
                name: "Off_Day",
                table: "Doctors_Profile");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date_Of_Birth",
                table: "Registration",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
