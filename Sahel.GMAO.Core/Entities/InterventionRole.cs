using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class InterventionRole
{
    [Key]
    public int Id { get; set; }

    public int DemandeTravailId { get; set; }
    public virtual DemandeTravail DemandeTravail { get; set; } = null!;

    public int IntervenantId { get; set; }
    public virtual User Intervenant { get; set; } = null!;

    public string Qualification { get; set; } = string.Empty;
    public double HeuresTravaillees { get; set; }
    public decimal TauxHoraire { get; set; }

    public virtual ICollection<PointageIntervention> Pointages { get; set; } = new List<PointageIntervention>();
}
