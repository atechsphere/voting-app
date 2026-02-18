namespace VotingApp.Models;

public class HomeViewModel
{
    public Dictionary<string, int> VoteResults { get; set; } = new();
    public int TotalVotes { get; set; }
    public int TotalVoters { get; set; }
    public bool IsUserLoggedIn { get; set; }
    public bool HasUserVoted { get; set; }
}
