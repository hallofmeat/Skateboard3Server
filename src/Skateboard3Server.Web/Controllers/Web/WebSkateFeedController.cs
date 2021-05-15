using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers.Web
{
    [Route("/skate3/webkit/PS3/English/i/SkateFeed")]
    public class WebSkateFeedController : Controller
    {
        public string Index()
        {
            //TODO auth
            return "SkateFeed";
        }
    }
}
