using System.ComponentModel.DataAnnotations;
using Sahel.GMAO.Core.Enums;

namespace Sahel.GMAO.Core.Entities;

public class WorkingProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // e.g. "Mécanicien Senior"

    public decimal HourlyRate { get; set; } // Taux (DA)
    public LaborUnit Unit { get; set; } = LaborUnit.Heure;

    public Specialite Specialite { get; set; } // Link to the speciality this profile belongs to
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
