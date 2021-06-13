using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers
{
    /// <summary>
    /// Not a real page but holds test pages
    /// </summary>
    [Route("/skate3/webkit/PS3/English/i/Test")]
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Inputs")]
        public IActionResult Inputs()
        {
            return View();
        }
    }
}
