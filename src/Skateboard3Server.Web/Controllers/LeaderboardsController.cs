using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

public class LeaderboardsController : Controller
{
    [Route("/skate3/webkit/PS3/English/i/Leaderboards")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/skate3/webkit/PS3/English/i/Leaderboards/Solo")]
    public IActionResult Solo()
    {
        return View();
    }

    [Route("/skate3/webkit/PS3/English/i/Leaderboards/Solo/Overall")]
    public IActionResult SoloOverall()
    {
        return View();
    }
}