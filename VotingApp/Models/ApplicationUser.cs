using Microsoft.AspNetCore.Identity;

namespace VotingApp.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? VotedAt { get; set; }
    public virtual Vote? Vote { get; set; }
}
