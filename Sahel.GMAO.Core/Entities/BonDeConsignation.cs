using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class BonDeConsignation
{
    [Key]
    public int Id { get; set; }

    public int? DemandeTravailId { get; set; }
    public virtual DemandeTravail? DemandeTravail { get; set; }

    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    [Required]
    public string Atelier { get; set; } = string.Empty;

    // Consignation
    public DateTime? DateHeureConsignation { get; set; }
    public string? MotifConsignation { get; set; }
    
    public int? AgentConsignationId { get; set; }
    public virtual User? AgentConsignation { get; set; }
    
    public string? ServiceUtilisateurConsignation { get; set; }
    public DateTime? VisaConsignation { get; set; } // Signature

    // Deconsignation
    public DateTime? DateHeureDeconsignation { get; set; }
    
    public int? AgentDeconsignationId { get; set; }
    public virtual User? AgentDeconsignation { get; set; }
    
    public string? ServiceUtilisateurDeconsignation { get; set; }
    public DateTime? VisaDeconsignation { get; set; } // Signature
}
