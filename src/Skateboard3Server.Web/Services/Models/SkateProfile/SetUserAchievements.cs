using Skateboard3Server.Web.Services.Models.Common;

namespace Skateboard3Server.Web.Services.Models.SkateProfile
{
    public class SetUserAchievements
    {
        public PlatformType PlatformId { get; set; }
        public uint UserId { get; set; }
        public string AchievementIds { get; set; }
    }
}