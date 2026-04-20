using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class ConsommableUsage
{
    [Key]
    public int Id { get; set; }

    public int DemandeTravailId { get; set; }
    public virtual DemandeTravail DemandeTravail { get; set; } = null!;

    public int ArticlePdrId { get; set; }
    public virtual ArticlePdr ArticlePdr { get; set; } = null!;

    public double Quantite { get; set; }
    public decimal PrixUnitaireApplique { get; set; } // Historical price at time of use
}
