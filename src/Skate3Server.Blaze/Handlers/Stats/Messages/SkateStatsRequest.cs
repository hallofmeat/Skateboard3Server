using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Stats.Messages
{
    [BlazeRequest(BlazeComponent.Stats, 0x2)]
    public class SkateStatsRequest : IRequest<SkateStatsResponse>
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
