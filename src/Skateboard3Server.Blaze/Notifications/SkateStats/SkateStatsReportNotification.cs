using System.Collections.Generic;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.SkateStats
{
    [BlazeNotification(BlazeComponent.SkateStats, (ushort)SkateStatsNotification.StatsReport)]
    public class SkateStatsReportNotification : BlazeNotification
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
        public uint GameType { get; set; }

        [TdfField("PRCS")]
        public bool Prcs { get; set; } //TODO

        [TdfField("RPRT")]
        public Dictionary<uint, StatReport> StatsReport { get; set; } //key is USID
    }
}
