using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task CreateUserAsync(User user, string plainPassword);
    Task UpdateUserAsync(User user, string? newPassword = null);
    Task DeleteUserAsync(int id);
}
