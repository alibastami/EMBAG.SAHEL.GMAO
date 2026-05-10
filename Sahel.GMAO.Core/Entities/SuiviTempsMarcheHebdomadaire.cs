using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

/// <summary>
/// DOC 08 & 09 — Suivi du Temps de Marche Hebdomadaire.
/// Tracks running hours of a specific equipment on a weekly basis.
/// Used to generate both the weekly summary table (DOC 08) and the
/// annual monthly aggregation matrix (DOC 09).
/// </summary>
public class SuiviTempsMarcheHebdomadaire
{
    [Key]
    public int Id { get; set; }

    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    public int Annee { get; set; }

    /// <summary>
    /// Month (1-12)
    /// </summary>
    [Range(1, 12)]
    public int Mois { get; set; }

    /// <summary>
    /// Week within the month (1-5 as per the template).
    /// </summary>
    [Range(1, 5)]
    public int Semaine { get; set; }

    /// <summary>
    /// Operating hours for this specific week.
    /// </summary>
    public double HeuresMarche { get; set; }

    /// <summary>
    /// Cumulative operating hours.
    /// In a fully integrated system, this could be calculated on the fly, 
    /// but the template requires tracking the running total at that point in time.
    /// </summary>
    public double HeuresCumulees { get; set; }
}
