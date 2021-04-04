using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Api.Controllers.WebServices.Common;
using Skateboard3Server.Api.Controllers.WebServices.SkateFeed;

namespace Skateboard3Server.Api.Controllers.WebServices
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
