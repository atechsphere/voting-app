using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VotingApp.Models;
using VotingApp.Services;

namespace VotingApp.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IVotingService _votingService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DashboardController> _logger;
    
    public DashboardController(
        IVotingService votingService, 
        UserManager<ApplicationUser> userManager,
        ILogger<DashboardController> logger)
    {
        _votingService = votingService;
        _userManager = userManager;
        _logger = logger;
    }
    
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var userId = user.Id;
        var hasVoted = await _votingService.HasUserVotedAsync(userId);
        var userVote = await _votingService.GetUserVoteAsync(userId);
        
        var model = new DashboardViewModel
        {
            HasVoted = hasVoted,
            VotedCandidate = userVote?.Candidate,
            VotedAt = userVote?.VotedAt,
            VoteResults = await _votingService.GetVoteResultsAsync(),
            TotalVotes = await _votingService.GetTotalVotesAsync(),
            VoteForm = new VoteViewModel
            {
                AvailableCandidates = VoteOptions.Candidates
            }
        };
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(DashboardViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var userId = user.Id;
        
        // Check if user has already voted
        if (await _votingService.HasUserVotedAsync(userId))
        {
            TempData["Error"] = "You have already voted. Each user can only vote once.";
            return RedirectToAction(nameof(Index));
        }
        
        // Validate candidate selection
        if (string.IsNullOrEmpty(model.VoteForm.SelectedCandidate))
        {
            TempData["Error"] = "Please select a candidate.";
            return RedirectToAction(nameof(Index));
        }
        
        // Cast the vote
        var success = await _votingService.CastVoteAsync(userId, model.VoteForm.SelectedCandidate);
        
        if (success)
        {
            _logger.LogInformation("User {UserId} successfully voted for {Candidate}", 
                userId, model.VoteForm.SelectedCandidate);
            TempData["Success"] = "Your vote has been recorded successfully!";
        }
        else
        {
            TempData["Error"] = "Unable to record your vote. Please try again.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}
