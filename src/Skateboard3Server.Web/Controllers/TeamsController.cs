using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers
{
    [Route("/skate3/webkit/PS3/English/i/Teams")]
    public class TeamsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("CreateTeam")]
        public IActionResult CreateTeam()
        {
            return View();
        }
    }
}
