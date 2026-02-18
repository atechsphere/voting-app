using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VotingApp.Models;

namespace VotingApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Vote> Votes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure Vote entity
        builder.Entity<Vote>()
            .HasOne(v => v.User)
            .WithOne(u => u.Vote)
            .HasForeignKey<Vote>(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Add unique constraint on UserId in Votes table
        builder.Entity<Vote>()
            .HasIndex(v => v.UserId)
            .IsUnique();
    }
}
