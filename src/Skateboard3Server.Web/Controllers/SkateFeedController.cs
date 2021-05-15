using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Web.WebServices.Common;
using Skateboard3Server.Web.WebServices.SkateFeed;

namespace Skateboard3Server.Web.Controllers
{
    [Route("/skate3/ws/SkateFeed.asmx")]
    [ApiController]
    [Produces("text/xml")]
    public class SkateFeedController : ControllerBase
    {
        [HttpPost("PlayerSignedIntoEANation")]
        [Consumes("application/x-www-form-urlencoded")]
        public IntegerContainer PlayerSignedIntoEaNation([FromForm] PlayerSignedIntoEaNation data)
        {
            return new IntegerContainer(0); //TODO: do this for real
        }
    }
}
