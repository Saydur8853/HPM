using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBloodBankTransfusion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blood_Donor",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonorNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    WeightKg = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MedicalNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeferred = table.Column<bool>(type: "bit", nullable: false),
                    DeferredUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeferralReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blood_Donor", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Blood_Requisition",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PatientID = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    PatientName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RegNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ComponentType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UnitsRequested = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ClinicalIndication = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequestedByDoctorID = table.Column<int>(type: "int", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EnteredBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blood_Requisition", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Blood_Donation",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonorID = table.Column<long>(type: "bigint", nullable: false),
                    BagNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VolumeMl = table.Column<int>(type: "int", nullable: false),
                    CollectionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CollectedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blood_Donation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Blood_Donation_Blood_Donor_DonorID",
                        column: x => x.DonorID,
                        principalTable: "Blood_Donor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Blood_Component",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonationID = table.Column<long>(type: "bigint", nullable: false),
                    ComponentNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ComponentType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    VolumeMl = table.Column<int>(type: "int", nullable: false),
                    PreparedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StorageLocation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiscardReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PreparedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blood_Component", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Blood_Component_Blood_Donation_DonationID",
                        column: x => x.DonationID,
                        principalTable: "Blood_Donation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Blood_Screening",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonationID = table.Column<long>(type: "bigint", nullable: false),
                    HIV = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    HepatitisB = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    HepatitisC = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Syphilis = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Malaria = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ScreenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScreenedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blood_Screening", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Blood_Screening_Blood_Donation_DonationID",
                        column: x => x.DonationID,
                        principalTable: "Blood_Donation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Blood_CrossMatch",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionID = table.Column<long>(type: "bigint", nullable: false),
                    ComponentID = table.Column<long>(type: "bigint", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blood_CrossMatch", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Blood_CrossMatch_Blood_Component_ComponentID",
                        column: x => x.ComponentID,
                        principalTable: "Blood_Component",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Blood_CrossMatch_Blood_Requisition_RequisitionID",
                        column: x => x.RequisitionID,
                        principalTable: "Blood_Requisition",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Blood_Issue",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrossMatchID = table.Column<long>(type: "bigint", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WardOrLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IssuedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnDisposition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ReturnedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blood_Issue", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Blood_Issue_Blood_CrossMatch_CrossMatchID",
                        column: x => x.CrossMatchID,
                        principalTable: "Blood_CrossMatch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Component_BloodGroup_ComponentType_Status_ExpiresAt",
                table: "Blood_Component",
                columns: new[] { "BloodGroup", "ComponentType", "Status", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Component_ComponentNo",
                table: "Blood_Component",
                column: "ComponentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Component_DonationID",
                table: "Blood_Component",
                column: "DonationID");

            migrationBuilder.CreateIndex(
                name: "IX_Blood_CrossMatch_ComponentID",
                table: "Blood_CrossMatch",
                column: "ComponentID",
                unique: true,
                filter: "[Result] = N'Compatible'");

            migrationBuilder.CreateIndex(
                name: "IX_Blood_CrossMatch_RequisitionID_ComponentID",
                table: "Blood_CrossMatch",
                columns: new[] { "RequisitionID", "ComponentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Donation_BagNo",
                table: "Blood_Donation",
                column: "BagNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Donation_BloodGroup_Status",
                table: "Blood_Donation",
                columns: new[] { "BloodGroup", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Donation_DonorID",
                table: "Blood_Donation",
                column: "DonorID");

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Donor_ContactNo",
                table: "Blood_Donor",
                column: "ContactNo");

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Donor_DonorNo",
                table: "Blood_Donor",
                column: "DonorNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Issue_CrossMatchID",
                table: "Blood_Issue",
                column: "CrossMatchID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Requisition_PatientID_Status",
                table: "Blood_Requisition",
                columns: new[] { "PatientID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Requisition_RequisitionNo",
                table: "Blood_Requisition",
                column: "RequisitionNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blood_Screening_DonationID",
                table: "Blood_Screening",
                column: "DonationID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blood_Issue");

            migrationBuilder.DropTable(
                name: "Blood_Screening");

            migrationBuilder.DropTable(
                name: "Blood_CrossMatch");

            migrationBuilder.DropTable(
                name: "Blood_Component");

            migrationBuilder.DropTable(
                name: "Blood_Requisition");

            migrationBuilder.DropTable(
                name: "Blood_Donation");

            migrationBuilder.DropTable(
                name: "Blood_Donor");
        }
    }
}
