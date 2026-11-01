using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPM.Web.Data.Entities;

[Table("Patient_Operation")]
public sealed class PatientOperation
{
    [Key]
    public long ID { get; set; }

    [Column("Reg_No")]
    [MaxLength(20)]
    public string RegNo { get; set; } = string.Empty;

    [Column("Patient_ID")]
    [MaxLength(30)]
    public string PatientID { get; set; } = string.Empty;

    [Column("Procedure_Name")]
    [MaxLength(300)]
    public string ProcedureName { get; set; } = string.Empty;

    [Column("Anaesthesia_Type")]
    [MaxLength(100)]
    public string? AnaesthesiaType { get; set; }

    [Column("Operator_ID")]
    public int OperatorID { get; set; }

    [Column("Anasthetist_ID")]
    public int? AnasthetistID { get; set; }

    [Column("Assistant_Doctor_IDs")]
    [MaxLength(500)]
    public string? AssistantDoctorIDs { get; set; }

    [Column("Theatre_Room")]
    [MaxLength(100)]
    public string? TheatreRoom { get; set; }

    [Column("Operation_Date")]
    public DateTime OperationDate { get; set; } = DateTime.Now;

    [MaxLength(1000)]
    public string? Drug { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "Scheduled";

    [Column("Entered_By")]
    public int EnteredBy { get; set; } = 1;

    [Column("Trans_Date")]
    public DateTime TransDate { get; set; } = DateTime.Now;
}

[Table("Operation_Note")]
public sealed class OperationNote
{
    [Key]
    public int ID { get; set; }

    [Column("Operation_ID")]
    public long? OperationID { get; set; }

    [Column("Reg_No")]
    [MaxLength(20)]
    public string RegNo { get; set; } = string.Empty;

    [Column("Patient_ID")]
    [MaxLength(30)]
    public string PatientID { get; set; } = string.Empty;

    [Column("Operation_Name")]
    [MaxLength(300)]
    public string OperationName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Indication { get; set; }

    [Column("Operation_Date")]
    public DateTime OperationDate { get; set; } = DateTime.Now;

    [Column("Surgeon_ID")]
    public int SurgeonID { get; set; }

    [Column("Assistant1_ID")]
    public int? Assistant1ID { get; set; }

    [Column("Assistant2_ID")]
    public int? Assistant2ID { get; set; }

    [Column("Assistant3_ID")]
    public int? Assistant3ID { get; set; }

    [Column("Anaesthetist_ID")]
    public int? AnaesthetistID { get; set; }

    [Column("Anaesthesia_Type")]
    [MaxLength(100)]
    public string? AnaesthesiaType { get; set; }

    [Column("Pre_Op_Diagnosis")]
    [MaxLength(500)]
    public string? PreOpDiagnosis { get; set; }

    [Column("Post_Op_Diagnosis")]
    [MaxLength(500)]
    public string? PostOpDiagnosis { get; set; }

    [Column("Procedure_And_Findings")]
    public string? ProcedureAndFindings { get; set; }

    [MaxLength(500)]
    public string? Incision { get; set; }

    [Column("Specimen_Biopsy")]
    [MaxLength(500)]
    public string? SpecimenBiopsy { get; set; }

    [Column("Blood_Loss_Estimate")]
    [MaxLength(100)]
    public string? BloodLossEstimate { get; set; }

    [Column("Drains_Packs")]
    [MaxLength(500)]
    public string? DrainsPacks { get; set; }

    [Column("Post_Op_Orders")]
    [MaxLength(2000)]
    public string? PostOpOrders { get; set; }

    [Column("Entered_By")]
    public int EnteredBy { get; set; } = 1;

    [Column("Trans_Date")]
    public DateTime TransDate { get; set; } = DateTime.Now;
}

[Table("Medical_Certificate")]
public sealed class MedicalCertificate
{
    [Key]
    public long ID { get; set; }

    [Column("Certificate_No")]
    [MaxLength(50)]
    public string CertificateNo { get; set; } = string.Empty;

    [Column("Certificate_Type")]
    [MaxLength(30)]
    public string CertificateType { get; set; } = string.Empty; // "Birth", "Death", "Fitness"

    [Column("Patient_ID")]
    [MaxLength(30)]
    public string PatientID { get; set; } = string.Empty;

    [Column("Reg_No")]
    [MaxLength(20)]
    public string? RegNo { get; set; }

    [Column("Issue_Date")]
    public DateTime IssueDate { get; set; } = DateTime.Now;

    [Column("Doctor_ID")]
    public int DoctorID { get; set; }

    [Column("Details_Json")]
    public string? DetailsJson { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    [Column("Issued_By")]
    public int IssuedBy { get; set; } = 1;

    [Column("Created_Date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
