using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Notifications.UserSession.Messages
{
    [BlazeNotification(BlazeComponent.UserSession, 0x2)]
    public class UserAddedNotification
    {
        [TdfField("AID")]
        public long AId { get; set; } //TODO

        [TdfField("ALOC")]
        public ulong ALoc { get; set; } //TODO

        [TdfField("EXBB")]
        public byte[] ExternalBlob { get; set; }

        [TdfField("EXID")]
        public ulong ExternalId { get; set; }

        [TdfField("ID")]
        public uint Id { get; set; }

        [TdfField("NAME")]
        public string Username { get; set; }

        [TdfField("ONLN")]
        public bool Online { get; set; }

        [TdfField("PID")]
        public bool ProfileId { get; set; }

    }
}
