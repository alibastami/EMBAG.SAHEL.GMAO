using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

/// <summary>
/// DOC 06 — Questionnaire Arrêt Technique.
/// Pre-incident anomaly questionnaire filled by the conductor/chef d'atelier.
/// Distinct from RapportIncident (which is post-incident, filled by maintenance).
/// </summary>
public class QuestionnaireArretTechnique
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NumeroQuestionnaire { get; set; } = string.Empty;

    public int EquipementId { get; set; }
    public virtual Equipement Equipement { get; set; } = null!;

    [Required]
    public string Atelier { get; set; } = string.Empty;

    [Required]
    public string Section { get; set; } = string.Empty;

    public int Annee { get; set; } = DateTime.Now.Year;

    [Required]
    public string ChefAtelier { get; set; } = string.Empty;

    [Required]
    public string NomConducteur { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    // Signature timestamps
    public DateTime? VisaChefAtelier { get; set; }
    public DateTime? VisaConducteur { get; set; }

    public virtual ICollection<LigneAnomalie> Anomalies { get; set; } = new List<LigneAnomalie>();
}

/// <summary>
/// Individual anomaly line within a QuestionnaireArretTechnique.
/// Maps to the 4-column table in DOC 06: Pos | Organe | Anomalies Constatées | Observation.
/// </summary>
public class LigneAnomalie
{
    [Key]
    public int Id { get; set; }

    public int QuestionnaireId { get; set; }
    public virtual QuestionnaireArretTechnique Questionnaire { get; set; } = null!;

    public int Position { get; set; }

    [Required]
    public string Organe { get; set; } = string.Empty;

    [Required]
    public string AnomalieConstatee { get; set; } = string.Empty;

    public string? Observation { get; set; }
}
