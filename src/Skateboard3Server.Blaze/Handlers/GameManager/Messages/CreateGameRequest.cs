using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.CreateGame)]
[UsedImplicitly]
public record CreateGameRequest : BlazeRequestMessage, IRequest<CreateGameResponse>
{
    [TdfField("ATTR")]
    public Dictionary<string, string> Attributes { get; init; }

    [TdfField("BTPL")]
    public ulong Btpl { get; init; } //TODO

    [TdfField("GCTR")]
    public string Gctr { get; init; } //TODO

    [TdfField("GNAM")]
    public string GameName { get; init; }

    [TdfField("GSET")]
    public uint GameSettings { get; init; }

    [TdfField("GURL")]
    public string Gurl { get; init; } //TODO gameUrl?

    [TdfField("GVER")]
    public int Gver { get; init; } //TODO: enum gameVersion?

    [TdfField("HNET")]
    public List<KeyValuePair<NetworkAddressType, PairNetworkAddress>> HostNetwork { get; init; }

    [TdfField("IGNO")]
    public bool Ignore { get; init; }

    [TdfField("NTOP")]
    public NetworkTopology NetworkTopology { get; init; }

    [TdfField("PATT")]
    public Dictionary<string, string> PlayerAttributes { get; init; }

    [TdfField("PCAP")]
    public List<ushort> PlayerCapacity { get; init; }

    [TdfField("PGID")]
    public string Pgid { get; init; } //TODO

    [TdfField("PGSC")]
    public byte[] Pgsc { get; init; } //TODO

    [TdfField("PMAX")]
    public ushort PlayerMax { get; init; }

    [TdfField("QCAP")]
    public ushort QueueCapacity { get; init; } //TODO queue capacity?

    [TdfField("RGID")]
    public uint Rgid { get; init; } //TODO

    [TdfField("SLOT")]
    public int Slot { get; init; } //TODO: enum

    [TdfField("TEAM")]
    public ushort Team { get; init; } //TODO team capacity?

    [TdfField("VOIP")]
    public VoipTopology VoipTopology { get; init; }

    [TdfField("VSTR")]
    public string VersionString { get; init; }
}