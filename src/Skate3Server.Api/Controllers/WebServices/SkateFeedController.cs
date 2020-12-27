using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
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
            return new IntegerContainer(1); //TODO: return 0 and do flow
        }
    }

    public class IntegerContainer
    {
        public IntegerContainer()
        {
        }

        public IntegerContainer(int value)
        {
            Value = value;
        }
        
        [XmlElement(ElementName = "value")]
        public int Value { get; set; }
    }
}
