using System.Collections.Generic;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Notifications.SkateStats
{
    [BlazeNotification(BlazeComponent.SkateStats, (ushort)SkateStatsNotification.StatsReport)]
    public class SkateStatsReportNotification : IBlazeNotification
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
