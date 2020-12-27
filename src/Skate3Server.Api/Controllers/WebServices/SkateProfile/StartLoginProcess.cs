using Skate3Server.Api.Controllers.WebServices.Common;

namespace Skate3Server.Api.Controllers.WebServices.SkateProfile
{
    public class StartLoginProcess
    {
        public PlatformType PlatformType { get; set; }
        public long UserId { get; set; }
        public string SessionKey { get; set; }
        public bool DeleteThumbs { get; set; }
        public int Difficulty { get; set; } //TODO: enum?
        public ulong TotalBoardSales { get; set; }
    }
}
