using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public enum StatutFabrication
{
    EnAttente,
    Validee,
    EnCours,
    Terminee,
    Annulee
}

public class DemandeFabrication
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroFabrication { get; set; } = string.Empty;

    public int? DemandeTravailId { get; set; }
    public virtual DemandeTravail? DemandeTravail { get; set; }

    public int EquipementId { get; set; } // Destination of the piece
    public virtual Equipement Equipement { get; set; } = null!;

    [Required]
    public string DesignationPiece { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public double Quantite { get; set; }

    public DateTime DateEmission { get; set; } = DateTime.Now;
    
    // Preparation
    public string? ReferencePiece { get; set; }
    public string? CodeArticle { get; set; }
    public string? NDessin { get; set; }
    public string? Matiere { get; set; }
    public double? TempsAlloueHeures { get; set; }
    public string? Consignes { get; set; }
    public bool DegreUrgenceImmediat { get; set; } // True = Immediat, False = Programme
    
    // Execution
    public DateTime? DebutTravail { get; set; }
    public DateTime? FinTravail { get; set; }
    public DateTime? ReceptionDate { get; set; }
    public DateTime? VisaExecution { get; set; }

    public StatutFabrication Statut { get; set; } = StatutFabrication.EnAttente;

    public string? PlanJointUrl { get; set; }
    public string? Observations { get; set; }

    // Preparation (Cost)
    public decimal TotalCoutPieces { get; set; }
    public decimal TotalCoutMainOeuvre { get; set; }
    public decimal TotalCoutOperation { get; set; }

    public DateTime? VisaPreparation { get; set; }
    public DateTime? VisaComptabiliteAnalytique { get; set; }

    // Grid details
    public virtual ICollection<MatiereFabrication> MatieresConsommees { get; set; } = new List<MatiereFabrication>();
    public virtual ICollection<IntervenantFabrication> Intervenants { get; set; } = new List<IntervenantFabrication>();
    public virtual ICollection<PointageMachineFabrication> PointagesMachines { get; set; } = new List<PointageMachineFabrication>();
}
