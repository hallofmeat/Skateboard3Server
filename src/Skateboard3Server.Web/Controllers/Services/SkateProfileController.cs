using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Web.Models.Common;
using Skateboard3Server.Web.Models.SkateProfile;

namespace Skateboard3Server.Web.Controllers.Services
{
    [Route("/skate3/ws/SkateProfile.asmx")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("text/xml")]
    [ApiController]
    public class SkateProfileController : ControllerBase
    {

        [HttpPost("StartLoginProcess")]
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
        public BoolContainer SetUserDlc([FromForm] SetUserDlc data)
        {
            return new BoolContainer(true);
        }

        [HttpPost("SetUserAchievements")]
        public IntegerContainer SetUserAchievements([FromForm] SetUserAchievements data)
        {
            return new IntegerContainer(0);
        }
    }
}
