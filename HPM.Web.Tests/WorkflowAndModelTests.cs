using HPM.Web.Data;
using HPM.Web.Data.Entities;
using HPM.Web.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HPM.Web.Tests;

public sealed class WorkflowAndModelTests
{
    [Theory]
    [InlineData(null, "InProcess", true)]
    [InlineData("Pending", "InProcess", true)]
    [InlineData("InProcess", "Completed", true)]
    [InlineData("Pending", "Completed", false)]
    [InlineData("Completed", "InProcess", false)]
    [InlineData("Completed", "Completed", false)]
    public void Diagnostic_status_transitions_are_forward_only(string? current, string next, bool expected) =>
        Assert.Equal(expected, DiagnosticWorkflow.CanAdvance(current, next));

    [Theory]
    [InlineData(EncounterStatuses.Waiting, EncounterStatuses.InConsultation, true)]
    [InlineData(EncounterStatuses.InConsultation, EncounterStatuses.Completed, true)]
    [InlineData(EncounterStatuses.Waiting, EncounterStatuses.Completed, false)]
    [InlineData(EncounterStatuses.Completed, EncounterStatuses.InConsultation, false)]
    public void Clinical_encounter_status_transitions_are_controlled(string current, string next, bool expected) =>
        Assert.Equal(expected, ClinicalEncounterWorkflow.CanTransition(current, next));

