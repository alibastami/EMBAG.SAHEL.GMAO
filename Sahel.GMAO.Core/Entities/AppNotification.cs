using System;
using System.ComponentModel.DataAnnotations;

namespace Sahel.GMAO.Core.Entities;

public class AppNotification
{
    public int Id { get; set; }
    
    public int? UserId { get; set; } // Null for broadcast
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    public bool IsRead { get; set; } = false;
    
    public string? Link { get; set; } // Optional: Deep link to a specific DT or Page
}
