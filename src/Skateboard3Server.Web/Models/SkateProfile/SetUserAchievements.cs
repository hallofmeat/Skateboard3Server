using Skateboard3Server.Web.Models.Common;

namespace Skateboard3Server.Web.Models.SkateProfile
{
    public class SetUserAchievements
    {
        public PlatformType PlatformId { get; set; }
        public long UserId { get; set; }
        public string AchievementIds { get; set; }
    }
}