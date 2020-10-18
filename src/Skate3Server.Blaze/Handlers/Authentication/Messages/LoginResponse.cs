using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeResponse(BlazeComponent.Authentication, 0xC8)]
    public class LoginResponse : BlazeResponse
    {
        [TdfField("AGUP")]
        public bool Agup { get; set; } //TODO

        [TdfField("PRIV")]
        public string Priv { get; set; } //TODO

        [TdfField("SESS")]
        public Session Session { get; set; }

        [TdfField("SPAM")]
        public bool Spam { get; set; } //TODO

        [TdfField("THST")]
        public string TermsHost { get; set; }

        [TdfField("TURI")]
        public string TermsUrl { get; set; }
    }

    public class Session
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
        public Profile Profile { get; set; }

        [TdfField("UID")]
        public long UserId { get; set; }
    }

    public class Profile
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
