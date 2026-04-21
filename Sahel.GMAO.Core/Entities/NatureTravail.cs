using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class NatureTravail
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
