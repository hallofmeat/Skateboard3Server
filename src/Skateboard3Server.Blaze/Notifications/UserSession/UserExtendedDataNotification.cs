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
        public string Bps { get; set; } //TODO bits per second?

        [TdfField("CTY")]
        public string Cty { get; set; } //TODO city?

        [TdfField("DMAP")]
        public Dictionary<uint, int> Dmap { get; set; } //TODO destination map?

        [TdfField("HWFG")]
        public uint HardwareFlags { get; set; }

        [TdfField("PSLM")]
        public List<int> Pslm { get; set; } //TODO: enum? 

        [TdfField("QDAT")]
        public QosNetworkData NetworkData { get; set; }

        [TdfField("ULST")]
        public List<ulong> Ulst { get; set; }

        [TdfField("UATT")]
        public ulong Uatt { get; set; } //TODO
    }
}
