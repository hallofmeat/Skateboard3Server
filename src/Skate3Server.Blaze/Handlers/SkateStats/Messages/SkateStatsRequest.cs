using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.SkateStats.Messages
{
    [BlazeRequest(BlazeComponent.SkateStats, (ushort)SkateStatsCommand.UpdateStats)]
    public class SkateStatsRequest : IRequest<SkateStatsResponse>, IBlazeRequest
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
