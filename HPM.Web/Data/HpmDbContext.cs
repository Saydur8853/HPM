using HPM.Web.Data.Entities;
using HPM.Web.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace HPM.Web.Data;

public sealed class HpmDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public HpmDbContext(
        DbContextOptions<HpmDbContext> options,
        IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<DoctorAppointment> DoctorAppointments => Set<DoctorAppointment>();
    public DbSet<OPDTicket> OPDTickets => Set<OPDTicket>();
    public DbSet<PatientEncounter> PatientEncounters => Set<PatientEncounter>();
    public DbSet<CashMemoDInvestigation> EmergencyMemos => Set<CashMemoDInvestigation>();
    public DbSet<CashMemoSubDInvestigation> EmergencyMemoItems => Set<CashMemoSubDInvestigation>();
    public DbSet<DiagnosticCashMemoReturn> DiagnosticCashMemoReturns => Set<DiagnosticCashMemoReturn>();
    public DbSet<DiagnosticCashMemoReturnItem> DiagnosticCashMemoReturnItems => Set<DiagnosticCashMemoReturnItem>();
    public DbSet<CashMemoOT> OTCashMemos => Set<CashMemoOT>();
    public DbSet<CashMemoSubOT> OTCashMemoItems => Set<CashMemoSubOT>();
    public DbSet<DoctorsProfile> Doctors => Set<DoctorsProfile>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<PatientAdmission> PatientAdmissions => Set<PatientAdmission>();
    public DbSet<AdmissionExtension> AdmissionExtensions => Set<AdmissionExtension>();
    public DbSet<IpdDepartment> IpdDepartments => Set<IpdDepartment>();
    public DbSet<BedCategory> BedCategories => Set<BedCategory>();
    public DbSet<BedCabin> BedCabins => Set<BedCabin>();
    public DbSet<BedCabinTransaction> BedCabinTransactions => Set<BedCabinTransaction>();
    public DbSet<BedTurnover> BedTurnovers => Set<BedTurnover>();
    public DbSet<IpdMoneyReceipt> IpdMoneyReceipts => Set<IpdMoneyReceipt>();
    public DbSet<IpdFinalBill> IpdFinalBills => Set<IpdFinalBill>();
    public DbSet<IpdMedicalDischargeOrder> IpdMedicalDischargeOrders => Set<IpdMedicalDischargeOrder>();
    public DbSet<IpdFinalBillReceipt> IpdFinalBillReceipts => Set<IpdFinalBillReceipt>();
    public DbSet<PatientTransaction> PatientTransactions => Set<PatientTransaction>();
    public DbSet<PatientPackageTransaction> PatientPackageTransactions => Set<PatientPackageTransaction>();
    public DbSet<PatientPayment> PatientPayments => Set<PatientPayment>();
    public DbSet<CentralCashCollection> CentralCashCollections => Set<CentralCashCollection>();
    public DbSet<IpdPatientServiceItem> IpdPatientServiceItems => Set<IpdPatientServiceItem>();
    public DbSet<PatientOperation> PatientOperations => Set<PatientOperation>();
    public DbSet<OperationNote> OperationNotes => Set<OperationNote>();
    public DbSet<MedicalCertificate> MedicalCertificates => Set<MedicalCertificate>();
    public DbSet<GenericGroup> GenericGroups => Set<GenericGroup>();
    public DbSet<MedicineType> MedicineTypes => Set<MedicineType>();
    public DbSet<SupplierPharmacy> PharmacySuppliers => Set<SupplierPharmacy>();
    public DbSet<MedicineStore> MedicineStores => Set<MedicineStore>();
    public DbSet<MedicineStoreBalance> MedicineStoreBalances => Set<MedicineStoreBalance>();
    public DbSet<MedicineReceivingChallan> MedicineReceivingChallans => Set<MedicineReceivingChallan>();
    public DbSet<MedicineReceivingChallanItem> MedicineReceivingChallanItems => Set<MedicineReceivingChallanItem>();
    public DbSet<PharmacyCashMemo> PharmacyCashMemos => Set<PharmacyCashMemo>();
    public DbSet<PharmacyCashMemoItem> PharmacyCashMemoItems => Set<PharmacyCashMemoItem>();
    public DbSet<PharmacySaleBatchAllocation> PharmacySaleBatchAllocations => Set<PharmacySaleBatchAllocation>();
    public DbSet<SupplierLedger> SupplierLedgers => Set<SupplierLedger>();
    public DbSet<MedicineReturn> MedicineReturns => Set<MedicineReturn>();
    public DbSet<MedicineReturnItem> MedicineReturnItems => Set<MedicineReturnItem>();
    public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionMedicine> PrescriptionMedicines => Set<PrescriptionMedicine>();
    public DbSet<PrescriptionDispense> PrescriptionDispenses => Set<PrescriptionDispense>();
    public DbSet<PrescriptionDispenseItem> PrescriptionDispenseItems => Set<PrescriptionDispenseItem>();
    public DbSet<PharmacySalesReturn> PharmacySalesReturns => Set<PharmacySalesReturn>();
    public DbSet<PharmacySalesReturnItem> PharmacySalesReturnItems => Set<PharmacySalesReturnItem>();
    public DbSet<MedicineStockAdjustment> MedicineStockAdjustments => Set<MedicineStockAdjustment>();
    public DbSet<MedicineStockAdjustmentItem> MedicineStockAdjustmentItems => Set<MedicineStockAdjustmentItem>();
    public DbSet<MedicineStockOpening> MedicineStockOpenings => Set<MedicineStockOpening>();
    public DbSet<AccountHead> AccountHeads => Set<AccountHead>();
    public DbSet<AccountVoucher> AccountVouchers => Set<AccountVoucher>();
    public DbSet<AccountVoucherLine> AccountVoucherLines => Set<AccountVoucherLine>();
    public DbSet<CashierHandover> CashierHandovers => Set<CashierHandover>();
    public DbSet<FinancialPeriodLock> FinancialPeriodLocks => Set<FinancialPeriodLock>();
    public DbSet<FinancialReversal> FinancialReversals => Set<FinancialReversal>();
    public DbSet<LegacyMigrationRun> LegacyMigrationRuns => Set<LegacyMigrationRun>();
    public DbSet<LegacyMigrationQuarantine> LegacyMigrationQuarantines => Set<LegacyMigrationQuarantine>();
    public DbSet<LabTestParameter> LabTestParameters => Set<LabTestParameter>();
    public DbSet<DiagnosticOrder> DiagnosticOrders => Set<DiagnosticOrder>();
    public DbSet<DiagnosticOrderItem> DiagnosticOrderItems => Set<DiagnosticOrderItem>();
    public DbSet<LabSampleCollection> LabSampleCollections => Set<LabSampleCollection>();
    public DbSet<LabTestResult> LabTestResults => Set<LabTestResult>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<HospitalProfile> HospitalProfiles => Set<HospitalProfile>();
    public DbSet<CanteenCategory> CanteenCategories => Set<CanteenCategory>();
    public DbSet<CanteenProduct> CanteenProducts => Set<CanteenProduct>();
    public DbSet<CanteenSale> CanteenSales => Set<CanteenSale>();
    public DbSet<CanteenSaleSub> CanteenSaleItems => Set<CanteenSaleSub>();
    public DbSet<CanteenChallan> CanteenChallans => Set<CanteenChallan>();
    public DbSet<CanteenChallanSub> CanteenChallanItems => Set<CanteenChallanSub>();
    public DbSet<CanteenDebtor> CanteenDebtors => Set<CanteenDebtor>();
    public DbSet<CanteenDebtorLedger> CanteenDebtorLedgers => Set<CanteenDebtorLedger>();
    public DbSet<PathologyGroupHead> PathologyGroupHeads => Set<PathologyGroupHead>();
    public DbSet<LabConsultant> LabConsultants => Set<LabConsultant>();
    public DbSet<StoreProductCategory> StoreProductCategories => Set<StoreProductCategory>();
    public DbSet<SupplierStore> StoreSuppliers => Set<SupplierStore>();
    public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();
    public DbSet<StoreChallan> StoreChallans => Set<StoreChallan>();
    public DbSet<StoreChallanSub> StoreChallanItems => Set<StoreChallanSub>();
    public DbSet<StoreIssue> StoreIssues => Set<StoreIssue>();
    public DbSet<StoreIssueSub> StoreIssueItems => Set<StoreIssueSub>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureAppUser(modelBuilder);
        ConfigureRegistration(modelBuilder);
        ConfigureAppointment(modelBuilder);
        ConfigureTicket(modelBuilder);
        ConfigureClinicalEncounters(modelBuilder);
        ConfigureEmergencyMemo(modelBuilder);
        ConfigureDiagnosticReturns(modelBuilder);
        ConfigureOtMemo(modelBuilder);
        ConfigureMasters(modelBuilder);
        ConfigureIpd(modelBuilder);
        ConfigurePharmacy(modelBuilder);
        ConfigureAccounting(modelBuilder);
        ConfigureDiagnostic(modelBuilder);
        ConfigureAudit(modelBuilder);
        ConfigureLegacyMigration(modelBuilder);
        ConfigureHospitalProfile(modelBuilder);
        ConfigureCanteen(modelBuilder);
    }

    private static void ConfigureAppUser(ModelBuilder b)
    {
        var user = b.Entity<AppUser>();
        user.Property(x => x.DoctorProfileID).HasColumnName("Doctor_Profile_ID");
        user.HasIndex(x => x.DoctorProfileID).IsUnique().HasFilter("[Doctor_Profile_ID] IS NOT NULL");
        user.HasOne<DoctorsProfile>().WithOne().HasForeignKey<AppUser>(x => x.DoctorProfileID).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureHospitalProfile(ModelBuilder b)
    {
        var profile = b.Entity<HospitalProfile>();
        profile.ToTable("Hospital_Profile");
        profile.HasKey(x => x.ID);
        profile.Property(x => x.ID).ValueGeneratedNever();
        profile.Property(x => x.HospitalName).HasColumnName("Hospital_Name").HasMaxLength(150);
        profile.Property(x => x.Tagline).HasColumnName("Tagline").HasMaxLength(200);
        profile.Property(x => x.Address).HasColumnName("Address").HasMaxLength(255);
        profile.Property(x => x.CityDistrict).HasColumnName("City_District").HasMaxLength(100);
        profile.Property(x => x.Phone).HasColumnName("Phone").HasMaxLength(100);
        profile.Property(x => x.Hotline).HasColumnName("Hotline").HasMaxLength(100);
        profile.Property(x => x.EmergencyContact).HasColumnName("Emergency_Contact").HasMaxLength(100);
        profile.Property(x => x.Email).HasColumnName("Email").HasMaxLength(100);
        profile.Property(x => x.Website).HasColumnName("Website").HasMaxLength(150);
        profile.Property(x => x.LicenseNo).HasColumnName("License_No").HasMaxLength(100);
        profile.Property(x => x.LogoData).HasColumnName("Logo_Data");
        profile.Property(x => x.LabHeaderDept).HasColumnName("Lab_Header_Dept").HasMaxLength(200);
        profile.Property(x => x.ReceiptHeaderNote).HasColumnName("Receipt_Header_Note").HasMaxLength(500);
        profile.Property(x => x.ReceiptFooterNote).HasColumnName("Receipt_Footer_Note").HasMaxLength(500);
        profile.Property(x => x.IsSetupComplete).HasColumnName("Is_Setup_Complete");
        profile.Property(x => x.SetupCompletedAt).HasColumnName("Setup_Completed_At").HasColumnType("datetime2");
        profile.Property(x => x.SetupBy).HasColumnName("Setup_By").HasMaxLength(100);
        profile.Property(x => x.UpdatedAt).HasColumnName("Updated_At").HasColumnType("datetime2");
        profile.Property(x => x.UpdatedBy).HasColumnName("Updated_By").HasMaxLength(100);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ValidateFinancialPostingControlsAsync(cancellationToken);

        var pending = ChangeTracker.Entries()
            .Where(x => x.Entity is not AuditLog
                && x.Entity.GetType().Namespace == typeof(Registration).Namespace
                && x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(x => new PendingAudit(
                x,
                x.State switch { EntityState.Added => "Create", EntityState.Modified => "Update", _ => "Delete" },
                ChangedFields(x),
                x.State == EntityState.Deleted ? EntityKey(x) : null))
            .ToList();

        if (pending.Count == 0)
            return await base.SaveChangesAsync(cancellationToken);

        if (Database.CurrentTransaction is not null)
            return await SaveAuditedChangesAsync(pending, cancellationToken);

        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await SaveAuditedChangesAsync(pending, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    private async Task<int> SaveAuditedChangesAsync(List<PendingAudit> pending, CancellationToken cancellationToken)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        var (userId, userName) = await CurrentUserAsync();
        foreach (var item in pending)
        {
            AuditLogs.Add(new AuditLog
            {
                UserID = userId,
                UserName = userName,
                Action = item.Action,
                EntityName = item.Entry.Metadata.ClrType.Name,
                EntityID = item.KeyBeforeSave ?? EntityKey(item.Entry),
                ChangedFields = item.Fields,
                EventTime = DateTime.UtcNow
            });
        }
        await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task ValidateFinancialPostingControlsAsync(CancellationToken cancellationToken)
    {
        var postings = ChangeTracker.Entries<AccountVoucher>()
            .Where(x => x.State == EntityState.Added && x.Entity.IsPosted
                || x.State == EntityState.Modified
                    && x.Property(nameof(AccountVoucher.IsPosted)).IsModified
                    && x.Entity.IsPosted)
            .Select(x => new { Date = x.Entity.VoucherDate.Date, x.Entity.EnteredBy })
            .Distinct()
            .ToList();

        foreach (var posting in postings)
        {
            if (await FinancialPeriodLocks.AsNoTracking().AnyAsync(
                x => x.IsActive && x.FromDate <= posting.Date && x.ToDate >= posting.Date,
                cancellationToken))
                throw new InvalidOperationException($"Financial period for {posting.Date:dd MMM yyyy} is locked.");

            if (await CashierHandovers.AsNoTracking().AnyAsync(
                x => x.CashierID == posting.EnteredBy && x.BusinessDate == posting.Date && x.IsLocked,
                cancellationToken))
                throw new InvalidOperationException($"Cashier day for {posting.Date:dd MMM yyyy} is already closed.");
        }
    }

    private Task<(int? UserId, string UserName)> CurrentUserAsync()
    {
        try
        {
            if (_httpContextAccessor?.HttpContext?.User is { } user && user.Identity?.IsAuthenticated == true)
            {
                int? id = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var value) ? value : null;
                return Task.FromResult((id, user.Identity?.Name ?? "system"));
            }

            return Task.FromResult<(int?, string)>((null, "system"));
        }
        catch
        {
            return Task.FromResult<(int?, string)>((null, "system"));
        }
    }

    private static string ChangedFields(EntityEntry entry)
    {
        var names = entry.State == EntityState.Modified
            ? entry.Properties.Where(x => x.IsModified).Select(x => x.Metadata.Name)
            : entry.Properties.Where(x => !x.Metadata.IsShadowProperty()).Select(x => x.Metadata.Name);
        return string.Join(", ", names.OrderBy(x => x));
    }

    private static string? EntityKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key is null) return null;
        return string.Join("/", key.Properties.Select(x => entry.Property(x.Name).CurrentValue?.ToString() ?? ""));
    }

    private sealed record PendingAudit(EntityEntry Entry, string Action, string Fields, string? KeyBeforeSave);

    private static void ConfigureAudit(ModelBuilder b)
    {
        var e = b.Entity<AuditLog>();
        e.ToTable("Audit_Log");
        e.HasKey(x => x.ID);
        e.Property(x => x.UserID).HasColumnName("User_ID");
        e.Property(x => x.UserName).HasColumnName("User_Name").HasMaxLength(100);
        e.Property(x => x.Action).HasMaxLength(20);
        e.Property(x => x.EntityName).HasColumnName("Entity_Name").HasMaxLength(100);
        e.Property(x => x.EntityID).HasColumnName("Entity_ID").HasMaxLength(100);
        e.Property(x => x.ChangedFields).HasColumnName("Changed_Fields").HasMaxLength(2000);
        e.Property(x => x.EventTime).HasColumnName("Event_Time").HasColumnType("datetime2");
        e.HasIndex(x => x.EventTime);
        e.HasIndex(x => new { x.EntityName, x.EntityID });
    }

    private static void ConfigureLegacyMigration(ModelBuilder b)
    {
        var run = b.Entity<LegacyMigrationRun>();
        run.ToTable("Legacy_Migration_Run");
        run.HasKey(x => x.ID);
        run.Property(x => x.SourceDatabase).HasColumnName("Source_Database").HasMaxLength(200);
        run.Property(x => x.TargetDatabase).HasColumnName("Target_Database").HasMaxLength(200);
        run.Property(x => x.ApprovedBy).HasColumnName("Approved_By").HasMaxLength(100);
        run.Property(x => x.ApprovalHash).HasColumnName("Approval_Hash").HasMaxLength(64);
        run.Property(x => x.Status).HasMaxLength(20);
        run.Property(x => x.StartedAt).HasColumnName("Started_At").HasColumnType("datetime2");
        run.Property(x => x.CompletedAt).HasColumnName("Completed_At").HasColumnType("datetime2");
        run.Property(x => x.IsRollbackVerification).HasColumnName("Is_Rollback_Verification");
        run.Property(x => x.SummaryJson).HasColumnName("Summary_Json").HasColumnType("nvarchar(max)");
        run.HasIndex(x => x.StartedAt);

        var quarantine = b.Entity<LegacyMigrationQuarantine>();
        quarantine.ToTable("Legacy_Migration_Quarantine");
        quarantine.HasKey(x => x.ID);
        quarantine.Property(x => x.RunID).HasColumnName("Run_ID");
        quarantine.Property(x => x.EntityName).HasColumnName("Entity_Name").HasMaxLength(100);
        quarantine.Property(x => x.SourceID).HasColumnName("Source_ID").HasMaxLength(100);
        quarantine.Property(x => x.Reason).HasMaxLength(500);
        quarantine.Property(x => x.PayloadJson).HasColumnName("Payload_Json").HasColumnType("nvarchar(max)");
        quarantine.Property(x => x.CreatedAt).HasColumnName("Created_At").HasColumnType("datetime2");
        quarantine.HasOne<LegacyMigrationRun>().WithMany().HasForeignKey(x => x.RunID).OnDelete(DeleteBehavior.Cascade);
        quarantine.HasIndex(x => new { x.RunID, x.EntityName });
    }

    private static void ConfigureRegistration(ModelBuilder b)
    {
        var e = b.Entity<Registration>();
        e.ToTable("Registration");
        e.HasKey(x => x.ID);
        e.Property(x => x.ID).UseIdentityColumn();
        e.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        e.HasIndex(x => x.PatientID).IsUnique();
        e.Property(x => x.Name).HasMaxLength(100);
        e.Property(x => x.DateOfBirth).HasColumnName("Date_Of_Birth").HasColumnType("datetime").IsRequired(false);
        e.Property(x => x.Sex).HasMaxLength(10);
        e.Property(x => x.BloodGroup).HasColumnName("Blood_Group").HasMaxLength(10);
        e.Property(x => x.Weight).HasColumnName("Weight").HasMaxLength(20);
        e.Property(x => x.PhotoData).HasColumnName("Photo_Data");
        e.Property(x => x.FatherName).HasColumnName("Father_Name").HasMaxLength(100);
        e.Property(x => x.MotherName).HasColumnName("Mother_Name").HasMaxLength(100);
        e.Property(x => x.CareOf).HasColumnName("Care_Of").HasMaxLength(50);
        e.Property(x => x.PermanentVillage).HasColumnName("Permanent_Village").HasMaxLength(200);
        e.Property(x => x.PermanentPO).HasColumnName("Permanent_PO").HasMaxLength(100);
        e.Property(x => x.PermanentPS).HasColumnName("Permanent_PS").HasMaxLength(100);
        e.Property(x => x.PermanentDistrict).HasColumnName("Permanent_District").HasMaxLength(50);
        e.Property(x => x.PresentVillage).HasColumnName("Present_Village").HasMaxLength(200);
        e.Property(x => x.PresentPO).HasColumnName("Present_PO").HasMaxLength(100);
        e.Property(x => x.PresentPS).HasColumnName("Present_PS").HasMaxLength(100);
        e.Property(x => x.PresentDistrict).HasColumnName("Present_District").HasMaxLength(50);
        e.Property(x => x.Contact).HasMaxLength(50);
        e.Property(x => x.Email).HasMaxLength(50);
        e.Property(x => x.Occupation).HasMaxLength(50);
        e.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        e.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
    }

    private static void ConfigureClinicalEncounters(ModelBuilder b)
    {
        var e = b.Entity<PatientEncounter>();
        e.ToTable("Patient_Encounter", table => table.HasCheckConstraint(
            "CK_Patient_Encounter_Status",
            "[Status] IN ('Waiting','InConsultation','Completed','Cancelled','NoShow')"));
        e.HasKey(x => x.ID);
        e.Property(x => x.ID).UseIdentityColumn();
        e.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        e.Property(x => x.PatientName).HasColumnName("Patient_Name").HasMaxLength(100);
        e.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        e.Property(x => x.DepartmentID).HasColumnName("Department_ID");
        e.Property(x => x.AppointmentID).HasColumnName("Appointment_ID");
        e.Property(x => x.TicketID).HasColumnName("Ticket_ID");
        e.Property(x => x.VisitType).HasColumnName("Visit_Type").HasMaxLength(20);
        e.Property(x => x.Status).HasMaxLength(30);
        e.Property(x => x.QueueNo).HasColumnName("Queue_No");
        e.Property(x => x.VisitDate).HasColumnName("Visit_Date").HasColumnType("date");
        e.Property(x => x.CheckedInAt).HasColumnName("Checked_In_At").HasColumnType("datetime2");
        e.Property(x => x.StartedAt).HasColumnName("Started_At").HasColumnType("datetime2");
        e.Property(x => x.CompletedAt).HasColumnName("Completed_At").HasColumnType("datetime2");
        e.Property(x => x.BloodPressure).HasColumnName("Blood_Pressure").HasMaxLength(20);
        e.Property(x => x.OxygenSaturation).HasColumnName("Oxygen_Saturation").HasMaxLength(20);
        e.Property(x => x.HeightCm).HasColumnName("Height_Cm").HasColumnType("decimal(6,2)");
        e.Property(x => x.WeightKg).HasColumnName("Weight_Kg").HasColumnType("decimal(6,2)");
        e.Property(x => x.ChiefComplaint).HasColumnName("Chief_Complaint").HasMaxLength(1000);
        e.Property(x => x.ClinicalNotes).HasColumnName("Clinical_Notes").HasMaxLength(2000);
        e.Property(x => x.CreatedBy).HasColumnName("Created_By");
        e.Property(x => x.StartedBy).HasColumnName("Started_By");
        e.Property(x => x.CompletedBy).HasColumnName("Completed_By");
        e.Property(x => x.CreatedAt).HasColumnName("Created_At").HasColumnType("datetime2");
        e.Property(x => x.UpdatedAt).HasColumnName("Updated_At").HasColumnType("datetime2");
        e.HasIndex(x => x.AppointmentID).IsUnique().HasFilter("[Appointment_ID] IS NOT NULL");
        e.HasIndex(x => x.TicketID).IsUnique().HasFilter("[Ticket_ID] IS NOT NULL");
        e.HasIndex(x => new { x.DoctorID, x.VisitDate, x.Status });
        e.HasIndex(x => new { x.PatientID, x.VisitDate });
        e.HasOne<DoctorAppointment>().WithOne().HasForeignKey<PatientEncounter>(x => x.AppointmentID).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<OPDTicket>().WithOne().HasForeignKey<PatientEncounter>(x => x.TicketID).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<DoctorsProfile>().WithMany().HasForeignKey(x => x.DoctorID).OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureAppointment(ModelBuilder b)
    {
        var e = b.Entity<DoctorAppointment>();
        e.ToTable("Doctor_Appointment");
        e.HasKey(x => x.ID);
        e.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        e.Property(x => x.ApptSLNO).HasColumnName("Appt_SLNO");
        e.Property(x => x.ApptDate).HasColumnName("Appt_Date").HasColumnType("date");
        e.Property(x => x.Name).HasMaxLength(100);
        e.Property(x => x.Age).HasMaxLength(50);
        e.Property(x => x.Sex).HasColumnName("Sex").HasMaxLength(50);
        e.Property(x => x.Weight).HasColumnName("Weight").HasMaxLength(20);
        e.Property(x => x.ContactNo).HasColumnName("Contact_No").HasMaxLength(20);
        e.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        e.Property(x => x.Status).HasMaxLength(30).HasDefaultValue("Booked");
        e.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        e.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        e.HasIndex(x => new { x.DoctorID, x.ApptDate, x.ApptSLNO }).IsUnique();
    }

    private static void ConfigureTicket(ModelBuilder b)
    {
        var e = b.Entity<OPDTicket>();
        e.ToTable("OPD_Ticket");
        e.HasKey(x => x.ID);
        e.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        e.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(50);
        e.Property(x => x.TicketRate).HasColumnName("Ticket_Rate").HasMaxLength(50);
        e.Property(x => x.Name).HasMaxLength(50);
        e.Property(x => x.Age).HasMaxLength(50);
        e.Property(x => x.Sex).HasMaxLength(50);
        e.Property(x => x.Vat).HasColumnType("numeric(18,0)");
        e.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        e.Property(x => x.DeptID).HasColumnName("Dept_ID");
        e.Property(x => x.Room).HasMaxLength(50);
        e.Property(x => x.Comments).HasMaxLength(200);
        e.Property(x => x.Total).HasColumnType("numeric(18,0)");
        e.Property(x => x.EnteredBy).HasColumnName("Entered_By");
    }

    private static void ConfigureEmergencyMemo(ModelBuilder b)
    {
        var e = b.Entity<CashMemoDInvestigation>();
        e.ToTable("Cash_Memo_DInvestigation", table => table.HasCheckConstraint(
            "CK_Cash_Memo_DInvestigation_Amounts",
            "[Total] >= 0 AND [Discount] >= 0 AND [Discount] <= [Total] AND [Paid] >= 0 AND [Paid] <= [Total] - [Discount]"));
        e.HasKey(x => x.ID);
        e.Property(x => x.PatientName).HasColumnName("Patient_Name").HasMaxLength(100);
        e.Property(x => x.PatientType).HasColumnName("Patient_Type").HasMaxLength(1);
        e.Property(x => x.Bed).HasMaxLength(10);
        e.Property(x => x.Age).HasMaxLength(50);
        e.Property(x => x.Sex).HasMaxLength(10);
        e.Property(x => x.Contact).HasMaxLength(200);
        e.Property(x => x.Total).HasColumnType("numeric(18,2)");
        e.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        e.Property(x => x.DiscountBy).HasColumnName("Discount_By").HasMaxLength(50);
        e.Property(x => x.Paid).HasColumnType("numeric(18,2)");
        e.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(50);
        e.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        e.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        e.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        e.Property(x => x.ModuleID).HasColumnName("Module_ID");
        e.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        e.Property(x => x.EncounterID).HasColumnName("Encounter_ID");
        e.Property(x => x.Remarks).HasMaxLength(200);
        e.HasIndex(x => x.EncounterID);
        e.HasOne<PatientEncounter>().WithMany().HasForeignKey(x => x.EncounterID).OnDelete(DeleteBehavior.Restrict);
        e.HasMany(x => x.Items).WithOne(x => x.Memo).HasForeignKey(x => x.MemoID).OnDelete(DeleteBehavior.Cascade);

        var item = b.Entity<CashMemoSubDInvestigation>();
        item.ToTable("Cash_Memo_Sub_DInvestigation", table => table.HasCheckConstraint(
            "CK_Cash_Memo_Sub_DInvestigation_Amounts", "[Rate] >= 0 AND [Quantity] > 0"));
        item.HasKey(x => x.ID);
        item.Property(x => x.MemoID).HasColumnName("Memo_ID");
        item.Property(x => x.TestID).HasColumnName("Test_ID");
        item.Property(x => x.Rate).HasColumnType("numeric(18,0)");
        item.HasIndex(x => new { x.MemoID, x.SLNo }).IsUnique();
    }

    private static void ConfigureDiagnosticReturns(ModelBuilder b)
    {
        var header = b.Entity<DiagnosticCashMemoReturn>();
        header.ToTable("Diagnostic_Cash_Memo_Return", table => table.HasCheckConstraint(
            "CK_Diagnostic_Cash_Memo_Return_Amounts", "[Adjustment_Amount] > 0 AND [Refund_Amount] >= 0 AND [Receivable_Adjustment] >= 0 AND [Refund_Amount] + [Receivable_Adjustment] = [Adjustment_Amount]"));
        header.HasKey(x => x.ID);
        header.Property(x => x.MemoID).HasColumnName("Memo_ID");
        header.Property(x => x.AdjustmentAmount).HasColumnName("Adjustment_Amount").HasColumnType("numeric(18,2)");
        header.Property(x => x.RefundAmount).HasColumnName("Refund_Amount").HasColumnType("numeric(18,2)");
        header.Property(x => x.ReceivableAdjustment).HasColumnName("Receivable_Adjustment").HasColumnType("numeric(18,2)");
        header.Property(x => x.Reason).HasMaxLength(500);
        header.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Posted");
        header.Property(x => x.RefundMethod).HasColumnName("Refund_Method").HasMaxLength(30);
        header.Property(x => x.RefundReference).HasColumnName("Refund_Reference").HasMaxLength(100);
        header.Property(x => x.RefundReceivedBy).HasColumnName("Refund_Received_By").HasMaxLength(100);
        header.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        header.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        header.Property(x => x.CancelledBy).HasColumnName("Cancelled_By");
        header.Property(x => x.CancelledAt).HasColumnName("Cancelled_At").HasColumnType("datetime");
        header.Property(x => x.CancellationReason).HasColumnName("Cancellation_Reason").HasMaxLength(500);
        header.HasOne(x => x.Memo).WithMany().HasForeignKey(x => x.MemoID).OnDelete(DeleteBehavior.Restrict);
        header.HasMany(x => x.Items).WithOne(x => x.Return).HasForeignKey(x => x.ReturnID).OnDelete(DeleteBehavior.Cascade);
        header.HasIndex(x => new { x.MemoID, x.TransDate });

        var item = b.Entity<DiagnosticCashMemoReturnItem>();
        item.ToTable("Diagnostic_Cash_Memo_Return_Sub", table => table.HasCheckConstraint(
            "CK_Diagnostic_Cash_Memo_Return_Sub_Amounts", "[Quantity] > 0 AND [Rate] >= 0"));
        item.HasKey(x => x.ID);
        item.Property(x => x.ReturnID).HasColumnName("Return_ID");
        item.Property(x => x.CashMemoItemID).HasColumnName("Cash_Memo_Sub_ID");
        item.Property(x => x.ServiceID).HasColumnName("Service_ID");
        item.Property(x => x.Rate).HasColumnType("numeric(18,2)");
        item.HasOne(x => x.CashMemoItem).WithMany().HasForeignKey(x => x.CashMemoItemID).OnDelete(DeleteBehavior.Restrict);
        item.HasIndex(x => new { x.ReturnID, x.CashMemoItemID }).IsUnique();
        item.HasIndex(x => x.CashMemoItemID);
    }

    private static void ConfigureOtMemo(ModelBuilder b)
    {
        var e = b.Entity<CashMemoOT>();
        e.ToTable("Cash_Memo_OT", table => table.HasCheckConstraint(
            "CK_Cash_Memo_OT_Amounts",
            "[Total] >= 0 AND [Discount] >= 0 AND [Discount] <= [Total]"));
        e.HasKey(x => x.ID);
        e.Property(x => x.PatientName).HasColumnName("Patient_Name").HasMaxLength(100);
        e.Property(x => x.PatientType).HasColumnName("Patient_Type").HasMaxLength(1);
        e.Property(x => x.Bed).HasMaxLength(10);
        e.Property(x => x.Age).HasMaxLength(50);
        e.Property(x => x.Sex).HasMaxLength(10);
        e.Property(x => x.Contact).HasMaxLength(200);
        e.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        e.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(50);
        e.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        e.Property(x => x.Total).HasColumnType("numeric(18,2)");
        e.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        e.Property(x => x.DiscountBy).HasColumnName("Discount_By").HasMaxLength(50);
        e.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        e.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        e.HasMany(x => x.Items).WithOne(x => x.Memo).HasForeignKey(x => x.MemoID).OnDelete(DeleteBehavior.Cascade);

        var item = b.Entity<CashMemoSubOT>();
        item.ToTable("Cash_Memo_Sub_OT", table => table.HasCheckConstraint(
            "CK_Cash_Memo_Sub_OT_Amounts", "[Rate] >= 0 AND [Quantity] > 0"));
        item.HasKey(x => x.ID);
        item.Property(x => x.MemoID).HasColumnName("Memo_ID");
        item.Property(x => x.ServiceID).HasColumnName("Service_ID");
        item.Property(x => x.Rate).HasColumnType("numeric(18,2)");
        item.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        item.HasIndex(x => new { x.MemoID, x.SLNO }).IsUnique();
    }

    private static void ConfigureMasters(ModelBuilder b)
    {
        var doctor = b.Entity<DoctorsProfile>();
        doctor.ToTable("Doctors_Profile");
        doctor.HasKey(x => x.ID);
        doctor.Property(x => x.Name).HasMaxLength(100);
        doctor.Property(x => x.Title).HasMaxLength(50);
        doctor.Property(x => x.Degree).HasMaxLength(150);
        doctor.Property(x => x.Attachment).HasMaxLength(10).HasDefaultValue("Temporary");
        doctor.Property(x => x.POSerial).HasColumnName("PO_Serial");
        doctor.Property(x => x.AreaCode).HasColumnName("Area_Code");
        doctor.Property(x => x.Balance).HasColumnType("numeric(18,0)");
        doctor.Property(x => x.OpeningBalance).HasColumnName("Opening_Balance").HasColumnType("numeric(18,0)");
        doctor.Property(x => x.Contact).HasMaxLength(50);
        doctor.Property(x => x.Email).HasMaxLength(50);
        doctor.Property(x => x.Room).HasMaxLength(10);
        doctor.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        doctor.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        doctor.Property(x => x.Address).HasMaxLength(200);
        doctor.Property(x => x.ReferralCategoryID).HasColumnName("Referral_Category_ID");
        doctor.Property(x => x.IsActive).HasColumnName("Is_Active").HasDefaultValue(true);
        doctor.Property(x => x.Priority).HasColumnName("Priority");
        doctor.Property(x => x.ConsultationTime).HasColumnName("Consultation_Time").HasMaxLength(100);
        doctor.Property(x => x.OffDay).HasColumnName("Off_Day").HasMaxLength(100);
        doctor.Property(x => x.ConsultationFee).HasColumnName("Consultation_Fee").HasColumnType("numeric(18,2)");
        doctor.Property(x => x.OpdConsultationFee).HasColumnName("OPD_Consultation_Fee").HasColumnType("numeric(18,2)");

        var department = b.Entity<Department>();
        department.ToTable("Department");
        department.HasKey(x => x.ID);
        department.Property(x => x.Name).HasMaxLength(50);
        department.Property(x => x.DepartmentCategoryID).HasColumnName("Department_Category_ID");
        department.Property(x => x.IsActive).HasColumnName("Is_Active").HasDefaultValue(true);
        department.HasIndex(x => x.Name).IsUnique();

        var ticketType = b.Entity<TicketType>();
        ticketType.ToTable("Ticket_Type");
        ticketType.HasKey(x => x.ID);
        ticketType.Property(x => x.Name).HasMaxLength(50);
        ticketType.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        ticketType.Property(x => x.IsActive).HasColumnName("Is_Active").HasDefaultValue(true);
        ticketType.HasIndex(x => x.Name).IsUnique();

        var category = b.Entity<ServiceCategory>();
        category.ToTable("Service_Category");
        category.HasKey(x => x.ID);
        category.Property(x => x.ID).ValueGeneratedNever();
        category.Property(x => x.Name).HasMaxLength(200);
        category.Property(x => x.SubID).HasColumnName("Sub_ID");
        category.Property(x => x.ModuleID).HasColumnName("Module_ID");
        category.Property(x => x.ServiceCharge).HasColumnName("Service_Charge").HasColumnType("decimal(18,2)");
        category.Property(x => x.IsPercentage).HasColumnName("Is_Percentage");
        category.Property(x => x.GroupSubID).HasColumnName("Group_Sub_ID");
        category.Property(x => x.InvestigationSLNO).HasColumnName("Investigation_SLNO");

        var service = b.Entity<Service>();
        service.ToTable("Service");
        service.HasKey(x => x.ID);
        service.Property(x => x.Name).HasMaxLength(200);
        service.Property(x => x.SaleRate).HasColumnName("Sale_Rate").HasColumnType("numeric(18,2)");
        service.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("numeric(18,2)");
        service.Property(x => x.ReOrderLevel).HasColumnName("Re_Order_Level");
        service.Property(x => x.IsVATApp).HasColumnName("Is_VAT_App");
        service.Property(x => x.CategoryID).HasColumnName("Category_ID");
        service.Property(x => x.TypeID).HasColumnName("Type_ID");
        service.Property(x => x.GenericID).HasColumnName("Generic_ID");
        service.Property(x => x.SupplierID).HasColumnName("Supplier_ID");
        service.Property(x => x.InvestigationModuleID).HasColumnName("Investigation_Module_ID");
        service.Property(x => x.DiagnosticType).HasColumnName("Diagnostic_Type").HasMaxLength(20);
        service.Property(x => x.DeliveryHours).HasColumnName("Delivery_Hours");

        department.HasData(
            new Department { ID = 1, Name = "Emergency" },
            new Department { ID = 2, Name = "Medicine" });
        doctor.HasData(new DoctorsProfile
        {
            ID = 1, Name = "Duty Doctor", Title = "Dr.", Degree = "MBBS", Room = "ER-01",
            Department = 1, EnteredBy = 1, TransDate = new DateTime(2026, 1, 1)
        });
        ticketType.HasData(new TicketType { ID = 1, Name = "Consultation Fee", Amount = 50m });
        category.HasData(
            new ServiceCategory { ID = 1, Name = "Team Charge/Doctor Consultation", SubID = 1 },
            new ServiceCategory { ID = 3, Name = "Medicine", SubID = 0 },
            new ServiceCategory { ID = 6, Name = "OPD Emergency", SubID = 0 },
            new ServiceCategory { ID = 54, Name = "OT Charge", SubID = 0 });
        service.HasData(
            new Service { ID = 1, Name = "Emergency consultation", SaleRate = 500m, CategoryID = 6 },
            new Service { ID = 2, Name = "Nebulization", SaleRate = 300m, CategoryID = 6 },
            new Service { ID = 3, Name = "Surgeon charge", SaleRate = 10000m, CategoryID = 1 },
            new Service { ID = 4, Name = "Anaesthesia charge", SaleRate = 2000m, CategoryID = 1 });
    }

    private static void ConfigureIpd(ModelBuilder b)
    {
        var admission = b.Entity<PatientAdmission>();
        admission.ToTable("Admission");
        admission.HasKey(x => x.ID);
        admission.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        admission.HasIndex(x => x.RegNo).IsUnique();
        admission.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        admission.Property(x => x.Diagnosis).HasMaxLength(200);
        admission.Property(x => x.Age).HasMaxLength(20);
        admission.Property(x => x.ReferredBy).HasColumnName("Referred_By");
        admission.Property(x => x.AdmittedBy).HasColumnName("Admitted_By");
        admission.Property(x => x.IsReferred).HasColumnName("Is_Referred");
        admission.Property(x => x.BedCabinID).HasColumnName("Bed_Cabin_ID");
        admission.Property(x => x.AdmissionFee).HasColumnName("Admission_Fee").HasColumnType("numeric(18,2)");
        admission.Property(x => x.AdmissionDate).HasColumnName("Admission_Date").HasColumnType("datetime");
        admission.Property(x => x.ReleaseDate).HasColumnName("Release_Date").HasColumnType("datetime");
        admission.Property(x => x.IsAdmitted).HasColumnName("Is_Admitted");
        admission.Property(x => x.BedAdmissionDate).HasColumnName("Bed_Admission_Date").HasColumnType("datetime");
        admission.Property(x => x.Comments).HasMaxLength(200);
        admission.Property(x => x.BillNo).HasColumnName("Bill_No");
        admission.Property(x => x.BillGeneratedBy).HasColumnName("Bill_Generated_By");
        admission.Property(x => x.PromotionOfficerID).HasColumnName("MPO_ID");
        admission.Property(x => x.IsPackage).HasColumnName("Is_Package");
        admission.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        admission.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        admission.Property(x => x.AmbulanceFee).HasColumnName("Ambulance").HasColumnType("numeric(18,2)");
        admission.Property(x => x.Outcome).HasMaxLength(50);
        admission.Property(x => x.PatientCategory).HasColumnName("Patient_Category").HasMaxLength(50);
        admission.Property(x => x.PatientReceivedBy).HasColumnName("Patient_Received_By");
        admission.Property(x => x.PatientReceivedDate).HasColumnName("Patient_Received_Date").HasColumnType("datetime");
        admission.HasIndex(x => x.PatientID);
        admission.HasIndex(x => new { x.IsAdmitted, x.AdmissionDate });

        var extension = b.Entity<AdmissionExtension>();
        extension.ToTable("Admission_Ext");
        extension.HasKey(x => x.ID);
        extension.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        extension.Property(x => x.ServiceCharge).HasColumnName("Service_Charge").HasColumnType("numeric(18,2)");
        extension.Property(x => x.Food).HasColumnType("numeric(18,2)");
        extension.HasIndex(x => x.RegNo).IsUnique();

        var department = b.Entity<IpdDepartment>();
        department.ToTable("IPD_Department");
        department.HasKey(x => x.ID);
        department.Property(x => x.Name).HasMaxLength(50);
        department.HasData(new IpdDepartment { ID = 1, Name = "General IPD" });

        var category = b.Entity<BedCategory>();
        category.ToTable("Bed_Category");
        category.HasKey(x => x.ID);
        category.Property(x => x.ID).ValueGeneratedNever();
        category.Property(x => x.Name).HasMaxLength(50);
        category.Property(x => x.Rate).HasColumnType("numeric(18,2)");
        category.Property(x => x.Specification).HasMaxLength(200);
        category.Property(x => x.IpdDepartmentID).HasColumnName("IPD_Dept_ID");
        category.Property(x => x.AdmissionFee).HasColumnName("Admission_Fee").HasColumnType("numeric(18,2)");
        category.Property(x => x.ServiceCharge).HasColumnName("Service_Charge").HasColumnType("numeric(18,2)");
        category.Property(x => x.Food).HasColumnType("numeric(18,2)");
        category.HasOne<IpdDepartment>().WithMany().HasForeignKey(x => x.IpdDepartmentID).OnDelete(DeleteBehavior.Restrict);
        category.HasData(new BedCategory { ID = 1, Name = "General Bed", Rate = 1000m, IpdDepartmentID = 1, AdmissionFee = 500m, ServiceCharge = 0m, Food = 0m });

        var bed = b.Entity<BedCabin>();
        bed.ToTable("Bed_Cabin", table => table.HasCheckConstraint(
            "CK_Bed_Cabin_Status", "[Status] IN ('Available','Occupied','Cleaning','Maintenance')"));
        bed.HasKey(x => x.ID);
        bed.Property(x => x.Name).HasMaxLength(50);
        bed.Property(x => x.CategoryID).HasColumnName("Category_ID");
        bed.Property(x => x.IsAllocated).HasColumnName("Is_Allocated");
        bed.Property(x => x.Status).HasMaxLength(20).HasDefaultValue(BedStatuses.Available);
        bed.HasOne<BedCategory>().WithMany().HasForeignKey(x => x.CategoryID).OnDelete(DeleteBehavior.Restrict);
        bed.HasIndex(x => x.Name).IsUnique();
        bed.HasIndex(x => x.Status);
        bed.HasData(new BedCabin { ID = 1, Name = "G-01", CategoryID = 1, IsAllocated = false, Status = BedStatuses.Available });

        var turnover = b.Entity<BedTurnover>();
        turnover.ToTable("Bed_Turnover", table => table.HasCheckConstraint(
            "CK_Bed_Turnover_Status", "[Status] IN ('Cleaning','AwaitingApproval','Completed','Cancelled')"));
        turnover.HasKey(x => x.ID);
        turnover.Property(x => x.BedCabinID).HasColumnName("Bed_Cabin_ID");
        turnover.Property(x => x.AdmissionID).HasColumnName("Admission_ID");
        turnover.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        turnover.Property(x => x.Status).HasMaxLength(30);
        turnover.Property(x => x.RequestedAt).HasColumnName("Requested_At").HasColumnType("datetime2");
        turnover.Property(x => x.CleaningCompletedBy).HasColumnName("Cleaning_Completed_By");
        turnover.Property(x => x.CleaningCompletedAt).HasColumnName("Cleaning_Completed_At").HasColumnType("datetime2");
        turnover.Property(x => x.CleaningNotes).HasColumnName("Cleaning_Notes").HasMaxLength(500);
        turnover.Property(x => x.ReleasedBy).HasColumnName("Released_By");
        turnover.Property(x => x.ReleasedAt).HasColumnName("Released_At").HasColumnType("datetime2");
        turnover.HasOne<BedCabin>().WithMany().HasForeignKey(x => x.BedCabinID).OnDelete(DeleteBehavior.Restrict);
        turnover.HasOne<PatientAdmission>().WithMany().HasForeignKey(x => x.AdmissionID).OnDelete(DeleteBehavior.Restrict);
        turnover.HasIndex(x => new { x.Status, x.RequestedAt });
        turnover.HasIndex(x => x.BedCabinID, "UX_Bed_Turnover_Open_Bed").IsUnique()
            .HasFilter("[Status] IN ('Cleaning','AwaitingApproval')");

        var transaction = b.Entity<BedCabinTransaction>();
        transaction.ToTable("Bed_Cabin_Transection");
        transaction.HasKey(x => x.ID);
        transaction.Property(x => x.BedCabinID).HasColumnName("Bed_Cabin_ID");
        transaction.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        transaction.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        transaction.Property(x => x.Rate).HasColumnType("numeric(18,2)");
        transaction.Property(x => x.PatientTransactionID).HasColumnName("Patient_Transaction_ID");
        transaction.Property(x => x.StartDate).HasColumnName("Start_Date").HasColumnType("datetime");
        transaction.Property(x => x.EndDate).HasColumnName("End_Date").HasColumnType("datetime");
        transaction.HasOne<BedCabin>().WithMany().HasForeignKey(x => x.BedCabinID).OnDelete(DeleteBehavior.Restrict);
        transaction.HasIndex(x => new { x.BedCabinID, x.EndDate });
        transaction.HasIndex(x => x.RegNo);
        transaction.HasIndex(x => x.BedCabinID, "UX_Bed_Cabin_Transection_Open_Bed")
            .IsUnique().HasFilter("[End_Date] IS NULL");
        transaction.HasIndex(x => x.RegNo, "UX_Bed_Cabin_Transection_Open_RegNo")
            .IsUnique().HasFilter("[End_Date] IS NULL");

        var receipt = b.Entity<IpdMoneyReceipt>();
        receipt.ToTable("Money_Receipt");
        receipt.HasKey(x => x.ID);
        receipt.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        receipt.Property(x => x.Against).HasMaxLength(50);
        receipt.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        receipt.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        receipt.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        receipt.HasIndex(x => x.RegNo);
        receipt.HasIndex(x => x.TransDate);

        var dischargeOrder = b.Entity<IpdMedicalDischargeOrder>();
        dischargeOrder.ToTable("IPD_Medical_Discharge_Order", table => table.HasCheckConstraint(
            "CK_IPD_Medical_Discharge_Order_Status", "[Status] IN ('Ready','Completed','Cancelled')"));
        dischargeOrder.HasKey(x => x.ID);
        dischargeOrder.Property(x => x.AdmissionID).HasColumnName("Admission_ID");
        dischargeOrder.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        dischargeOrder.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        dischargeOrder.Property(x => x.Status).HasMaxLength(20);
        dischargeOrder.Property(x => x.Outcome).HasMaxLength(50);
        dischargeOrder.Property(x => x.FinalDiagnosis).HasColumnName("Final_Diagnosis").HasMaxLength(1000);
        dischargeOrder.Property(x => x.ConditionAtDischarge).HasColumnName("Condition_At_Discharge").HasMaxLength(500);
        dischargeOrder.Property(x => x.Advice).HasMaxLength(1000);
        dischargeOrder.Property(x => x.FollowUp).HasColumnName("Follow_Up").HasMaxLength(500);
        dischargeOrder.Property(x => x.OrderedBy).HasColumnName("Ordered_By");
        dischargeOrder.Property(x => x.OrderedAt).HasColumnName("Ordered_At").HasColumnType("datetime2");
        dischargeOrder.Property(x => x.CompletedBy).HasColumnName("Completed_By");
        dischargeOrder.Property(x => x.CompletedAt).HasColumnName("Completed_At").HasColumnType("datetime2");
        dischargeOrder.HasIndex(x => x.AdmissionID).IsUnique();
        dischargeOrder.HasIndex(x => new { x.Status, x.OrderedAt });
        dischargeOrder.HasOne<PatientAdmission>().WithOne().HasForeignKey<IpdMedicalDischargeOrder>(x => x.AdmissionID).OnDelete(DeleteBehavior.Restrict);
        dischargeOrder.HasOne<DoctorsProfile>().WithMany().HasForeignKey(x => x.DoctorID).OnDelete(DeleteBehavior.Restrict);

        var finalBill = b.Entity<IpdFinalBill>();
        finalBill.ToTable("IPD_Final_Bill", table => table.HasCheckConstraint(
            "CK_IPD_Final_Bill_Amounts",
            "[Gross_Amount] >= 0 AND [Discount_Amount] >= 0 AND [Discount_Amount] <= [Gross_Amount] AND [Net_Amount] = [Gross_Amount] - [Discount_Amount] AND [Receipt_Amount] >= 0 AND [Applied_Advance_Amount] >= 0 AND [Balance_Due] >= 0 AND [Excess_Payment] >= 0"));
        finalBill.HasKey(x => x.BillNo);
        finalBill.Property(x => x.BillNo).HasColumnName("Bill_No").ValueGeneratedOnAdd();
        finalBill.Property(x => x.AdmissionID).HasColumnName("Admission_ID");
        finalBill.Property(x => x.MedicalDischargeOrderID).HasColumnName("Medical_Discharge_Order_ID");
        finalBill.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        finalBill.Property(x => x.AdmissionFee).HasColumnName("Admission_Fee").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.BedCharges).HasColumnName("Bed_Charges").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.ServiceCharges).HasColumnName("Service_Charges").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.HospitalCharges).HasColumnName("Hospital_Charges").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.AmbulanceCharges).HasColumnName("Ambulance_Charges").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.GrossAmount).HasColumnName("Gross_Amount").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.DiscountAmount).HasColumnName("Discount_Amount").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.DiscountAuthorizedBy).HasColumnName("Discount_Authorized_By").HasMaxLength(100);
        finalBill.Property(x => x.NetAmount).HasColumnName("Net_Amount").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.ReceiptAmount).HasColumnName("Receipt_Amount").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.AppliedAdvanceAmount).HasColumnName("Applied_Advance_Amount").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.BalanceDue).HasColumnName("Balance_Due").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.ExcessPayment).HasColumnName("Excess_Payment").HasColumnType("numeric(18,2)");
        finalBill.Property(x => x.SettlementStatus).HasColumnName("Settlement_Status").HasMaxLength(20);
        finalBill.Property(x => x.DischargeDate).HasColumnName("Discharge_Date").HasColumnType("datetime");
        finalBill.Property(x => x.Outcome).HasMaxLength(50);
        finalBill.Property(x => x.DischargeNotes).HasColumnName("Discharge_Notes").HasMaxLength(1000);
        finalBill.Property(x => x.GeneratedBy).HasColumnName("Generated_By");
        finalBill.Property(x => x.GeneratedAt).HasColumnName("Generated_At").HasColumnType("datetime");
        finalBill.HasIndex(x => x.AdmissionID).IsUnique();
        finalBill.HasIndex(x => x.RegNo).IsUnique();
        finalBill.HasIndex(x => x.MedicalDischargeOrderID).IsUnique().HasFilter("[Medical_Discharge_Order_ID] IS NOT NULL");
        finalBill.HasOne<PatientAdmission>().WithOne().HasForeignKey<IpdFinalBill>(x => x.AdmissionID).OnDelete(DeleteBehavior.Restrict);
        finalBill.HasOne<IpdMedicalDischargeOrder>().WithOne().HasForeignKey<IpdFinalBill>(x => x.MedicalDischargeOrderID).OnDelete(DeleteBehavior.Restrict);

        var finalBillReceipt = b.Entity<IpdFinalBillReceipt>();
        finalBillReceipt.ToTable("IPD_Final_Bill_Receipt", table => table.HasCheckConstraint(
            "CK_IPD_Final_Bill_Receipt_Amount", "[Amount] > 0"));
        finalBillReceipt.HasKey(x => x.ID);
        finalBillReceipt.Property(x => x.BillNo).HasColumnName("Bill_No");
        finalBillReceipt.Property(x => x.MoneyReceiptID).HasColumnName("Money_Receipt_ID");
        finalBillReceipt.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        finalBillReceipt.Property(x => x.Against).HasMaxLength(50);
        finalBillReceipt.HasOne(x => x.Bill).WithMany(x => x.Receipts).HasForeignKey(x => x.BillNo).OnDelete(DeleteBehavior.Cascade);
        finalBillReceipt.HasOne<IpdMoneyReceipt>().WithMany().HasForeignKey(x => x.MoneyReceiptID).OnDelete(DeleteBehavior.Restrict);
        finalBillReceipt.HasIndex(x => x.MoneyReceiptID).IsUnique();

        ConfigurePatientTransaction<PatientTransaction>(b, "Patient_Transaction");
        ConfigurePatientTransaction<PatientPackageTransaction>(b, "Patient_Package_Transaction");

        var payment = b.Entity<PatientPayment>();
        payment.ToTable("Patient_Payment");
        payment.HasKey(x => x.ID);
        payment.Property(x => x.MoneyReceiptID).HasColumnName("MR_ID");
        payment.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        payment.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        payment.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        payment.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        payment.Property(x => x.IsHospitalPayment).HasColumnName("IS_Hospital_Payment");
        payment.HasIndex(x => x.MoneyReceiptID);
        payment.HasIndex(x => x.RegNo);

        var cash = b.Entity<CentralCashCollection>();
        cash.ToTable("Central_Cash_Collection");
        cash.HasKey(x => x.ID);
        cash.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        cash.Property(x => x.ModuleFlag).HasColumnName("Module_Flag");
        cash.Property(x => x.HeadID).HasColumnName("Head_ID");
        cash.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        cash.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        cash.Property(x => x.TransDateCL).HasColumnName("Trans_Date_CL").HasColumnType("datetime");
        cash.Property(x => x.ReferenceID).HasColumnName("Reference_ID");
        cash.HasIndex(x => x.ReferenceID);

        var serviceItem = b.Entity<IpdPatientServiceItem>();
        serviceItem.ToTable("IPD_Patient_Service_Item");
        serviceItem.HasKey(x => x.ID);
        serviceItem.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        serviceItem.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        serviceItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        serviceItem.Property(x => x.ServiceName).HasColumnName("Service_Name").HasMaxLength(200);
        serviceItem.Property(x => x.Rate).HasColumnType("numeric(18,2)");
        serviceItem.Property(x => x.Total).HasColumnType("numeric(18,2)");
        serviceItem.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        serviceItem.Property(x => x.ServiceDate).HasColumnName("Service_Date").HasColumnType("datetime");
        serviceItem.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        serviceItem.Property(x => x.Remarks).HasMaxLength(200);
        serviceItem.HasIndex(x => x.RegNo);
        serviceItem.HasIndex(x => x.ServiceDate);
    }

    private static void ConfigurePatientTransaction<TEntity>(ModelBuilder b, string tableName)
        where TEntity : class
    {
        var transaction = b.Entity<TEntity>();
        transaction.ToTable(tableName);
        transaction.HasKey("ID");
        transaction.Property<string>("RegNo").HasColumnName("Reg_No").HasMaxLength(10);
        transaction.Property<int>("ServiceID").HasColumnName("Service_ID");
        transaction.Property<decimal>("ServiceRate").HasColumnName("Service_Rate").HasColumnType("numeric(18,2)");
        transaction.Property<decimal>("Quantity").HasColumnType("numeric(18,2)");
        transaction.Property<int?>("IpdDepartmentID").HasColumnName("IPD_Dept_ID");
        transaction.Property<int>("EnteredBy").HasColumnName("Entered_By");
        transaction.Property<DateTime>("TransDate").HasColumnName("Trans_Date").HasColumnType("datetime");
        transaction.HasIndex("RegNo");
    }

    private static void ConfigurePharmacy(ModelBuilder b)
    {
        var generic = b.Entity<GenericGroup>();
        generic.ToTable("Generic_Group");
        generic.HasKey(x => x.ID);
        generic.Property(x => x.Name).HasMaxLength(200);
        generic.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        generic.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        generic.HasIndex(x => x.Name);
        generic.HasData(new GenericGroup { ID = 1, Name = "Others" });

        var type = b.Entity<MedicineType>();
        type.ToTable("Medicine_Type");
        type.HasKey(x => x.ID);
        type.Property(x => x.Type).HasMaxLength(100);
        type.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        type.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        type.HasIndex(x => x.Type);
        type.HasData(new MedicineType { ID = 1, Type = "CAPSULE" });

        var supplier = b.Entity<SupplierPharmacy>();
        supplier.ToTable("Supplier_Pharmacy");
        supplier.HasKey(x => x.ID);
        supplier.Property(x => x.Name).HasMaxLength(50);
        supplier.Property(x => x.Address).HasMaxLength(100);
        supplier.Property(x => x.Contact).HasMaxLength(50);
        supplier.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        supplier.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        supplier.HasIndex(x => x.Name);

        var store = b.Entity<MedicineStore>();
        store.ToTable("Medicine_Store");
        store.HasKey(x => x.ID);
        store.Property(x => x.Name).HasMaxLength(50);
        store.Property(x => x.IsActive).HasColumnName("Is_Active");
        store.Property(x => x.IsBufferStore).HasColumnName("Is_Buffer_Store");
        store.HasIndex(x => x.Name).IsUnique();
        store.HasData(new MedicineStore { ID = 1, Name = "Main Pharmacy", IsActive = true, IsBufferStore = false });

        var balance = b.Entity<MedicineStoreBalance>();
        balance.ToTable("Medicine_Store_Balance");
        balance.HasKey(x => x.ID);
        balance.Property(x => x.ServiceID).HasColumnName("Service_ID");
        balance.Property(x => x.MedicineStoreID).HasColumnName("Medicine_Store_ID");
        balance.Property(x => x.Balance).HasColumnType("numeric(18,2)");
        balance.Property(x => x.ExpireDate).HasColumnName("Expire_Date").HasColumnType("date");
        balance.Property(x => x.MedicineReceivingChallanSubID).HasColumnName("Medicine_Receiving_Challan_Sub_ID");
        balance.Property(x => x.BatchNo).HasColumnName("Batch_No").HasMaxLength(50);
        balance.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);
        balance.HasOne<MedicineStore>().WithMany().HasForeignKey(x => x.MedicineStoreID).OnDelete(DeleteBehavior.Restrict);
        balance.HasIndex(x => new { x.MedicineStoreID, x.ServiceID, x.BatchNo, x.ExpireDate });

        var challan = b.Entity<MedicineReceivingChallan>();
        challan.ToTable("Medicine_Receiving_Challan");
        challan.HasKey(x => x.ID);
        challan.Property(x => x.InvoiceNo).HasColumnName("Invoice_No").HasMaxLength(20);
        challan.Property(x => x.InvoiceDate).HasColumnName("Invoice_Date").HasColumnType("datetime");
        challan.Property(x => x.IsPosted).HasColumnName("Is_Posted");
        challan.Property(x => x.Total).HasColumnType("numeric(18,2)");
        challan.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        challan.Property(x => x.VAT).HasColumnType("numeric(18,2)");
        challan.Property(x => x.StoreID).HasColumnName("Store_ID");
        challan.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        challan.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        challan.HasIndex(x => new { x.Supplier, x.InvoiceNo }).IsUnique();

        var challanItem = b.Entity<MedicineReceivingChallanItem>();
        challanItem.ToTable("Medicine_Receiving_Challan_Sub");
        challanItem.HasKey(x => x.ID);
        challanItem.Property(x => x.ChallanNo).HasColumnName("Challan_No");
        challanItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        challanItem.Property(x => x.Total).HasColumnType("numeric(18,2)");
        challanItem.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("numeric(18,2)");
        challanItem.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        challanItem.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        challanItem.Property(x => x.VAT).HasColumnType("numeric(18,2)");
        challanItem.Property(x => x.ExpireDate).HasColumnName("Expire_Date").HasColumnType("date");
        challanItem.Property(x => x.BatchNo).HasColumnName("Batch_No").HasMaxLength(50);
        challanItem.Property(x => x.SaleRate).HasColumnName("Sale_Rate").HasColumnType("numeric(18,2)");
        challanItem.HasOne(x => x.Challan).WithMany(x => x.Items).HasForeignKey(x => x.ChallanNo).OnDelete(DeleteBehavior.Cascade);
        challanItem.HasIndex(x => new { x.ChallanNo, x.SLNO }).IsUnique();

        var memo = b.Entity<PharmacyCashMemo>();
        memo.ToTable("Cash_Memo");
        memo.HasKey(x => x.ID);
        memo.Property(x => x.Name).HasMaxLength(50);
        memo.Property(x => x.Contact).HasMaxLength(20);
        memo.Property(x => x.RegNo).HasColumnName("Reg_No").HasMaxLength(10);
        memo.Property(x => x.PatientType).HasColumnName("Patient_Type").HasMaxLength(1);
        memo.Property(x => x.Total).HasColumnType("numeric(18,2)");
        memo.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        memo.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        memo.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        memo.Property(x => x.IsPackage).HasColumnName("Is_Package");
        memo.Property(x => x.IsCredit).HasColumnName("Is_Credit");
        memo.Property(x => x.BufferStore).HasColumnName("Buffer_Store");
        memo.HasIndex(x => x.TransDate);

        var memoItem = b.Entity<PharmacyCashMemoItem>();
        memoItem.ToTable("Cash_Memo_Sub");
        memoItem.HasKey(x => x.ID);
        memoItem.Property(x => x.BillNo).HasColumnName("Bill_No");
        memoItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        memoItem.Property(x => x.Rate).HasColumnType("numeric(18,2)");
        memoItem.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        memoItem.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("numeric(18,2)");
        memoItem.HasOne(x => x.Memo).WithMany(x => x.Items).HasForeignKey(x => x.BillNo).OnDelete(DeleteBehavior.Cascade);
        memoItem.HasIndex(x => new { x.BillNo, x.SLNO }).IsUnique();

        var saleBatch = b.Entity<PharmacySaleBatchAllocation>();
        saleBatch.ToTable("Pharmacy_Sale_Batch_Allocation", table => table.HasCheckConstraint(
            "CK_Pharmacy_Sale_Batch_Allocation_Amounts", "[Quantity] > 0 AND [Buy_Rate] >= 0"));
        saleBatch.HasKey(x => x.ID);
        saleBatch.Property(x => x.CashMemoItemID).HasColumnName("Cash_Memo_Sub_ID");
        saleBatch.Property(x => x.StockBalanceID).HasColumnName("Stock_Balance_ID");
        saleBatch.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        saleBatch.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("numeric(18,2)");
        saleBatch.Property(x => x.BatchNo).HasColumnName("Batch_No").HasMaxLength(50);
        saleBatch.Property(x => x.ExpireDate).HasColumnName("Expire_Date").HasColumnType("date");
        saleBatch.HasOne(x => x.CashMemoItem).WithMany(x => x.BatchAllocations).HasForeignKey(x => x.CashMemoItemID).OnDelete(DeleteBehavior.Cascade);
        saleBatch.HasOne(x => x.StockBalance).WithMany().HasForeignKey(x => x.StockBalanceID).OnDelete(DeleteBehavior.Restrict);
        saleBatch.HasIndex(x => new { x.CashMemoItemID, x.StockBalanceID }).IsUnique();
        saleBatch.HasIndex(x => new { x.StockBalanceID, x.ExpireDate });

        var ledger = b.Entity<SupplierLedger>();
        ledger.ToTable("Supplier_Ledger");
        ledger.HasKey(x => x.ID);
        ledger.Property(x => x.SupplierID).HasColumnName("Supplier_ID");
        ledger.Property(x => x.ChallanNo).HasColumnName("Challan_No");
        ledger.Property(x => x.SupplierPaymentID).HasColumnName("Supplier_Payment_ID");
        ledger.Property(x => x.Debit).HasColumnType("decimal(18,2)");
        ledger.Property(x => x.Credit).HasColumnType("decimal(18,2)");
        ledger.Property(x => x.Balance).HasColumnType("decimal(18,2)");
        ledger.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        ledger.HasIndex(x => new { x.SupplierID, x.TransDate });

        var medicineReturn = b.Entity<MedicineReturn>();
        medicineReturn.ToTable("Medicine_Return");
        medicineReturn.HasKey(x => x.ID);
        medicineReturn.Property(x => x.SupplierID).HasColumnName("Supplier_ID");
        medicineReturn.Property(x => x.StoreID).HasColumnName("Store_ID");
        medicineReturn.Property(x => x.IsPosted).HasColumnName("Is_Posted");
        medicineReturn.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        medicineReturn.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        medicineReturn.HasIndex(x => new { x.SupplierID, x.TransDate });

        var medicineReturnItem = b.Entity<MedicineReturnItem>();
        medicineReturnItem.ToTable("Medicine_Return_Sub");
        medicineReturnItem.HasKey(x => x.ID);
        medicineReturnItem.Property(x => x.MedicineReturnID).HasColumnName("Medicine_Return_ID");
        medicineReturnItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        medicineReturnItem.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        medicineReturnItem.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("numeric(18,2)");
        medicineReturnItem.HasOne(x => x.MedicineReturn).WithMany(x => x.Items).HasForeignKey(x => x.MedicineReturnID).OnDelete(DeleteBehavior.Cascade);
        medicineReturnItem.HasIndex(x => new { x.MedicineReturnID, x.SLNO }).IsUnique();

        var supplierPayment = b.Entity<SupplierPayment>();
        supplierPayment.ToTable("Supplier_Payment");
        supplierPayment.HasKey(x => x.ID);
        supplierPayment.Property(x => x.SupplierID).HasColumnName("Supplier_ID");
        supplierPayment.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        supplierPayment.Property(x => x.PaymentMode).HasColumnName("Payment_Mode").HasMaxLength(50);
        supplierPayment.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        supplierPayment.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        supplierPayment.HasIndex(x => new { x.SupplierID, x.TransDate });

        var prescription = b.Entity<Prescription>();
        prescription.ToTable("Prescription");
        prescription.HasKey(x => x.ID);
        prescription.Property(x => x.Name).HasMaxLength(100);
        prescription.Property(x => x.Age).HasMaxLength(20);
        prescription.Property(x => x.Sex).HasMaxLength(10);
        prescription.Property(x => x.BloodGroup).HasColumnName("Blood_Group").HasMaxLength(10);
        prescription.Property(x => x.VisitNo).HasColumnName("Visit_No");
        prescription.Property(x => x.VisitDate).HasColumnName("Visit_Date").HasColumnType("date");
        prescription.Property(x => x.Type).HasMaxLength(10);
        prescription.Property(x => x.Height).HasColumnType("decimal(18,2)");
        prescription.Property(x => x.Weight).HasColumnType("decimal(18,2)");
        prescription.Property(x => x.BMI).HasMaxLength(10);
        prescription.Property(x => x.Prem).HasMaxLength(10);
        prescription.Property(x => x.Pulse).HasMaxLength(20);
        prescription.Property(x => x.Restp).HasMaxLength(20);
        prescription.Property(x => x.BP).HasMaxLength(20);
        prescription.Property(x => x.Temperature).HasMaxLength(10);
        prescription.Property(x => x.Waist).HasMaxLength(10);
        prescription.Property(x => x.Hip).HasMaxLength(10);
        prescription.Property(x => x.Complain).HasColumnType("nvarchar(max)");
        prescription.Property(x => x.MedicalHistory).HasColumnName("Medical_History").HasColumnType("nvarchar(max)");
        prescription.Property(x => x.Findings).HasColumnType("nvarchar(max)");
        prescription.Property(x => x.Diagnosis).HasColumnType("nvarchar(max)");
        prescription.Property(x => x.Investigation).HasColumnType("nvarchar(max)");
        prescription.Property(x => x.Remark).HasColumnType("nvarchar(max)");
        prescription.Property(x => x.Medicine).HasColumnType("nvarchar(max)");
        prescription.HasIndex(x => new { x.VisitDate, x.VisitNo })
            .IsUnique().HasFilter("[Visit_Date] IS NOT NULL AND [Visit_No] IS NOT NULL");
        prescription.Property(x => x.ReferredBy).HasColumnName("Referred_By");
        prescription.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        prescription.Property(x => x.EncounterID).HasColumnName("Encounter_ID");
        prescription.Property(x => x.TicketID).HasColumnName("Ticket_ID");
        prescription.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Signed");
        prescription.Property(x => x.CreatedBy).HasColumnName("Created_By");
        prescription.Property(x => x.SignedAt).HasColumnName("Signed_At").HasColumnType("datetime2");
        prescription.HasOne<DoctorsProfile>().WithMany().HasForeignKey(x => x.ReferredBy).OnDelete(DeleteBehavior.Restrict);
        prescription.HasIndex(x => x.VisitDate);
        prescription.HasIndex(x => x.PatientID);
        prescription.HasIndex(x => x.EncounterID).IsUnique().HasFilter("[Encounter_ID] IS NOT NULL");
        prescription.HasOne<PatientEncounter>().WithOne().HasForeignKey<Prescription>(x => x.EncounterID).OnDelete(DeleteBehavior.Restrict);

        var prescriptionMedicine = b.Entity<PrescriptionMedicine>();
        prescriptionMedicine.ToTable("Prescription_Medicine");
        prescriptionMedicine.HasKey(x => x.ID);
        prescriptionMedicine.Property(x => x.PrescriptionID).HasColumnName("Prescription_ID");
        prescriptionMedicine.Property(x => x.ServiceID).HasColumnName("Service_ID");
        prescriptionMedicine.Property(x => x.Interval).HasColumnType("decimal(18,2)");
        prescriptionMedicine.Property(x => x.Dose).HasColumnType("decimal(18,2)");
        prescriptionMedicine.Property(x => x.DosagePattern).HasColumnName("DosagePattern").HasMaxLength(50);
        prescriptionMedicine.Property(x => x.Instruction).HasColumnName("Instruction").HasMaxLength(100);
        prescriptionMedicine.HasOne(x => x.Prescription).WithMany(x => x.Medicines).HasForeignKey(x => x.PrescriptionID).OnDelete(DeleteBehavior.Cascade);
        prescriptionMedicine.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);
        prescriptionMedicine.HasIndex(x => new { x.PrescriptionID, x.SLNO }).IsUnique();

        var dispense = b.Entity<PrescriptionDispense>();
        dispense.ToTable("Pharmacy_Prescription_Dispense");
        dispense.HasKey(x => x.ID);
        dispense.Property(x => x.PrescriptionID).HasColumnName("Prescription_ID");
        dispense.Property(x => x.CashMemoID).HasColumnName("Cash_Memo_ID");
        dispense.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        dispense.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        dispense.HasOne<Prescription>().WithMany().HasForeignKey(x => x.PrescriptionID).OnDelete(DeleteBehavior.Restrict);
        dispense.HasOne<PharmacyCashMemo>().WithMany().HasForeignKey(x => x.CashMemoID).OnDelete(DeleteBehavior.Restrict);
        dispense.HasIndex(x => x.CashMemoID).IsUnique();
        dispense.HasIndex(x => x.PrescriptionID);

        var dispenseItem = b.Entity<PrescriptionDispenseItem>();
        dispenseItem.ToTable("Pharmacy_Prescription_Dispense_Sub");
        dispenseItem.HasKey(x => x.ID);
        dispenseItem.Property(x => x.DispenseID).HasColumnName("Dispense_ID");
        dispenseItem.Property(x => x.PrescriptionMedicineID).HasColumnName("Prescription_Medicine_ID");
        dispenseItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        dispenseItem.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        dispenseItem.HasOne(x => x.Dispense).WithMany(x => x.Items).HasForeignKey(x => x.DispenseID).OnDelete(DeleteBehavior.Cascade);
        dispenseItem.HasIndex(x => x.PrescriptionMedicineID);

        var salesReturn = b.Entity<PharmacySalesReturn>();
        salesReturn.ToTable("Pharmacy_Sales_Return");
        salesReturn.HasKey(x => x.ID);
        salesReturn.Property(x => x.CashMemoID).HasColumnName("Cash_Memo_ID");
        salesReturn.Property(x => x.StoreID).HasColumnName("Store_ID");
        salesReturn.Property(x => x.RefundAmount).HasColumnName("Refund_Amount").HasColumnType("numeric(18,2)");
        salesReturn.Property(x => x.Reason).HasMaxLength(500);
        salesReturn.Property(x => x.IsPosted).HasColumnName("Is_Posted");
        salesReturn.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        salesReturn.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        salesReturn.HasOne<PharmacyCashMemo>().WithMany().HasForeignKey(x => x.CashMemoID).OnDelete(DeleteBehavior.Restrict);
        salesReturn.HasOne<MedicineStore>().WithMany().HasForeignKey(x => x.StoreID).OnDelete(DeleteBehavior.Restrict);
        salesReturn.HasIndex(x => new { x.CashMemoID, x.TransDate });

        var salesReturnItem = b.Entity<PharmacySalesReturnItem>();
        salesReturnItem.ToTable("Pharmacy_Sales_Return_Sub");
        salesReturnItem.HasKey(x => x.ID);
        salesReturnItem.Property(x => x.SalesReturnID).HasColumnName("Sales_Return_ID");
        salesReturnItem.Property(x => x.CashMemoSubID).HasColumnName("Cash_Memo_Sub_ID");
        salesReturnItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        salesReturnItem.Property(x => x.StockBalanceID).HasColumnName("Stock_Balance_ID");
        salesReturnItem.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        salesReturnItem.Property(x => x.Rate).HasColumnType("numeric(18,2)");
        salesReturnItem.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("numeric(18,2)");
        salesReturnItem.Property(x => x.IsRestocked).HasColumnName("Is_Restocked");
        salesReturnItem.HasOne(x => x.SalesReturn).WithMany(x => x.Items).HasForeignKey(x => x.SalesReturnID).OnDelete(DeleteBehavior.Cascade);
        salesReturnItem.HasOne<PharmacyCashMemoItem>().WithMany().HasForeignKey(x => x.CashMemoSubID).OnDelete(DeleteBehavior.Restrict);
        salesReturnItem.HasOne<MedicineStoreBalance>().WithMany().HasForeignKey(x => x.StockBalanceID).OnDelete(DeleteBehavior.Restrict);
        salesReturnItem.HasIndex(x => x.CashMemoSubID);

        var adjustment = b.Entity<MedicineStockAdjustment>();
        adjustment.ToTable("Medicine_Stock_Adjustment");
        adjustment.HasKey(x => x.ID);
        adjustment.Property(x => x.StoreID).HasColumnName("Store_ID");
        adjustment.Property(x => x.AdjustmentType).HasColumnName("Adjustment_Type").HasMaxLength(30);
        adjustment.Property(x => x.Reason).HasMaxLength(500);
        adjustment.Property(x => x.IsPosted).HasColumnName("Is_Posted");
        adjustment.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        adjustment.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        adjustment.HasOne<MedicineStore>().WithMany().HasForeignKey(x => x.StoreID).OnDelete(DeleteBehavior.Restrict);
        adjustment.HasIndex(x => new { x.StoreID, x.TransDate });

        var adjustmentItem = b.Entity<MedicineStockAdjustmentItem>();
        adjustmentItem.ToTable("Medicine_Stock_Adjustment_Sub");
        adjustmentItem.HasKey(x => x.ID);
        adjustmentItem.Property(x => x.AdjustmentID).HasColumnName("Adjustment_ID");
        adjustmentItem.Property(x => x.StockBalanceID).HasColumnName("Stock_Balance_ID");
        adjustmentItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        adjustmentItem.Property(x => x.BatchNo).HasColumnName("Batch_No").HasMaxLength(50);
        adjustmentItem.Property(x => x.ExpireDate).HasColumnName("Expire_Date").HasColumnType("date");
        adjustmentItem.Property(x => x.BeforeQuantity).HasColumnName("Before_Quantity").HasColumnType("numeric(18,2)");
        adjustmentItem.Property(x => x.ChangeQuantity).HasColumnName("Change_Quantity").HasColumnType("numeric(18,2)");
        adjustmentItem.Property(x => x.AfterQuantity).HasColumnName("After_Quantity").HasColumnType("numeric(18,2)");
        adjustmentItem.Property(x => x.UnitCost).HasColumnName("Unit_Cost").HasColumnType("numeric(18,2)");
        adjustmentItem.HasOne(x => x.Adjustment).WithMany(x => x.Items).HasForeignKey(x => x.AdjustmentID).OnDelete(DeleteBehavior.Cascade);
        adjustmentItem.HasOne<MedicineStoreBalance>().WithMany().HasForeignKey(x => x.StockBalanceID).OnDelete(DeleteBehavior.Restrict);
        adjustmentItem.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);
        adjustmentItem.HasIndex(x => x.StockBalanceID);

        var opening = b.Entity<MedicineStockOpening>();
        opening.ToTable("Medicine_Stock_Opening");
        opening.HasKey(x => x.ID);
        opening.Property(x => x.StoreID).HasColumnName("Store_ID");
        opening.Property(x => x.ServiceID).HasColumnName("Service_ID");
        opening.Property(x => x.OpeningBalance).HasColumnName("Opening_Balance").HasColumnType("numeric(18,2)");
        opening.Property(x => x.CapturedAt).HasColumnName("Captured_At").HasColumnType("datetime");
        opening.HasOne<MedicineStore>().WithMany().HasForeignKey(x => x.StoreID).OnDelete(DeleteBehavior.Restrict);
        opening.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);
        opening.HasIndex(x => new { x.StoreID, x.ServiceID }).IsUnique();
    }

    private static void ConfigureAccounting(ModelBuilder b)
    {
        var account = b.Entity<AccountHead>();
        account.ToTable("Accounts_Chart");
        account.HasKey(x => x.ID);
        account.Property(x => x.Code).HasMaxLength(20);
        account.Property(x => x.Name).HasMaxLength(100);
        account.Property(x => x.AccountType).HasColumnName("Account_Type").HasMaxLength(20);
        account.Property(x => x.ParentID).HasColumnName("Parent_ID");
        account.Property(x => x.IsActive).HasColumnName("Is_Active");
        account.HasIndex(x => x.Code).IsUnique();
        account.HasData(
            new AccountHead { ID = 1, Code = "1000", Name = "Cash in Hand", AccountType = "Asset", IsActive = true },
            new AccountHead { ID = 2, Code = "4000", Name = "Hospital Service Revenue", AccountType = "Income", IsActive = true },
            new AccountHead { ID = 3, Code = "4010", Name = "Pharmacy Sales", AccountType = "Income", IsActive = true },
            new AccountHead { ID = 4, Code = "5000", Name = "Medicine Purchase", AccountType = "Expense", IsActive = true },
            new AccountHead { ID = 5, Code = "5100", Name = "General Expense", AccountType = "Expense", IsActive = true },
            new AccountHead { ID = 6, Code = "2000", Name = "Supplier Payable", AccountType = "Liability", IsActive = true },
            new AccountHead { ID = 7, Code = "1100", Name = "Patient Receivable", AccountType = "Asset", IsActive = true },
            new AccountHead { ID = 8, Code = "2100", Name = "Patient Advance", AccountType = "Liability", IsActive = true });

        var voucher = b.Entity<AccountVoucher>();
        voucher.ToTable("Accounts_Voucher");
        voucher.HasKey(x => x.ID);
        voucher.Property(x => x.VoucherNo).HasColumnName("Voucher_No").HasMaxLength(20);
        voucher.Property(x => x.VoucherType).HasColumnName("Voucher_Type").HasMaxLength(20);
        voucher.Property(x => x.VoucherDate).HasColumnName("Voucher_Date").HasColumnType("date");
        voucher.Property(x => x.Narration).HasMaxLength(500);
        voucher.Property(x => x.Reference).HasMaxLength(100);
        voucher.Property(x => x.IsPosted).HasColumnName("Is_Posted");
        voucher.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Pending");
        voucher.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        voucher.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        voucher.Property(x => x.ApprovedBy).HasColumnName("Approved_By");
        voucher.Property(x => x.ApprovedAt).HasColumnName("Approved_At").HasColumnType("datetime");
        voucher.Property(x => x.CancelledBy).HasColumnName("Cancelled_By");
        voucher.Property(x => x.CancelledAt).HasColumnName("Cancelled_At").HasColumnType("datetime");
        voucher.Property(x => x.CancellationReason).HasColumnName("Cancellation_Reason").HasMaxLength(500);
        voucher.Property(x => x.ReversalOfVoucherID).HasColumnName("Reversal_Of_Voucher_ID");
        voucher.HasIndex(x => x.VoucherNo).IsUnique();
        voucher.HasIndex(x => x.VoucherDate);
        voucher.HasIndex(x => x.Status);
        voucher.HasIndex(x => x.Reference);
        voucher.HasIndex(x => x.ReversalOfVoucherID).IsUnique().HasFilter("[Reversal_Of_Voucher_ID] IS NOT NULL");

        var line = b.Entity<AccountVoucherLine>();
        line.ToTable("Accounts_Voucher_Line");
        line.HasKey(x => x.ID);
        line.Property(x => x.VoucherID).HasColumnName("Voucher_ID");
        line.Property(x => x.AccountID).HasColumnName("Account_ID");
        line.Property(x => x.Debit).HasColumnType("numeric(18,2)");
        line.Property(x => x.Credit).HasColumnType("numeric(18,2)");
        line.HasOne(x => x.Voucher).WithMany(x => x.Lines).HasForeignKey(x => x.VoucherID).OnDelete(DeleteBehavior.Cascade);
        line.HasOne<AccountHead>().WithMany().HasForeignKey(x => x.AccountID).OnDelete(DeleteBehavior.Restrict);
        line.HasIndex(x => new { x.AccountID, x.VoucherID });

        var handover = b.Entity<CashierHandover>();
        handover.ToTable("Cashier_Handover");
        handover.HasKey(x => x.ID);
        handover.Property(x => x.BusinessDate).HasColumnName("Business_Date").HasColumnType("date");
        handover.Property(x => x.CashierID).HasColumnName("Cashier_ID");
        handover.Property(x => x.SystemAmount).HasColumnName("System_Amount").HasColumnType("numeric(18,2)");
        handover.Property(x => x.HandedOverAmount).HasColumnName("Handed_Over_Amount").HasColumnType("numeric(18,2)");
        handover.Property(x => x.Variance).HasColumnType("numeric(18,2)");
        handover.Property(x => x.Notes).HasMaxLength(500);
        handover.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        handover.Property(x => x.IsLocked).HasColumnName("Is_Locked");
        handover.Property(x => x.ClosedAt).HasColumnName("Closed_At").HasColumnType("datetime");
        handover.Property(x => x.ReopenedBy).HasColumnName("Reopened_By");
        handover.Property(x => x.ReopenedAt).HasColumnName("Reopened_At").HasColumnType("datetime");
        handover.Property(x => x.ReopenReason).HasColumnName("Reopen_Reason").HasMaxLength(500);
        handover.HasIndex(x => new { x.BusinessDate, x.CashierID }).IsUnique();

        var periodLock = b.Entity<FinancialPeriodLock>();
        periodLock.ToTable("Financial_Period_Lock");
        periodLock.HasKey(x => x.ID);
        periodLock.Property(x => x.FromDate).HasColumnName("From_Date").HasColumnType("date");
        periodLock.Property(x => x.ToDate).HasColumnName("To_Date").HasColumnType("date");
        periodLock.Property(x => x.Reason).HasMaxLength(500);
        periodLock.Property(x => x.IsActive).HasColumnName("Is_Active");
        periodLock.Property(x => x.LockedBy).HasColumnName("Locked_By");
        periodLock.Property(x => x.LockedAt).HasColumnName("Locked_At").HasColumnType("datetime");
        periodLock.Property(x => x.UnlockedBy).HasColumnName("Unlocked_By");
        periodLock.Property(x => x.UnlockedAt).HasColumnName("Unlocked_At").HasColumnType("datetime");
        periodLock.Property(x => x.UnlockReason).HasColumnName("Unlock_Reason").HasMaxLength(500);
        periodLock.HasIndex(x => new { x.IsActive, x.FromDate, x.ToDate });

        var reversal = b.Entity<FinancialReversal>();
        reversal.ToTable("Financial_Reversal");
        reversal.HasKey(x => x.ID);
        reversal.Property(x => x.EntityType).HasColumnName("Entity_Type").HasMaxLength(50);
        reversal.Property(x => x.EntityID).HasColumnName("Entity_ID");
        reversal.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        reversal.Property(x => x.Reason).HasMaxLength(500);
        reversal.Property(x => x.ReversalVoucherID).HasColumnName("Reversal_Voucher_ID");
        reversal.Property(x => x.ReversedBy).HasColumnName("Reversed_By");
        reversal.Property(x => x.ReversedAt).HasColumnName("Reversed_At").HasColumnType("datetime");
        reversal.HasIndex(x => new { x.EntityType, x.EntityID }).IsUnique();
    }

    private static void ConfigureDiagnostic(ModelBuilder b)
    {
        var order = b.Entity<DiagnosticOrder>();
        order.ToTable("Diagnostic_Order", table => table.HasCheckConstraint(
            "CK_Diagnostic_Order_Status", "[Status] IN ('Pending','Billed','Cancelled')"));
        order.HasKey(x => x.ID);
        order.Property(x => x.EncounterID).HasColumnName("Encounter_ID");
        order.Property(x => x.PatientID).HasColumnName("Patient_ID").HasMaxLength(11);
        order.Property(x => x.PatientName).HasColumnName("Patient_Name").HasMaxLength(100);
        order.Property(x => x.DoctorID).HasColumnName("Doctor_ID");
        order.Property(x => x.Status).HasMaxLength(20);
        order.Property(x => x.ClinicalNotes).HasColumnName("Clinical_Notes").HasMaxLength(500);
        order.Property(x => x.BilledMemoID).HasColumnName("Billed_Memo_ID");
        order.Property(x => x.OrderedBy).HasColumnName("Ordered_By");
        order.Property(x => x.OrderedAt).HasColumnName("Ordered_At").HasColumnType("datetime2");
        order.HasIndex(x => new { x.Status, x.OrderedAt });
        order.HasIndex(x => x.BilledMemoID).IsUnique().HasFilter("[Billed_Memo_ID] IS NOT NULL");
        order.HasOne<PatientEncounter>().WithMany().HasForeignKey(x => x.EncounterID).OnDelete(DeleteBehavior.Restrict);
        order.HasOne<DoctorsProfile>().WithMany().HasForeignKey(x => x.DoctorID).OnDelete(DeleteBehavior.Restrict);
        order.HasOne<CashMemoDInvestigation>().WithOne().HasForeignKey<DiagnosticOrder>(x => x.BilledMemoID).OnDelete(DeleteBehavior.Restrict);

        var orderItem = b.Entity<DiagnosticOrderItem>();
        orderItem.ToTable("Diagnostic_Order_Item");
        orderItem.HasKey(x => x.ID);
        orderItem.Property(x => x.OrderID).HasColumnName("Order_ID");
        orderItem.Property(x => x.ServiceID).HasColumnName("Service_ID");
        orderItem.HasIndex(x => new { x.OrderID, x.ServiceID }).IsUnique();
        orderItem.HasOne(x => x.Order).WithMany(x => x.Items).HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Cascade);
        orderItem.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);

        var param = b.Entity<LabTestParameter>();
        param.ToTable("Lab_Test_Parameter");
        param.HasKey(x => x.ID);
        param.Property(x => x.ServiceID).HasColumnName("Service_ID");
        param.Property(x => x.ParameterName).HasColumnName("Parameter_Name").HasMaxLength(100);
        param.Property(x => x.Unit).HasMaxLength(50);
        param.Property(x => x.ReferenceRange).HasColumnName("Reference_Range").HasMaxLength(200);
        param.Property(x => x.SortOrder).HasColumnName("Sort_Order");
        param.HasIndex(x => x.ServiceID);

        var sample = b.Entity<LabSampleCollection>();
        sample.ToTable("Lab_Sample_Collection");
        sample.HasKey(x => x.ID);
        sample.Property(x => x.MemoID).HasColumnName("Memo_ID");
        sample.Property(x => x.ServiceID).HasColumnName("Service_ID");
        sample.Property(x => x.SpecimenType).HasColumnName("Specimen_Type").HasMaxLength(100);
        sample.Property(x => x.CollectedAt).HasColumnName("Collected_At").HasColumnType("datetime");
        sample.Property(x => x.CollectedBy).HasColumnName("Collected_By");
        sample.Property(x => x.ReceivedAt).HasColumnName("Received_At").HasColumnType("datetime");
        sample.Property(x => x.ReceivedBy).HasColumnName("Received_By");
        sample.Property(x => x.ProcessingStartedAt).HasColumnName("Processing_Started_At").HasColumnType("datetime");
        sample.Property(x => x.ProcessingStartedBy).HasColumnName("Processing_Started_By");
        sample.Property(x => x.CompletedAt).HasColumnName("Completed_At").HasColumnType("datetime");
        sample.Property(x => x.CompletedBy).HasColumnName("Completed_By");
        sample.Property(x => x.ScheduledAt).HasColumnName("Scheduled_At").HasColumnType("datetime");
        sample.Property(x => x.Workstation).HasMaxLength(100);
        sample.Property(x => x.Status).HasMaxLength(30);
        sample.Property(x => x.Remarks).HasMaxLength(200);
        sample.HasIndex(x => new { x.MemoID, x.ServiceID }).IsUnique();
        sample.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);

        var result = b.Entity<LabTestResult>();
        result.ToTable("Lab_Test_Result");
        result.HasKey(x => x.ID);
        result.Property(x => x.MemoID).HasColumnName("Memo_ID");
        result.Property(x => x.ServiceID).HasColumnName("Service_ID");
        result.Property(x => x.ParameterName).HasColumnName("Parameter_Name").HasMaxLength(100);
        result.Property(x => x.ResultValue).HasColumnName("Result_Value").HasMaxLength(4000);
        result.Property(x => x.Unit).HasMaxLength(50);
        result.Property(x => x.ReferenceRange).HasColumnName("Reference_Range").HasMaxLength(200);
        result.Property(x => x.Flag).HasMaxLength(30);
        result.Property(x => x.Interpretation).HasMaxLength(200);
        result.Property(x => x.ReportedAt).HasColumnName("Reported_At").HasColumnType("datetime");
        result.Property(x => x.ReportedBy).HasColumnName("Reported_By");
        result.Property(x => x.IsVerified).HasColumnName("Is_Verified");
        result.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Active");
        result.Property(x => x.CancelledBy).HasColumnName("Cancelled_By");
        result.Property(x => x.CancelledAt).HasColumnName("Cancelled_At").HasColumnType("datetime");
        result.Property(x => x.CancellationReason).HasColumnName("Cancellation_Reason").HasMaxLength(500);
        result.HasIndex(x => new { x.MemoID, x.ServiceID });
        result.HasQueryFilter(x => x.Status != "Cancelled");
    }

    private static void ConfigureCanteen(ModelBuilder b)
    {
        var category = b.Entity<CanteenCategory>();
        category.ToTable("Canteen_Category");
        category.HasKey(x => x.ID);
        category.Property(x => x.Name).HasMaxLength(50);

        var product = b.Entity<CanteenProduct>();
        product.ToTable("Canteen_Products");
        product.HasKey(x => x.ID);
        product.Property(x => x.Name).HasMaxLength(100);
        product.Property(x => x.CategoryID).HasColumnName("Category_ID");
        product.Property(x => x.SaleRate).HasColumnName("Sale_Rate").HasColumnType("decimal(18,2)");
        product.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("decimal(18,2)");
        product.Property(x => x.Balance).HasColumnType("decimal(18,2)");
        product.Property(x => x.ReorderLevel).HasColumnName("Reorder_Level");
        product.Property(x => x.Unit).HasMaxLength(50);
        product.Property(x => x.IsKitchenItem).HasColumnName("IsKitchen_Item");
        product.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryID).OnDelete(DeleteBehavior.Restrict);

        var sale = b.Entity<CanteenSale>();
        sale.ToTable("Canteen_Sales");
        sale.HasKey(x => x.ID);
        sale.Property(x => x.CustomerName).HasColumnName("Customer_Name").HasMaxLength(50);
        sale.Property(x => x.Contact).HasMaxLength(20);
        sale.Property(x => x.CustomerType).HasColumnName("Customer_Type").HasMaxLength(10);
        sale.Property(x => x.Total).HasColumnType("numeric(18,2)");
        sale.Property(x => x.ServiceCharge).HasColumnName("Service_Charge").HasColumnType("numeric(18,2)");
        sale.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        sale.Property(x => x.Paid).HasColumnType("numeric(18,2)");
        sale.Property(x => x.TokenNo).HasColumnName("Token_No").HasMaxLength(5);
        sale.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        sale.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        sale.Property(x => x.VAT).HasColumnType("numeric(18,2)");
        sale.HasMany(x => x.Items).WithOne(x => x.Sale).HasForeignKey(x => x.BillNo).OnDelete(DeleteBehavior.Cascade);

        var saleSub = b.Entity<CanteenSaleSub>();
        saleSub.ToTable("Canteen_Sales_Sub");
        saleSub.HasKey(x => x.ID);
        saleSub.Property(x => x.BillNo).HasColumnName("Bill_No");
        saleSub.Property(x => x.ServiceID).HasColumnName("Service_ID");
        saleSub.Property(x => x.Rate).HasColumnType("decimal(18,2)");
        saleSub.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("decimal(18,2)");
        saleSub.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
        saleSub.Property(x => x.Total).HasColumnType("decimal(18,2)");
        saleSub.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);

        var challan = b.Entity<CanteenChallan>();
        challan.ToTable("Canteen_Challan");
        challan.HasKey(x => x.ID);
        challan.Property(x => x.InvoiceNo).HasColumnName("Invoice_No").HasMaxLength(20);
        challan.Property(x => x.InvoiceDate).HasColumnName("Invoice_Date").HasColumnType("datetime");
        challan.Property(x => x.IsPosted).HasColumnName("Is_Posted");
        challan.Property(x => x.Total).HasColumnType("numeric(18,2)");
        challan.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        challan.Property(x => x.VAT).HasColumnType("numeric(18,2)");
        challan.Property(x => x.EnteredBy).HasColumnName("Entered_By");
        challan.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        challan.HasMany(x => x.Items).WithOne(x => x.Challan).HasForeignKey(x => x.ChallanNo).OnDelete(DeleteBehavior.Cascade);

        var challanSub = b.Entity<CanteenChallanSub>();
        challanSub.ToTable("Canteen_Challan_Sub");
        challanSub.HasKey(x => x.ID);
        challanSub.Property(x => x.ChallanNo).HasColumnName("Challan_No");
        challanSub.Property(x => x.ServiceID).HasColumnName("Service_ID");
        challanSub.Property(x => x.Total).HasColumnType("numeric(18,2)");
        challanSub.Property(x => x.BuyRate).HasColumnName("Buy_Rate").HasColumnType("numeric(18,2)");
        challanSub.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        challanSub.Property(x => x.Discount).HasColumnType("numeric(18,2)");
        challanSub.Property(x => x.VAT).HasColumnType("numeric(18,2)");
        challanSub.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ServiceID).OnDelete(DeleteBehavior.Restrict);

        var debtor = b.Entity<CanteenDebtor>();
        debtor.ToTable("Canteen_Debtor");
        debtor.HasKey(x => x.ID);
        debtor.Property(x => x.Name).HasMaxLength(50);
        debtor.Property(x => x.Designation).HasMaxLength(50);
        debtor.Property(x => x.Balance).HasColumnType("decimal(18,2)");

        var debtorLedger = b.Entity<CanteenDebtorLedger>();
        debtorLedger.ToTable("Canteen_Debtor_Ledger");
        debtorLedger.HasKey(x => x.ID);
        debtorLedger.Property(x => x.DebtorID).HasColumnName("Debtor_ID");
        debtorLedger.Property(x => x.BillNo).HasColumnName("Bill_No");
        debtorLedger.Property(x => x.Debit).HasColumnType("decimal(18,2)");
        debtorLedger.Property(x => x.Credit).HasColumnType("decimal(18,2)");
        debtorLedger.Property(x => x.Balance).HasColumnType("decimal(18,2)");
        debtorLedger.Property(x => x.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
        debtorLedger.HasOne(x => x.Debtor).WithMany().HasForeignKey(x => x.DebtorID).OnDelete(DeleteBehavior.Restrict);
    }
}
