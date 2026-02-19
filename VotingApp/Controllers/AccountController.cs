using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VotingApp.Models;

namespace VotingApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await _userManager.CreateAsync(user, model.Password);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} registered successfully", model.Email);
            
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Dashboard");
        }
        
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }
        
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, 
            model.Password, 
            model.RememberMe, 
            lockoutOnFailure: true);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in", model.Email);
            return RedirectToLocal(returnUrl);
        }
        
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account locked out", model.Email);
            ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
            return View(model);
        }
        
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
    
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Dashboard");
    }
}
