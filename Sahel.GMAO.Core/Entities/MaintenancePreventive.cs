using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class MaintenancePreventive
{
    [Key]
    public int Id { get; set; }

    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    [Required]
    public string Operation { get; set; } = string.Empty;

    public int FrequenceJours { get; set; } // e.g., 30 for monthly

    public DateTime? DateDerniereRealisation { get; set; }
    public DateTime DateProchaineEcheance { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    public string? ConsignesParticulieres { get; set; }
}
