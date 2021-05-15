using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers.Web
{
    [Route("/skate3/webkit/PS3/English/i/Users")]
    public class UsersController : Controller
    {
        [HttpGet("Show/{userId}")]
        public IActionResult Show(string userId)
        {
            ViewData["Message"] = $"Users Show {userId}";
            return View();
        }
    }
}
