using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Web.Auth;
using Serilog;

namespace Sahel.GMAO.Web.Controllers;

[Route("Account")]
public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
    {
        Log.Information("[Auth] Login attempt for user: {Username} via AccountController", username);

        var user = await _authService.AuthenticateAsync(username, password);
        if (user == null)
        {
            Log.Warning("[Auth] Login failed for user: {Username}", username);
            return Redirect("/login?error=true");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", user.FullName),
            new Claim("Position", user.Position),
            new Claim("CanManageUsers", user.CanManageUsers.ToString().ToLower()),
            new Claim("CanViewAudit", user.CanViewAudit.ToString().ToLower()),
            new Claim("CanEditInventory", user.CanEditInventory.ToString().ToLower())
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });

        Log.Information("[Auth] Login successful. Cookie issued for user: {Username}", username);

        return Redirect("/");
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        Log.Information("[Auth] Logging out user: {User}", User.Identity?.Name);
        await HttpContext.SignOutAsync("Cookies");
        return Redirect("/login");
    }
}
