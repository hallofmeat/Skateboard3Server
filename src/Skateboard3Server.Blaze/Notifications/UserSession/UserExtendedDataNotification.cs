using System.Collections.Generic;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.UserSession
{
    [BlazeNotification(BlazeComponent.UserSession, (ushort)UserSessionNotification.UserExtendedData)]
    public class UserExtendedDataNotification : BlazeNotification
    {
        [TdfField("DATA")]
        public ExtendedData Data { get; set; }

        [TdfField("USID")]
        public uint UserId { get; set; }
    }

    public class ExtendedData
    {
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, NetworkAddress> Address { get; set; }

        [TdfField("BPS")]
        public string BandwidthServer { get; set; } //maps to qos bandwidth server name

        [TdfField("CTY")]
        public string Cty { get; set; } //TODO city?

        [TdfField("DMAP")]
        public Dictionary<uint, int> Dmap { get; set; } //TODO data map?

        [TdfField("HWFG")]
        public uint HardwareFlags { get; set; }

        [TdfField("PSLM")]
        public List<int> Pings { get; set; } //One entry for each qos ping server

        [TdfField("QDAT")]
        public QosNetworkData NetworkData { get; set; }

        [TdfField("ULST")]
        public List<ulong> Ulst { get; set; } //TODO: session list? (short 04, short 01, int gameid)

        [TdfField("UATT")]
        public ulong Uatt { get; set; } //TODO
    }
}
