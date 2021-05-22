using Skateboard3Server.Web.Services.Models.Common;

namespace Skateboard3Server.Web.Services.Models.SkateProfile
{
    public class UploadAiProfile
    {
        public PlatformType PlatformId { get; set; }
        public long UserId { get; set; }
        public int TypeId { get; set; } //TODO: enum?
        public byte[] AiProfile { get; set; }


    }
}