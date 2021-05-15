using Skateboard3Server.Web.Models.Common;

namespace Skateboard3Server.Web.Models.SkateProfile
{
    public class SetUserDlc
    {
        public PlatformType PlatformId { get; set; }
        public long UserId { get; set; }
        public string OfferIds { get; set; }
    }
}