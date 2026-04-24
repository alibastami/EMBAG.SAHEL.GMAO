using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class InterventionLog
{
    [Key]
    public int Id { get; set; }

    public int DemandeTravailId { get; set; }
    public virtual DemandeTravail DemandeTravail { get; set; } = null!;

    public int IntervenantId { get; set; }
    public virtual User Intervenant { get; set; } = null!;

    public DateTime DateIntervention { get; set; } = DateTime.Now;

    [Required]
    public string TravailExecute { get; set; } = string.Empty;

    public double DureeHeures { get; set; }
    public double ArretProductionHeures { get; set; }
}
