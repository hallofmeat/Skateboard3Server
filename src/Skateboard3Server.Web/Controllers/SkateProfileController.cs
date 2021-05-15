using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Web.WebServices.Common;
using Skateboard3Server.Web.WebServices.SkateProfile;

namespace Skateboard3Server.Web.Controllers
{
    [Route("/skate3/ws/SkateProfile.asmx")]
    [ApiController]
    [Produces("text/xml")]
    public class SkateProfileController : ControllerBase
    {
        [HttpPost("StartLoginProcess")]
        [Consumes("application/x-www-form-urlencoded")]
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

        [HttpPost("SetUserDLC")]
        [Consumes("application/x-www-form-urlencoded")]
        public BoolContainer SetUserDlc([FromForm] SetUserDlc data)
        {
            return new BoolContainer(true);
        }

        [HttpPost("SetUserAchievements")]
        [Consumes("application/x-www-form-urlencoded")]
        public IntegerContainer SetUserAchievements([FromForm] SetUserAchievements data)
        {
            return new IntegerContainer(0);
        }
    }
}
