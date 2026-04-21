using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty; // DSI, Mécanicien, Électricien

    public string Position { get; set; } = string.Empty;

    // Privileges
    public bool CanManageUsers { get; set; } = false;
    public bool CanViewAudit { get; set; } = false;
    public bool CanEditInventory { get; set; } = false;
}
