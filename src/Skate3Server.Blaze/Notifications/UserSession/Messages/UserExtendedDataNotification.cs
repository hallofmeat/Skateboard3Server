using System.Collections.Generic;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Notifications.UserSession.Messages
{
    [BlazeNotification(BlazeComponent.UserSession, 0x1)]
    public class UserExtendedDataNotification
    {
        [TdfField("DATA")]
        public ExtendedData Data { get; set; }

        [TdfField("USID")]
        public uint UserSessionId { get; set; }
    }

    public class ExtendedData
    {
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, object> Address { get; set; } //TODO this can also be an address pair

        [TdfField("BPS")]
        public string Bps { get; set; } //TODO bits per second?

        [TdfField("CTY")]
        public string Cty { get; set; } //TODO city?

        [TdfField("DMAP")]
        public Dictionary<uint, int> Dmap { get; set; } //TODO destination map?

        [TdfField("HWFG")]
        public uint HardwareFlags { get; set; }

        [TdfField("QDAT")]
        public NetworkData NetworkData { get; set; }

        [TdfField("UATT")]
        public ulong Uatt { get; set; } //TODO
    }

    public class NetworkData
    {
        [TdfField("DBPS")]
        public ulong DownstreamBitsPerSecond { get; set; }

        [TdfField("NATT")]
        public int NatType { get; set; }

        [TdfField("UBPS")]
        public ulong UpstreamBitsPerSecond { get; set; }

    }
}
