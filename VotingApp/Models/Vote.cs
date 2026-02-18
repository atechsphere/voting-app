using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingApp.Models;

public class Vote
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Candidate { get; set; } = string.Empty;
    
    public DateTime VotedAt { get; set; }
    
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
