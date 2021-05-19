using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers.Web
{
    /// <summary>
    /// These are the entrypoints for the web browser from here auth is done, cookies set, and redirected
    /// </summary>
    [Route("/skate3/webkit/PS3/English/i/Sessions")]
    public class SessionsController : ControllerBase
    {
        [HttpGet("GameLogin/{blazeId}/bootflow")]
        public IActionResult BootFlow(string blazeId)
        {
            //TODO do logic here to auth and set cookies and junk
            return Redirect("/skate3/webkit/PS3/English/i/SkateFeed");
        }

        [HttpGet("GameLogin/{blazeId}/skatefeed")]
        public IActionResult SkateFeed(string blazeId)
        {
            //TODO do logic here to auth and set cookies and junk
            return Redirect($"/skate3/webkit/PS3/English/i/SkateFeed");
        }

        [HttpGet("GameLogin/{blazeId}/playerprofile")]
        public IActionResult PlayerProfile(string blazeId)
        {
            //TODO do logic here to auth and set cookies and junk
            return Redirect($"/skate3/webkit/PS3/English/i/Users/Show/{12345}"); //TODO lookup real user
        }
    }
}
