using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    public class LoginResponse
    {
        [TdfField("AGUP")]
        public sbyte Agup { get; set; } //TODO

        [TdfField("PRIV")]
        public string Priv { get; set; } //TODO

        [TdfField("SESS")]
        public Session Session { get; set; }

        [TdfField("SPAM")]
        public sbyte Spam { get; set; }

        [TdfField("THST")]
        public string Thst { get; set; } //TODO

        [TdfField("TURI")]
        public string Turi { get; set; } //TODO
    }

    public class Session
    {
        [TdfField("BUID")]
        public uint BUserId { get; set; } //TODO: not the same as UID???

        [TdfField("FRST")]
        public byte Frst { get; set; } //TODO

        [TdfField("KEY")]
        public string Key { get; set; }

        [TdfField("LLOG")]
        public long LastLogin { get; set; }

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
        public string Username { get; set; }

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
