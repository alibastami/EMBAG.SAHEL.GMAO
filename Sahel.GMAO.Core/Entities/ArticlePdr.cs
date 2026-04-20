using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class ArticlePdr
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string CodeArticle { get; set; } = string.Empty;

    [Required]
    public string Designation { get; set; } = string.Empty;

    public string? ReferenceConstructeur { get; set; }

    [Required]
    public string Unite { get; set; } = "U"; // U, Kg, M, etc.

    public decimal PrixUnitaire { get; set; }

    public double QuantiteEnStock { get; set; }
    public double SeuilAlerte { get; set; }

    // Navigation
    public virtual ICollection<ConsommableUsage> Usages { get; set; } = new List<ConsommableUsage>();
}
