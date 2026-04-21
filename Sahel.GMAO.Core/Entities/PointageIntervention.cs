using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class PointageIntervention
{
    [Key]
    public int Id { get; set; }

    public int InterventionRoleId { get; set; }
    public virtual InterventionRole InterventionRole { get; set; } = null!;

    public DateTime DateIntervention { get; set; }
    
    // Pour simplifier l'accès, on peut stocker le jour du mois (1-31) directement
    public int JourDuMois { get; set; }

    public double HeuresTravaillees { get; set; }
}
