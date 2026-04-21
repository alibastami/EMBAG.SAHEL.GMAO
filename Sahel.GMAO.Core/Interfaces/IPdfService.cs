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
}
