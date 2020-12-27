using Microsoft.AspNetCore.Mvc;
using Skate3Server.Api.Controllers.WebServices.Common;
using Skate3Server.Api.Controllers.WebServices.SkateFeed;

namespace Skate3Server.Api.Controllers.WebServices
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
