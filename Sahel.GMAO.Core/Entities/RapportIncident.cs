using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class RapportIncident
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NumeroRapport { get; set; } = string.Empty;

    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    public DateTime DateIncident { get; set; }
    public string Atelier { get; set; } = string.Empty; // Added for DOC 14 header
    public string DescriptionIncident { get; set; } = string.Empty;
    public string DegatsConstates { get; set; } = string.Empty; // Damages observed (DOC 14)
    public string CausesProbables { get; set; } = string.Empty;
    public string ActionsCorrectives { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty; // Formal conclusion (DOC 14)

    // Three-signatory footer (DOC 14)
    public string? NomPreparateur { get; set; }
    public string? NomChefService { get; set; }
    public string? NomChefDeptMaintenance { get; set; }

    public int RedacteurId { get; set; }
    public virtual User Redacteur { get; set; } = null!;
}
