namespace VotingApp.Models;

public class DashboardViewModel
{
    public bool HasVoted { get; set; }
    public string? VotedCandidate { get; set; }
    public DateTime? VotedAt { get; set; }
    public Dictionary<string, int> VoteResults { get; set; } = new();
    public int TotalVotes { get; set; }
    public VoteViewModel VoteForm { get; set; } = new();
}
