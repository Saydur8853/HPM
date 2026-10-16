using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPM.Web.Data.Entities;

[Table("Pathology_Group_Head")]
public sealed class PathologyGroupHead
{
    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
}

[Table("Lab_Consultent")]
public sealed class LabConsultant
{
    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Degree { get; set; }

    [MaxLength(300)]
    public string? Others { get; set; }

    [MaxLength(300)]
    public string? Institution { get; set; }

    public int? Module { get; set; }

    public int? Consultent2 { get; set; }

    public byte[]? Signature { get; set; }

    public bool Is_Active { get; set; } = true;

    [MaxLength(200)]
    public string? Title1 { get; set; }

    [MaxLength(200)]
    public string? Title2 { get; set; }
}
