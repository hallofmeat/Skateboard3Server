using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.ResetServer)]
    public class ResetServerRequest : IRequest<ResetServerResponse>, IBlazeRequest
    {
        [TdfField("ATTR")]
        public Dictionary<string, string> Attributes { get; set; }

        [TdfField("BTPL")]
        public ulong Btpl { get; set; } //TODO

        [TdfField("GCTR")]
        public string Gctr { get; set; } //TODO

        [TdfField("GNAM")]
        public string Gnam { get; set; } //TODO

        [TdfField("GSET")]
        public uint Gset { get; set; } //TODO

        [TdfField("GURL")]
        public string Gurl { get; set; } //TODO

        [TdfField("GVER")]
        public int Gver { get; set; } //TODO: enum

        [TdfField("HNET")]
        public List<KeyValuePair<NetworkAddressType, PairNetworkAddress>> Hnet { get; set; }

        [TdfField("IGNO")]
        public bool Igno { get; set; } //TODO

        [TdfField("NTOP")]
        public int NetworkTopology { get; set; } //TODO enum

        [TdfField("PATT")]
        public Dictionary<string, string> PlayerAttributes { get; set; }

        [TdfField("PCAP")]
        public List<ushort> Pcap { get; set; } //TODO

        [TdfField("PGID")]
        public string Pgid { get; set; } //TODO

        [TdfField("PGSC")]
        public byte[] Pgsc { get; set; } //TODO

        [TdfField("PMAX")]
        public ushort Pmax { get; set; } //TODO peer max?

        [TdfField("QCAP")]
        public ushort Qcap { get; set; } //TODO

        [TdfField("RGID")]
        public uint Rgid { get; set; } //TODO

        [TdfField("SLOT")]
        public int Slot { get; set; } //TODO: enum

        [TdfField("TEAM")]
        public ushort Team { get; set; } //TODO

        [TdfField("VOIP")]
        public int Voip { get; set; } //TODO enum

        [TdfField("VSTR")]
        public string VersionString { get; set; }
    }
}
