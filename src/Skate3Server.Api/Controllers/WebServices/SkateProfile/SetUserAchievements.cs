using Skate3Server.Api.Controllers.WebServices.Common;

namespace Skate3Server.Api.Controllers.WebServices.SkateProfile
{
    public class SetUserAchievements
    {
        public PlatformType PlatformId { get; set; }
        public long UserId { get; set; }
        public string AchievementIds { get; set; }
    }
}