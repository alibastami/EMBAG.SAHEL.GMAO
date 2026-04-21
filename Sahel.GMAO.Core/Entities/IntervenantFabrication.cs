using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class IntervenantFabrication
{
    [Key]
    public int Id { get; set; }

    public int DemandeFabricationId { get; set; }
    public virtual DemandeFabrication DemandeFabrication { get; set; } = null!;

    public string NomsEtPrenoms { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public double NombreHeures { get; set; }
    public decimal CoutHoraire { get; set; }
    public decimal Total => (decimal)NombreHeures * CoutHoraire;
}
