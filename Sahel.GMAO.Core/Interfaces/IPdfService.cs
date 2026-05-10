using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateDtPdfAsync(DemandeTravail dt);
    Task<byte[]> GenerateFabricationPdfAsync(DemandeFabrication df);
    Task<byte[]> GenerateConsignationPdfAsync(BonDeConsignation bon);
    Task<byte[]> GenerateFicheEntretienPdfAsync(FicheEntretienPreventif fiche);
    Task<byte[]> GenerateEquipementHistoryPdfAsync(Equipement equipement, List<DemandeTravail> interventions);
    Task<byte[]> GenerateStockStatePdfAsync(List<ArticlePdr> articles);
    Task<byte[]> GeneratePlanningPdfAsync(List<MaintenancePreventive> preventives);

    // New Generators from Analysis
    Task<byte[]> GenerateQuestionnaireArretPdfAsync(QuestionnaireArretTechnique q);
    Task<byte[]> GeneratePlanningEntretienPreventifPdfAsync(List<Equipement> equipements, List<MaintenancePreventive> plannings, int annee);
    Task<byte[]> GenerateSuiviTempsMarchePdfAsync(Equipement eq, List<SuiviTempsMarcheHebdomadaire> suivi, int[] annees);
    Task<byte[]> GenerateHeuresMachinesPdfAsync(List<Equipement> equipements, List<SuiviTempsMarcheHebdomadaire> suivi, int annee);
    Task<byte[]> GeneratePlanningVisitePeriodiquePdfAsync(List<Equipement> equipements, List<MaintenancePreventive> plannings, int annee);
    Task<byte[]> GenerateFicheInspectionPdfAsync(FicheEntretienPreventif fiche);
    Task<byte[]> GenerateTravauxNonProgrammesPdfAsync(DemandeTravail dt);
    Task<byte[]> GenerateTravauxProgrammesPdfAsync(FicheEntretienPreventif fiche);
    Task<byte[]> GenerateRapportCirconstancielPdfAsync(RapportIncident rapport);
}
