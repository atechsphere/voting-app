using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VotingApp.Models;
using VotingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace VotingApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IVotingService _votingService;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public HomeController(
        ILogger<HomeController> logger, 
        IVotingService votingService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _votingService = votingService;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            VoteResults = await _votingService.GetVoteResultsAsync(),
            TotalVotes = await _votingService.GetTotalVotesAsync(),
            TotalVoters = await _votingService.GetTotalVotersAsync(),
            IsUserLoggedIn = User.Identity?.IsAuthenticated ?? false
        };
        
        if (model.IsUserLoggedIn)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.HasUserVoted = await _votingService.HasUserVotedAsync(user.Id);
            }
        }
        
        return View(model);
    }
    
    public IActionResult Privacy()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}
