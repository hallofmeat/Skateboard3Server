using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.StartMatchmaking)]
[UsedImplicitly]
public record StartMatchmakingRequest : BlazeRequestMessage, IRequest<StartMatchmakingResponse>
{
    [TdfField("ATTR")]
    public Dictionary<string, string> Attributes { get; init; }

    [TdfField("BTPL")]
    public ulong Btpl { get; init; } //TODO

    [TdfField("CRIT")]
    public MatchmakingCriteria MatchmakingCriteria { get; init; }

    [TdfField("DUR")]
    public uint Dur { get; init; } //TODO duration?

    [TdfField("GNAM")]
    public string GameName { get; init; }

    [TdfField("GSET")]
    public uint GameSettings { get; init; }

    [TdfField("GVER")]
    public string Gver { get; init; } //TODO gameVersion?

    [TdfField("IGNO")]
    public bool Igno { get; init; } //TODO ignore?

    [TdfField("MODE")]
    public uint Mode { get; init; }

    [TdfField("NTOP")]
    public int NetworkTopology { get; init; } //TODO enum

    [TdfField("PATT")]
    public Dictionary<string, string> PlayerAttributes { get; init; }

    [TdfField("PMAX")]
    public ushort PlayerMax { get; init; }

    [TdfField("PNET")]
    public KeyValuePair<NetworkAddressType, PairNetworkAddress> PlayerNetwork { get; init; }

    [TdfField("QCAP")]
    public ushort Qcap { get; init; } //TODO queue capacity?

    [TdfField("VOIP")]
    public int Voip { get; init; } //TODO enum voipTopology?
}

public class MatchmakingCriteria
{
    [TdfField("CUST")]
    public CustomCriteria Custom { get; init; }

    [TdfField("DNF")]
    public DnfCriteria Dnf { get; init; } //TODO

    [TdfField("GEO")]
    public ThldCriteria Geo { get; init; } //TODO

    [TdfField("GVER")]
    public int Gver { get; init; } //TODO enum? gameVersion?

    [TdfField("NAT")]
    public ThldCriteria Nat { get; init; } //TODO

    [TdfField("PSR")]
    public ThldCriteria Psr { get; init; } //TODO

    [TdfField("RANK")]
    public ThldValueCriteria Rank { get; init; } //TODO

    [TdfField("RLST")]
    public List<MatchmakingRule> Rules { get; init; }

    [TdfField("RSZR")]
    public RszrCriteria Rszr { get; init; } //TODO

    [TdfField("SIZE")]
    public SizeCriteria Size { get; init; }

    [TdfField("TEAM")]
    public TeamCriteria Team { get; init; }

    [TdfField("VIAB")]
    public ThldCriteria Viab { get; init; } //TODO
}

public class CustomCriteria
{
    [TdfField("EXPL")]
    public CustomExpl Expl { get; init; }
}

public class CustomExpl //TODO better name
{
    [TdfField("MASK")]
    public ulong Mask { get; init; } //TODO

    [TdfField("THLD")]
    public string Thld { get; init; } //TODO
}

public class DnfCriteria
{
    [TdfField("DNF")]
    public int Dnf { get; init; } //TODO enum
}

public class ThldCriteria //TODO better name, threshold?
{
    [TdfField("THLD")]
    public string Thld { get; init; } //TODO
}

public class ThldValueCriteria //TODO better name, threshold?
{
    [TdfField("THLD")]
    public string Thld { get; init; } //TODO

    [TdfField("VALU")]
    public int Value { get; init; } //TODO enum?
}

public class MatchmakingRule
{
    [TdfField("NAME")]
    public string Name { get; init; }

    [TdfField("THLD")]
    public string Thld { get; init; } //TODO

    [TdfField("VALU")]
    public List<string> Values { get; init; }
}

public class RszrCriteria //TODO better name
{
    [TdfField("PCAP")]
    public ushort Pcap { get; init; } //TODO

    [TdfField("PMIN")]
    public ushort Pmin { get; init; } //TODO
}

public class SizeCriteria
{
    [TdfField("ISSG")]
    public byte Issg { get; init; } //TODO

    [TdfField("PCAP")]
    public ushort Pcap { get; init; } //TODO

    [TdfField("PCNT")]
    public ushort Pcnt { get; init; } //TODO

    [TdfField("PMIN")]
    public ushort Pmin { get; init; } //TODO

    [TdfField("THLD")]
    public string Thld { get; init; } //TODO
}

public class TeamCriteria
{
    [TdfField("SDIF")]
    public ushort Sdif { get; init; } //TODO

    [TdfField("THLD")]
    public string Thld { get; init; } //TODO

    [TdfField("TID")]
    public ushort Tid { get; init; } //TODO teamid?

}