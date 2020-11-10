using System.Collections.Generic;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Notifications.UserSession.Messages
{
    [BlazeNotification(BlazeComponent.SkateStats, (ushort)SkateStatsCommand.StatsReport)]
    public class SkateStatsNotification
    {
        [TdfField("EROR")]
        public int Error { get; set; }

        [TdfField("FNL")]
        public bool Final { get; set; }

        [TdfField("GRID")]
        public uint Grid { get; set; } //TODO

        [TdfField("RPRT")]
        public RequestReport RequestReport { get; set; } //TODO
    }

    public class RequestReport
    {
        [TdfField("FNSH")]
        public bool Finished { get; set; }

        [TdfField("GRID")]
        public uint Grid { get; set; } //TODO

        [TdfField("GTYP")]
        public uint GType { get; set; } //TODO

        [TdfField("PRCS")]
        public bool Prcs { get; set; } //TODO

        [TdfField("RPRT")]
        public Dictionary<uint, StatReport> StatsReport { get; set; } //key is USID
    }
}
