using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.StartMatchmaking)]
    public class StartMatchmakingRequest : IRequest<StartMatchmakingResponse>, IBlazeRequest
    {
        [TdfField("ATTR")]
        public Dictionary<string, string> Attributes { get; set; }

        [TdfField("BTPL")]
        public ulong Btpl { get; set; } //TODO

        [TdfField("CRIT")]
        public MatchmakingCriteria MatchmakingCriteria { get; set; }

        [TdfField("DUR")]
        public uint Dur { get; set; } //TODO

        [TdfField("GNAM")]
        public string Gnam { get; set; } //TODO

        [TdfField("GSET")]
        public uint Gset { get; set; } //TODO

        [TdfField("GVER")]
        public string Gver { get; set; } //TODO

        [TdfField("IGNO")]
        public bool Igno { get; set; } //TODO

        [TdfField("MODE")]
        public uint Mode { get; set; }

        [TdfField("NTOP")]
        public int NetworkTopology { get; set; } //TODO enum

        [TdfField("PATT")]
        public Dictionary<string, string> Patt { get; set; } //TODO dlc mask?

        [TdfField("PMAX")]
        public ushort Pmax { get; set; } //TODO peer max?

        [TdfField("PNET")]
        public KeyValuePair<NetworkAddressType, PairNetworkAddress> PeerNetwork { get; set; }

        [TdfField("QCAP")]
        public ushort Qcap { get; set; } //TODO

        [TdfField("VOIP")]
        public int Voip { get; set; } //TODO enum
    }

    public class MatchmakingCriteria
    {
        [TdfField("CUST")]
        public CustomCriteria Custom { get; set; }

        [TdfField("DNF")]
        public DnfCriteria Dnf { get; set; } //TODO

        [TdfField("GEO")]
        public ThldCriteria Geo { get; set; } //TODO

        [TdfField("GVER")]
        public int Gver { get; set; } //TODO enum?

        [TdfField("NAT")]
        public ThldCriteria Nat { get; set; } //TODO

        [TdfField("PSR")]
        public ThldCriteria Psr { get; set; } //TODO

        [TdfField("RANK")]
        public ThldValueCriteria Rank { get; set; } //TODO

        [TdfField("RLIST")]
        public List<MatchmakingRule> Rules { get; set; }

        [TdfField("RSZR")]
        public RszrCriteria Rszr { get; set; } //TODO

        [TdfField("SIZE")]
        public SizeCriteria Size { get; set; }

        [TdfField("TEAM")]
        public TeamCriteria Team { get; set; }

        [TdfField("VIAB")]
        public ThldCriteria Viab { get; set; } //TODO
    }



    public class CustomCriteria
    {
        [TdfField("EXPL")]
        public CustomExpl Expl { get; set; }
    }

    public class CustomExpl //TODO better name
    {
        [TdfField("MASK")]
        public ulong Mask { get; set; } //TODO

        [TdfField("THLD")]
        public string Thld { get; set; } //TODO
    }

    public class DnfCriteria
    {
        [TdfField("DNF")]
        public int Dnf { get; set; } //TODO enum
    }

    public class ThldCriteria //TODO better name
    {
        [TdfField("THLD")]
        public string Thld { get; set; } //TODO
    }

    public class ThldValueCriteria //TODO better name
    {
        [TdfField("THLD")]
        public string Thld { get; set; } //TODO

        [TdfField("VALU")]
        public int Value { get; set; } //TODO enum?
    }

    public class MatchmakingRule
    {
        [TdfField("Name")]
        public string Name { get; set; }

        [TdfField("THLD")]
        public string Thld { get; set; } //TODO

        [TdfField("VALU")]
        public List<string> Values { get; set; }
    }

    public class RszrCriteria //TODO better name
    {
        [TdfField("PCAP")]
        public ushort Pcap { get; set; } //TODO

        [TdfField("PMIN")]
        public ushort Pmin { get; set; } //TODO
    }

    public class SizeCriteria
    {
        [TdfField("ISSG")]
        public byte Issg { get; set; } //TODO

        [TdfField("PCAP")]
        public ushort Pcap { get; set; } //TODO

        [TdfField("PCNT")]
        public ushort Pcnt { get; set; } //TODO

        [TdfField("PMIN")]
        public ushort Pmin { get; set; } //TODO

        [TdfField("THLD")]
        public string Thld { get; set; } //TODO
    }

    public class TeamCriteria
    {
        [TdfField("SDIF")]
        public ushort Sdif { get; set; } //TODO

        [TdfField("THLD")]
        public string Thld { get; set; } //TODO

        [TdfField("TID")]
        public ushort Tid { get; set; } //TODO

    }

}
