using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.SkateStats.Messages;

[BlazeRequest(BlazeComponent.SkateStats, (ushort)SkateStatsCommand.UpdateStats)]
[UsedImplicitly]
public record SkateStatsRequest : BlazeRequestMessage, IRequest<SkateStatsResponse>
{
    [TdfField("FNSH")]
    public bool Finished { get; init; }

    [TdfField("GRID")]
    public uint Grid { get; init; } //TODO

    [TdfField("GTYP")]
    public uint GameType { get; init; }

    [TdfField("PRCS")]
    public bool Prcs { get; init; } //TODO

    [TdfField("RPRT")]
    public Dictionary<uint, StatReport> StatsReport { get; init; } //key is USID

}