using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Web.Services.Models.Common;
using Skateboard3Server.Web.Services.Models.SkateFeed;

namespace Skateboard3Server.Web.Services;

[Route("/skate3/ws/SkateFeed.asmx")]
[ApiController]
public class SkateFeedServiceController : ControllerBase
{
    [HttpPost("PlayerSignedIntoEANation")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("text/xml")]
    public IntegerContainer PlayerSignedIntoEaNation([FromForm] PlayerSignedIntoEaNation data)
    {
        return new IntegerContainer(0); //TODO: do this for real

    }
}