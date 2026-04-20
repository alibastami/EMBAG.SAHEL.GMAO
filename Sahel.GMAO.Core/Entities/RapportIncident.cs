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
    public string DescriptionIncident { get; set; } = string.Empty;
    public string CausesProbables { get; set; } = string.Empty;
    public string ActionsCorrectives { get; set; } = string.Empty;

    public int RedacteurId { get; set; }
    public virtual User Redacteur { get; set; } = null!;
}
