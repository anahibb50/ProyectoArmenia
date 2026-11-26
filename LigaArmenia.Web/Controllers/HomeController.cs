using System.Diagnostics;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaArmenia.Web.Controllers;

[Authorize]
public sealed class HomeController : Controller
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserSecurityStateService _securityStateService;

    public HomeController(
        ICurrentUserService currentUser,
        IUserSecurityStateService securityStateService)
    {
        _currentUser = currentUser;
        _securityStateService = securityStateService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (_currentUser.UserId.HasValue)
        {
            var state = await _securityStateService.GetStateAsync(_currentUser.UserId.Value, cancellationToken);
            ViewBag.SecurityState = state;
        }

        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
