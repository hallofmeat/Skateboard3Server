using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.SkateStats.Messages;

[BlazeRequest(BlazeComponent.SkateStats, (ushort)SkateStatsCommand.UpdateStats)]
[UsedImplicitly]
public record SkateStatsRequest : BlazeRequestMessage, IRequest<SkateStatsResponse>
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