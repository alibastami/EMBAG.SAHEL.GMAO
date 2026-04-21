using System.ComponentModel.DataAnnotations;
using Sahel.GMAO.Core.Enums;

namespace Sahel.GMAO.Core.Entities;

public class DemandeTravail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NumeroDT { get; set; } = string.Empty; // e.g., DT-2024-001

    public DateTime DateEmission { get; set; } = DateTime.Now;

    // Applicant
    public int DemandeurId { get; set; }
    public virtual User Demandeur { get; set; } = null!;

    // Equipment
    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    [Required]
    public string OrganePartie { get; set; } = string.Empty;

    public double? TempsDeMarcheHeures { get; set; }

    [Required]
    public string TravailDemande { get; set; } = string.Empty;
    
    public Specialite? SpecialiteRequise { get; set; }

    public string? InstructionsPreparation { get; set; }
    public DateTime? DateReceptionPrevue { get; set; }

    public StatutDT Statut { get; set; } = StatutDT.EnAttente;

    // Execution Info
    public string? TravailExecute { get; set; }
    public DateTime? DateExecutionDebut { get; set; }
    public DateTime? DateExecutionFin { get; set; }
    public double DureeArretProductionHeures { get; set; }

    // Cost Totals (Calculated from Consommables and Labor)
    public decimal TotalCoutPieces { get; set; }
    public decimal TotalCoutMainOeuvre { get; set; }
    public decimal TotalCoutOperation { get; set; }

    // Navigation properties
    public virtual ICollection<ConsommableUsage> Consommables { get; set; } = new List<ConsommableUsage>();
    public virtual ICollection<InterventionRole> Intervenants { get; set; } = new List<InterventionRole>();
    public virtual ICollection<BonDeConsignation> BonsDeConsignation { get; set; } = new List<BonDeConsignation>();
    public virtual ICollection<DemandeFabrication> DemandesFabrication { get; set; } = new List<DemandeFabrication>();
}
