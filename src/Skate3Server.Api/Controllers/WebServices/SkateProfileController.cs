using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Skate3Server.Api.Controllers.WebServices.SkateProfile;

namespace Skate3Server.Api.Controllers.WebServices
{
    [Route("/skate3/ws/SkateProfile.asmx")]
    [ApiController]
    [Produces("text/xml")]
    public class SkateProfileController : ControllerBase
    {
        [HttpPost("SetUserDLC")]
        [Consumes("application/x-www-form-urlencoded")]
        public BoolContainer SetUserDlc([FromForm] SetUserDLC data)
        {
            return new BoolContainer(true);
        }
    }

    public class BoolContainer
    {
        public BoolContainer()
        {
        }

        public BoolContainer(bool value)
        {
            Value = value;
        }
        
        [XmlElement(ElementName = "value")]
        public bool Value { get; set; }
    }
}
