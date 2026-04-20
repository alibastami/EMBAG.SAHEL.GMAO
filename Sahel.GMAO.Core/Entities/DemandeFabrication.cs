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

    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    [Required]
    public string DesignationPiece { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public double Quantite { get; set; }

    public DateTime DateEmission { get; set; } = DateTime.Now;
    public DateTime DateSouhaitee { get; set; }

    public StatutFabrication Statut { get; set; } = StatutFabrication.EnAttente;

    public string? PlanJointUrl { get; set; } // Optional: Link to a file or document
    public string? Observations { get; set; }
}
