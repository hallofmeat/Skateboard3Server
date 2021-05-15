using System.Runtime.Serialization;
using Skateboard3Server.Web.WebServices.Common;

namespace Skateboard3Server.Web.WebServices.SkateProfile
{
    [DataContract]
    public class SetUserAchievements
    {
        [DataMember]
        public PlatformType PlatformId { get; set; }
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public string AchievementIds { get; set; }
    }
}