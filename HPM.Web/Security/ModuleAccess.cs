using System.Security.Claims;

namespace HPM.Web.Security;

public static class ModuleAccess
{
    public static readonly string[] AdministratorOnly = [RoleNames.Administrator];
    public static readonly string[] PatientRegistration = [RoleNames.Administrator, RoleNames.Reception];
    public static readonly string[] Appointments = [RoleNames.Administrator, RoleNames.Reception];
    public static readonly string[] OpdTickets = [RoleNames.Administrator, RoleNames.Reception, RoleNames.Cashier];
    public static readonly string[] EmergencyServices = [RoleNames.Administrator, RoleNames.Cashier, RoleNames.EmergencyOperator];
    public static readonly string[] OtCashMemos = [RoleNames.Administrator, RoleNames.Cashier, RoleNames.OTOperator];
    public static readonly string[] IpdAdmissions = [RoleNames.Administrator, RoleNames.Reception, RoleNames.IPDOperator];
    public static readonly string[] IpdOccupancy = [RoleNames.Administrator, RoleNames.Reception, RoleNames.IPDOperator, RoleNames.Doctor];
    public static readonly string[] IpdMoneyReceipts = [RoleNames.Administrator, RoleNames.Cashier, RoleNames.IPDOperator];
    public static readonly string[] IpdPatientServices = [RoleNames.Administrator, RoleNames.IPDOperator, RoleNames.Cashier, RoleNames.Doctor];
    public static readonly string[] IpdBilling = [RoleNames.Administrator, RoleNames.Cashier, RoleNames.IPDOperator];
    public static readonly string[] IpdMedicalDischarge = [RoleNames.Administrator, RoleNames.Doctor];
    public static readonly string[] DiagnosticBilling = [RoleNames.Administrator, RoleNames.Cashier, RoleNames.Reception];
    public static readonly string[] DiagnosticWorklist = [RoleNames.Administrator, RoleNames.LabTechnician];
    public static readonly string[] DiagnosticResults = [RoleNames.Administrator, RoleNames.LabTechnician, RoleNames.Doctor];
    public static readonly string[] DoctorQueue = [RoleNames.Administrator, RoleNames.Doctor, RoleNames.Nurse];
    public static readonly string[] PharmacyPos = [RoleNames.Administrator, RoleNames.PharmacyOperator, RoleNames.Cashier];
    public static readonly string[] PharmacyPurchases = [RoleNames.Administrator, RoleNames.PharmacyOperator];
    public static readonly string[] PharmacyReturns = [RoleNames.Administrator, RoleNames.PharmacyOperator, RoleNames.Cashier];
    public static readonly string[] PharmacyPrescriptions = [RoleNames.Administrator, RoleNames.Doctor, RoleNames.PharmacyOperator];
    public static readonly string[] PharmacyStock = [RoleNames.Administrator, RoleNames.PharmacyOperator];
    public static readonly string[] BloodBank = [RoleNames.Administrator, RoleNames.BloodBankOperator, RoleNames.LabTechnician, RoleNames.Doctor, RoleNames.Nurse, RoleNames.IPDOperator];
    public static readonly string[] Canteen = [RoleNames.Administrator, RoleNames.Cashier];
    public static readonly string[] Reports = [RoleNames.Administrator, RoleNames.Reception, RoleNames.Cashier];
    public static readonly string[] Accounts = [RoleNames.Administrator, RoleNames.Cashier];
    public static readonly string[] OperationList = [RoleNames.Administrator, RoleNames.Doctor, RoleNames.OTOperator, RoleNames.Nurse];
    public static readonly string[] OtNotes = [RoleNames.Administrator, RoleNames.Doctor, RoleNames.OTOperator];
    public static readonly string[] NurseDesk = [RoleNames.Administrator, RoleNames.Nurse, RoleNames.Doctor, RoleNames.Reception];
    public static readonly string[] MedicalCertificates = [RoleNames.Administrator, RoleNames.Doctor, RoleNames.Reception, RoleNames.Nurse];
    public static readonly string[] PatientDocuments =
    [
        RoleNames.Administrator, RoleNames.Reception, RoleNames.Cashier, RoleNames.Doctor,
        RoleNames.LabTechnician, RoleNames.Nurse, RoleNames.EmergencyOperator, RoleNames.OTOperator
    ];

    public static readonly string[] OpdGroup = Union(PatientRegistration, Appointments, OpdTickets, EmergencyServices, OtCashMemos);
    public static readonly string[] IpdGroup = Union(IpdAdmissions, IpdOccupancy, IpdMoneyReceipts, IpdPatientServices, IpdBilling, IpdMedicalDischarge);
    public static readonly string[] DiagnosticGroup = Union(DiagnosticBilling, DiagnosticWorklist, DiagnosticResults);
    public static readonly string[] DoctorGroup = Union(DoctorQueue, PharmacyPrescriptions, AdministratorOnly);
    public static readonly string[] MedicalGroup = Union(DoctorQueue, PharmacyPrescriptions, DiagnosticBilling, OtCashMemos, OperationList, OtNotes, NurseDesk, MedicalCertificates);
    public static readonly string[] PharmacyGroup = Union(PharmacyPos, PharmacyPurchases, PharmacyReturns, PharmacyPrescriptions, PharmacyStock);

    public static bool CanView(ClaimsPrincipal user, IReadOnlyList<string> roles) =>
        roles.Count == 0 || roles.Any(user.IsInRole);

    public static IReadOnlyList<string> LegacyModuleRoles(string? module) => module switch
    {
        "Canteen" => Canteen,
        "Store" or "Payroll & HRM" => AdministratorOnly,
        "IPD Services" => IpdOccupancy,
        "Medical Services" => OtCashMemos,
        "Nurse's Desk" or "Certificates & Others" => DoctorQueue,
        _ => AdministratorOnly
    };

    private static string[] Union(params IReadOnlyList<string>[] roleSets) =>
        roleSets.SelectMany(x => x).Distinct(StringComparer.Ordinal).ToArray();
}
