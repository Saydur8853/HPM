using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkDoctorUsersToProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Doctor_Profile_ID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Doctor_Profile_ID",
                table: "AspNetUsers",
                column: "Doctor_Profile_ID",
                unique: true,
                filter: "[Doctor_Profile_ID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Doctors_Profile_Doctor_Profile_ID",
                table: "AspNetUsers",
                column: "Doctor_Profile_ID",
                principalTable: "Doctors_Profile",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Doctors_Profile_Doctor_Profile_ID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Doctor_Profile_ID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_Profile_ID",
                table: "AspNetUsers");
        }
    }
}
