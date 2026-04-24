using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IStockService
{
    Task<List<ArticlePdr>> GetAllAsync();
    Task<ArticlePdr?> GetByIdAsync(int id);
    Task<ArticlePdr?> GetByCodeAsync(string code);
    Task CreateArticleAsync(ArticlePdr article);
    Task UpdateArticleAsync(ArticlePdr article);
    Task DeleteArticleAsync(int id);
    Task UpdateStockAsync(int articleId, double quantiteVariation);
    Task<List<ArticlePdr>> GetAlertesStockAsync();
}
