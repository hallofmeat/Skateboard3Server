using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

[Route("/skate3/webkit/PS3/English/i/Leaderboards")]
public class LeaderboardsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}