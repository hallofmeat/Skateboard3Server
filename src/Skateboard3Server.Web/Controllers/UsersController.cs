using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

//[Authorize]
[Route("/skate3/webkit/PS3/English/i/Users")]
public class UsersController : Controller
{
    [HttpGet("Show")]
    public IActionResult Show()
    {
        ViewData["Username"] = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        return View();
    }

    [HttpGet("Invitations")]
    public IActionResult Invitations()
    {
        return View();
    }
}