using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class Equipement
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty; // e.g., P-101

    [Required]
    [StringLength(200)]
    public string Designation { get; set; } = string.Empty;

    public string? Marque { get; set; }
    public string? Modele { get; set; }
    public string? NumeroSerie { get; set; }

    [Required]
    public string Section { get; set; } = string.Empty; // e.g., OFFSET, FACONNAGE

    public string? SousSection { get; set; }

    public DateTime? DateMiseEnService { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    // Technical characteristics (JSON or key-value later?)
    public string? CaracteristiquesTechniques { get; set; }

    // Navigation properties
    public virtual ICollection<DemandeTravail> DemandesTravail { get; set; } = new List<DemandeTravail>();
}
