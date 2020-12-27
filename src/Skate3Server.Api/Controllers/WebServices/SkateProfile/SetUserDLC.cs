namespace Skate3Server.Api.Controllers.WebServices.SkateProfile
{
    public class SetUserDLC
    {
        public PlatformType PlatformType { get; set; }
        public long UserId { get; set; }
        public string OfferIds { get; set; }
    }
}