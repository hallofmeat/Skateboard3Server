using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeResponse(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Login)]
    public class LoginResponse : IBlazeResponse
    {
        [TdfField("AGUP")]
        public bool Agup { get; set; } //TODO

        [TdfField("PRIV")]
        public string Priv { get; set; } //TODO

        [TdfField("SESS")]
        public LoginSession Session { get; set; }

        [TdfField("SPAM")]
        public bool Spam { get; set; } //TODO

        [TdfField("THST")]
        public string TermsHost { get; set; }

        [TdfField("TURI")]
        public string TermsUrl { get; set; }

    }

    public class LoginSession
    {
        [TdfField("BUID")]
        public uint BlazeId { get; set; }

        [TdfField("FRST")]
        public bool FirstLogin { get; set; }

        [TdfField("KEY")]
        public string BlazeKey { get; set; }

        [TdfField("LLOG")]
        public long LastLoginTime { get; set; }

        [TdfField("MAIL")]
        public string Email { get; set; }

        [TdfField("PDTL")]
        public LoginProfile Profile { get; set; }

        [TdfField("UID")]
        public long AccountId { get; set; }
    }

    public class LoginProfile
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
