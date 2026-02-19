using Microsoft.EntityFrameworkCore;
using VotingApp.Data;
using VotingApp.Models;

namespace VotingApp.Services;

public interface IVotingService
{
    Task<bool> HasUserVotedAsync(string userId);
    Task<bool> CastVoteAsync(string userId, string candidate);
    Task<Dictionary<string, int>> GetVoteResultsAsync();
    Task<int> GetTotalVotesAsync();
    Task<Vote?> GetUserVoteAsync(string userId);
    Task<int> GetTotalVotersAsync();
}

public class VotingService : IVotingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VotingService> _logger;
    
    public VotingService(ApplicationDbContext context, ILogger<VotingService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<bool> HasUserVotedAsync(string userId)
    {
        return await _context.Votes.AnyAsync(v => v.UserId == userId);
    }
    
    public async Task<bool> CastVoteAsync(string userId, string candidate)
    {
        // Check if user has already voted
        if (await HasUserVotedAsync(userId))
        {
            _logger.LogWarning("User {UserId} attempted to vote multiple times", userId);
            return false;
        }
        
        // Validate candidate
        if (!VoteOptions.Candidates.Contains(candidate))
        {
            _logger.LogWarning("Invalid candidate selected: {Candidate}", candidate);
            return false;
        }
        
        try
        {
            var vote = new Vote
            {
                UserId = userId,
                Candidate = candidate,
                VotedAt = DateTime.UtcNow
            };
            
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Vote cast successfully by user {UserId} for {Candidate}", userId, candidate);
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error casting vote for user {UserId}", userId);
            return false;
        }
    }
    
    public async Task<Dictionary<string, int>> GetVoteResultsAsync()
    {
        var results = await _context.Votes
            .GroupBy(v => v.Candidate)
            .Select(g => new { Candidate = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Candidate, x => x.Count);
        
        // Ensure all candidates are in the results
        foreach (var candidate in VoteOptions.Candidates)
        {
            if (!results.ContainsKey(candidate))
            {
                results[candidate] = 0;
            }
        }
        
        return results;
    }
    
    public async Task<int> GetTotalVotesAsync()
    {
        return await _context.Votes.CountAsync();
    }
    
    public async Task<Vote?> GetUserVoteAsync(string userId)
    {
        return await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId);
    }
    
    public async Task<int> GetTotalVotersAsync()
    {
        return await _context.Users.CountAsync();
    }
}
