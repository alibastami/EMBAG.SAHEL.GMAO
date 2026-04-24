using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public AuthService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        using var context = await _factory.CreateDbContextAsync();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return null;

        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return user;
        }

        return null;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Users.Include(u => u.WorkingProfile).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task CreateUserAsync(User user, string plainPassword)
    {
        using var context = await _factory.CreateDbContextAsync();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user, string? newPassword = null)
    {
        using var context = await _factory.CreateDbContextAsync();
        var existing = await context.Users.FindAsync(user.Id);
        if (existing != null)
        {
            existing.Username = user.Username;
            existing.FullName = user.FullName;
            existing.Role = user.Role;
            existing.Position = user.Position;
            existing.Specialite = user.Specialite;
            existing.WorkingProfileId = user.WorkingProfileId;
            existing.CanManageUsers = user.CanManageUsers;
            existing.CanViewAudit = user.CanViewAudit;
            existing.CanEditInventory = user.CanEditInventory;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
