using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class TacheEntretien
{
    [Key]
    public int Id { get; set; }

    public int FicheEntretienPreventifId { get; set; }
    public virtual FicheEntretienPreventif FicheEntretienPreventif { get; set; } = null!;

    public int Position { get; set; }
    public string Organes { get; set; } = string.Empty;
    public string OperationAEffectuer { get; set; } = string.Empty;
    public bool EstFait { get; set; }
    
    public double TempsPrevuHeures { get; set; }
    public double TempsRealiseHeures { get; set; }
    
    public string? Observation { get; set; }
    public string? SuiteADonner { get; set; }
    public string? Consigne { get; set; }    // For Inspection Périodique (DOC 11)
    public string? ValeurMesuree { get; set; } // Measured value for inspection (DOC 11)
}
