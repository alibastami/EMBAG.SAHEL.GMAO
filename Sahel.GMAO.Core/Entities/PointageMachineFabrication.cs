using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class PointageMachineFabrication
{
    [Key]
    public int Id { get; set; }

    public int DemandeFabricationId { get; set; }
    public virtual DemandeFabrication DemandeFabrication { get; set; } = null!;

    public int IntervenantId { get; set; }
    public virtual User Intervenant { get; set; } = null!;

    public double Tour200Heures { get; set; }
    public double Tour250Heures { get; set; }
    public double Tour300Heures { get; set; }
    public double FraisageHeures { get; set; }
    public double EtauLimeurHeures { get; set; }
    public double AffutageHeures { get; set; }
    public double SoudureHeures { get; set; }
    public double AutresHeures { get; set; }
}
