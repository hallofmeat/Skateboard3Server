using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeResponse(BlazeComponent.Authentication, (ushort)AuthenticationCommand.SessionData)]
    public class SessionDataResponse : BlazeResponse
    {
        [TdfField("BUID")]
        public uint BlazeId { get; set; }

        [TdfField("FRST")]
        public bool FirstLogin { get; set; }

        [TdfField("KEY")]
        public string SessionKey { get; set; }

        [TdfField("LLOG")]
        public long LastLoginTime { get; set; }

        [TdfField("MAIL")]
        public string Email { get; set; }

        [TdfField("PDTL")]
        public SessionDataProfile Profile { get; set; }

        [TdfField("UID")]
        public long AccountId { get; set; }
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
