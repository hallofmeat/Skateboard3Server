using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

//[Authorize]
[Route("/skate3/webkit/PS3/English/i/Leaderboards")]
public class LeaderboardsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Route("Solo")]
    public IActionResult Solo()
    {
        return View();
    }

    [Route("Solo/Overall")]
    public IActionResult SoloOverall()
    {
        return View();
    }

    [Route("Solo/Overall/Ranked")]
    public IActionResult SoloOverallRanked()
    {
        return View();
    }
}