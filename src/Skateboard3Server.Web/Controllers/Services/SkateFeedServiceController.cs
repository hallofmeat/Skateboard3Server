using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Web.Models.Common;
using Skateboard3Server.Web.Models.SkateFeed;

namespace Skateboard3Server.Web.Controllers.Services
{
    [Route("/skate3/ws/SkateFeed.asmx")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("text/xml")]
    [ApiController]
    public class SkateFeedServiceController : ControllerBase
    {
        [HttpPost("PlayerSignedIntoEANation")]
        public IntegerContainer PlayerSignedIntoEaNation([FromForm] PlayerSignedIntoEaNation data)
        {
            return new IntegerContainer(0); //TODO: do this for real

        }
    }
}
