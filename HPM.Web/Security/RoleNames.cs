namespace HPM.Web.Security;

public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string Reception = "Reception";
    public const string Cashier = "Cashier";
    public const string EmergencyOperator = "Emergency Operator";
    public const string OTOperator = "OT Operator";
    public const string IPDOperator = "IPD Operator";
    public const string Nurse = "Nurse";
    public const string Doctor = "Doctor";
    public const string LabTechnician = "Lab Technician";
    public const string PharmacyOperator = "Pharmacy Operator";
    public const string BloodBankOperator = "Blood Bank Operator";
    public const string Patient = "Patient";
    public const string Authorizer = "Authorizer";

    public static readonly string[] All =
    [
        Administrator,
        Reception,
        Cashier,
        EmergencyOperator,
        OTOperator,
        IPDOperator,
        Nurse,
        Doctor,
        LabTechnician,
        PharmacyOperator,
        BloodBankOperator,
        Patient,
        Authorizer
    ];
}
