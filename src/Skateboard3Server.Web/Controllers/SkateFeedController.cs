using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers
{
    [Route("/skate3/webkit/PS3/English/i/SkateFeed")]
    public class SkateFeedController : Controller
    {
        public IActionResult Index()
        {
            //TODO auth
            return View();
        }
    }
}
