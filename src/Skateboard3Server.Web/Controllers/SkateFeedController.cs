using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

// TODO - [Authorize] along wih most other pages
[Route("/skate3/webkit/PS3/English/i/SkateFeed")]
public class SkateFeedController : Controller
{
    public IActionResult Index()
    {
        ViewData["Username"] = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        return View();
    }
}