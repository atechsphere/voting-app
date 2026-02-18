using System.ComponentModel.DataAnnotations;

namespace VotingApp.Models;

public class VoteViewModel
{
    [Required(ErrorMessage = "Please select a candidate")]
    public string SelectedCandidate { get; set; } = string.Empty;
    
    public string[] AvailableCandidates { get; set; } = Array.Empty<string>();
}
