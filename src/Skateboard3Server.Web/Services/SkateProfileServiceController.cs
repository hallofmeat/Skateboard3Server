using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Web.Services.Models.Common;
using Skateboard3Server.Web.Services.Models.SkateProfile;

namespace Skateboard3Server.Web.Services
{
    [Route("/skate3/ws/SkateProfile.asmx")]

    [ApiController]
    public class SkateProfileServiceController : ControllerBase
    {

        [HttpPost("StartLoginProcess")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("text/xml")]
        public StartLoginProcessResponse StartLoginProcess([FromForm] StartLoginProcess data)
        {
            return new StartLoginProcessResponse
            {
                PrivacyFlagContainer = string.Empty, //TODO: forces a self-closing tag
                AwardedBoardSales = 0,
                TeamInfo = new TeamInfo //TODO
                {
                    TeamId = 0,
                    NumMembers = 0,
                }
            };
        }

        [HttpGet("GetSchema")]
        public IActionResult GetSchema(PlatformType platformId, long userId) //Note this is not authed on the real version
        {
            //TODO returns 404 if schema doesnt exist otherwise returns octet stream of schema
            var bytes = System.IO.File.ReadAllBytes("testuserschema.bin"); //gross
            return File(bytes, "application/octet-stream");
        }

        [HttpPost("UploadSchema")]
        [Consumes("multipart/form-data")]
        [Produces("text/xml")]
        public LongContainer UploadSchema([FromForm] UploadSchema data)
        {
            //TODO save schema to user
            return new LongContainer(1); //TODO no idea if this number is right
        }

        [HttpPost("UploadAIProfile")]
        [Consumes("multipart/form-data")]
        [Produces("text/xml")]
        public LongContainer UploadSchema([FromForm] UploadAiProfile data)
        {
            //TODO save AI profile?
            return new LongContainer(2); //TODO no idea if this number is right
        }

        [HttpPost("SetUserDLC")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("text/xml")]
        public BoolContainer SetUserDlc([FromForm] SetUserDlc data)
        {
            return new BoolContainer(true);
        }

        [HttpPost("SetUserAchievements")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("text/xml")]
        public IntegerContainer SetUserAchievements([FromForm] SetUserAchievements data)
        {
            return new IntegerContainer(0);
        }
    }
}
