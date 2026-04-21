using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class MatiereFabrication
{
    [Key]
    public int Id { get; set; }

    public int DemandeFabricationId { get; set; }
    public virtual DemandeFabrication DemandeFabrication { get; set; } = null!;

    public string N_BSM { get; set; } = string.Empty;
    public string DesignationMatiere { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public double Longueur { get; set; }
    public decimal Prix { get; set; }
    public string? Observation { get; set; }
}
