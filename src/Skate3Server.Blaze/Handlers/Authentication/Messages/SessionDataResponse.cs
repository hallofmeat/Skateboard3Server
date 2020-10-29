using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeResponse(BlazeComponent.Authentication, 0xE6)]
    public class SessionDataResponse : BlazeResponse
    {
        [TdfField("BUID")]
        public uint BlazeId { get; set; }

        [TdfField("FRST")]
        public bool FirstLogin { get; set; }

        [TdfField("KEY")]
        public string Key { get; set; }

        [TdfField("LLOG")]
        public long LastLoginTime { get; set; }

        [TdfField("MAIL")]
        public string Email { get; set; }

        [TdfField("PDTL")]
        public SessionDataProfile Profile { get; set; }

        [TdfField("UID")]
        public long UserId { get; set; }
    }

    public class SessionDataProfile
    {
        [TdfField("DSNM")]
        public string DisplayName { get; set; }

        [TdfField("LAST")]
        public uint LastUsed { get; set; }

        [TdfField("PID")]
        public long ProfileId { get; set; }

        [TdfField("XREF")]
        public ulong ExternalProfileId { get; set; }

        [TdfField("XTYP")]
        public ExternalProfileType ExternalProfileType { get; set; }
    }
}
