using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public StockService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<ArticlePdr>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.ArticlesPdr.ToListAsync();
    }

    public async Task<ArticlePdr?> GetByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.ArticlesPdr.FindAsync(id);
    }

    public async Task<ArticlePdr?> GetByCodeAsync(string code)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.ArticlesPdr.FirstOrDefaultAsync(a => a.CodeArticle == code);
    }

    public async Task CreateArticleAsync(ArticlePdr article)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.ArticlesPdr.Add(article);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStockAsync(int articleId, double quantiteVariation)
    {
        using var context = await _factory.CreateDbContextAsync();
        var article = await context.ArticlesPdr.FindAsync(articleId);
        if (article != null)
        {
            article.QuantiteEnStock += quantiteVariation;
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<ArticlePdr>> GetAlertesStockAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.ArticlesPdr
            .Where(a => a.QuantiteEnStock <= a.SeuilAlerte)
            .ToListAsync();
    }
}
