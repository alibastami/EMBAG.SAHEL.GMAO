using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class FicheEntretienPreventif
{
    [Key]
    public int Id { get; set; }

    public string NumeroOT { get; set; } = string.Empty; // N° OT
    public string Section { get; set; } = string.Empty;
    public string Periodicite { get; set; } = string.Empty;
    public string Partie { get; set; } = string.Empty; // MEC / ELEC

    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    public int? MaintenancePreventiveId { get; set; } // Link to the schedule/template
    public virtual MaintenancePreventive? MaintenancePreventive { get; set; }

    public DateTime? DateFaitLe { get; set; }
    
    public int? IntervenantId { get; set; }
    public virtual User? Intervenant { get; set; }
    public DateTime? VisaIntervenant { get; set; }

    public virtual ICollection<TacheEntretien> Taches { get; set; } = new List<TacheEntretien>();
}