    [Fact]
    public void Operational_roles_are_unique_and_include_lab_technician()
    {
        Assert.Contains(RoleNames.LabTechnician, RoleNames.All);
        Assert.Equal(RoleNames.All.Length, RoleNames.All.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Module_navigation_permissions_match_operational_roles()
    {
        var doctor = Principal(RoleNames.Doctor);
        Assert.False(ModuleAccess.CanView(doctor, ModuleAccess.PatientRegistration));
        Assert.False(ModuleAccess.CanView(doctor, ModuleAccess.Appointments));
        Assert.True(ModuleAccess.CanView(doctor, ModuleAccess.DoctorQueue));
        Assert.True(ModuleAccess.CanView(doctor, ModuleAccess.DiagnosticResults));
        Assert.True(ModuleAccess.CanView(doctor, ModuleAccess.PharmacyPrescriptions));

        var reception = Principal(RoleNames.Reception);
        Assert.True(ModuleAccess.CanView(reception, ModuleAccess.PatientRegistration));
        Assert.True(ModuleAccess.CanView(reception, ModuleAccess.Appointments));
        Assert.False(ModuleAccess.CanView(reception, ModuleAccess.Accounts));
        Assert.False(ModuleAccess.CanView(reception, ModuleAccess.PharmacyPos));

        var lab = Principal(RoleNames.LabTechnician);
        Assert.True(ModuleAccess.CanView(lab, ModuleAccess.DiagnosticWorklist));
        Assert.True(ModuleAccess.CanView(lab, ModuleAccess.DiagnosticResults));
        Assert.False(ModuleAccess.CanView(lab, ModuleAccess.DiagnosticBilling));

        var pharmacy = Principal(RoleNames.PharmacyOperator);
        Assert.True(ModuleAccess.CanView(pharmacy, ModuleAccess.PharmacyPos));
        Assert.True(ModuleAccess.CanView(pharmacy, ModuleAccess.PharmacyStock));
        Assert.False(ModuleAccess.CanView(pharmacy, ModuleAccess.PatientRegistration));

        var cashier = Principal(RoleNames.Cashier);
        Assert.True(ModuleAccess.CanView(cashier, ModuleAccess.Accounts));
        Assert.True(ModuleAccess.CanView(cashier, ModuleAccess.DiagnosticBilling));
        Assert.False(ModuleAccess.CanView(cashier, ModuleAccess.PatientRegistration));

        var emergency = Principal(RoleNames.EmergencyOperator);
        Assert.True(ModuleAccess.CanView(emergency, ModuleAccess.EmergencyServices));
        Assert.False(ModuleAccess.CanView(emergency, ModuleAccess.OpdTickets));

        var ot = Principal(RoleNames.OTOperator);
        Assert.True(ModuleAccess.CanView(ot, ModuleAccess.OtCashMemos));
        Assert.False(ModuleAccess.CanView(ot, ModuleAccess.EmergencyServices));

        var ipd = Principal(RoleNames.IPDOperator);
        Assert.True(ModuleAccess.CanView(ipd, ModuleAccess.IpdAdmissions));
        Assert.True(ModuleAccess.CanView(ipd, ModuleAccess.IpdBilling));
        Assert.False(ModuleAccess.CanView(ipd, ModuleAccess.PatientRegistration));

        var nurse = Principal(RoleNames.Nurse);
        Assert.True(ModuleAccess.CanView(nurse, ModuleAccess.DoctorQueue));
        Assert.False(ModuleAccess.CanView(nurse, ModuleAccess.IpdBilling));

        var administrator = Principal(RoleNames.Administrator);
        Assert.All(
            new[] { ModuleAccess.PatientRegistration, ModuleAccess.IpdBilling, ModuleAccess.DiagnosticWorklist, ModuleAccess.PharmacyStock, ModuleAccess.Accounts },
            roles => Assert.True(ModuleAccess.CanView(administrator, roles)));
    }

    [Fact]
    public void Model_enforces_per_test_samples_and_batch_trace_uniqueness()
    {
        var options = new DbContextOptionsBuilder<HpmDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PrimeHpmModelOnly;Trusted_Connection=True")
            .Options;
        using var db = new HpmDbContext(options);

        var sample = db.Model.FindEntityType(typeof(LabSampleCollection))!;
        Assert.Contains(sample.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(LabSampleCollection.MemoID), nameof(LabSampleCollection.ServiceID)]));

        var allocation = db.Model.FindEntityType(typeof(PharmacySaleBatchAllocation))!;
        Assert.Contains(allocation.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(PharmacySaleBatchAllocation.CashMemoItemID), nameof(PharmacySaleBatchAllocation.StockBalanceID)]));
        var appUser = db.Model.FindEntityType(typeof(AppUser))!;
        Assert.NotNull(appUser.FindProperty(nameof(AppUser.MustChangePassword)));
        Assert.Contains(appUser.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(AppUser.DoctorProfileID)]));

        var encounter = db.Model.FindEntityType(typeof(PatientEncounter))!;
        Assert.Contains(encounter.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(PatientEncounter.AppointmentID)]));
        Assert.Contains(encounter.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(PatientEncounter.TicketID)]));

        var prescription = db.Model.FindEntityType(typeof(Prescription))!;
        Assert.Contains(prescription.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(Prescription.EncounterID)]));

        var diagnosticOrderItem = db.Model.FindEntityType(typeof(DiagnosticOrderItem))!;
        Assert.Contains(diagnosticOrderItem.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(DiagnosticOrderItem.OrderID), nameof(DiagnosticOrderItem.ServiceID)]));

        var diagnosticOrder = db.Model.FindEntityType(typeof(DiagnosticOrder))!;
        Assert.Contains(diagnosticOrder.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(DiagnosticOrder.BilledMemoID)]));

        var dischargeOrder = db.Model.FindEntityType(typeof(IpdMedicalDischargeOrder))!;
        Assert.Contains(dischargeOrder.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(IpdMedicalDischargeOrder.AdmissionID)]));

        var finalBill = db.Model.FindEntityType(typeof(IpdFinalBill))!;
        Assert.Contains(finalBill.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(IpdFinalBill.MedicalDischargeOrderID)]));

        var bed = db.Model.FindEntityType(typeof(BedCabin))!;
        Assert.NotNull(bed.FindProperty(nameof(BedCabin.Status)));
        var turnover = db.Model.FindEntityType(typeof(BedTurnover))!;
        Assert.Contains(turnover.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(x => x.Name).SequenceEqual([nameof(BedTurnover.BedCabinID)]));
    }

    [Fact]
    public void BarcodeQrService_Generates_Valid_Code128_And_QrCode_Svgs()
    {
        var service = new HPM.Web.Services.BarcodeQrService();

        // 1D Code-128 Barcode
        var barcodeSvg = service.GenerateBarcodeSvg("REG-000123", barHeight: 40);
        Assert.False(string.IsNullOrWhiteSpace(barcodeSvg));
        Assert.Contains("<svg", barcodeSvg);
        Assert.Contains("<rect", barcodeSvg);
        Assert.Contains("REG-000123", barcodeSvg);

        // 2D QR Code
        var qrSvg = service.GenerateQrSvg("HPM:PAT|123|REG-000123|John Doe", pixelsPerModule: 3);
        Assert.False(string.IsNullOrWhiteSpace(qrSvg));
        Assert.Contains("<svg", qrSvg);

        // Payloads
        var patPayload = service.GeneratePatientQrPayload(123, "REG-000123", "John Doe", "01700000000");
        Assert.StartsWith("HPM:PAT|", patPayload);

        var ipdPayload = service.GenerateWristbandQrPayload(45, "I2600001", "Jane Doe", "32y/F", "O+", "Bed-101", "Dr. Smith");
        Assert.StartsWith("HPM:WB|", ipdPayload);
        Assert.Contains("Bed-101", ipdPayload);

        var tubePayload = service.GenerateLabTubeQrPayload("SMP-9901", 1001, "John Doe", "CBC, RBS");
        Assert.StartsWith("HPM:TUBE|", tubePayload);

        var labPayload = service.GenerateLabQrPayload(1001, "P001", "John Doe", "SMP-1001", "https://hospital.com");
        Assert.Equal("https://hospital.com/portal/report/1001", labPayload);

        var dcPayload = service.GenerateDischargeQrPayload(45, "I2600001", "Jane Doe", 15000, true);
        Assert.StartsWith("HPM:DC|", dcPayload);
        Assert.Contains("CLEARED", dcPayload);
    }

    internal sealed class TestAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    private static ClaimsPrincipal Principal(string role) =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.Name, "test"), new Claim(ClaimTypes.Role, role)], "Test"));
}
