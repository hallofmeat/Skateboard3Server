using System.Collections.Generic;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.CreateGame)]
    public class CreateGameRequest : BlazeRequest, IRequest<CreateGameResponse>
    {
        [TdfField("ATTR")]
        public Dictionary<string, string> Attributes { get; set; }

        [TdfField("BTPL")]
        public ulong Btpl { get; set; } //TODO

        [TdfField("GCTR")]
        public string Gctr { get; set; } //TODO

        [TdfField("GNAM")]
        public string GameName { get; set; }

        [TdfField("GSET")]
        public uint GameSettings { get; set; }

        [TdfField("GURL")]
        public string Gurl { get; set; } //TODO gameUrl?

        [TdfField("GVER")]
        public int Gver { get; set; } //TODO: enum gameVersion?

        [TdfField("HNET")]
        public List<KeyValuePair<NetworkAddressType, PairNetworkAddress>> HostNetwork { get; set; }

        [TdfField("IGNO")]
        public bool Ignore { get; set; }

        [TdfField("NTOP")]
        public NetworkTopology NetworkTopology { get; set; }

        [TdfField("PATT")]
        public Dictionary<string, string> PlayerAttributes { get; set; }

        [TdfField("PCAP")]
        public List<ushort> PlayerCapacity { get; set; }

        [TdfField("PGID")]
        public string Pgid { get; set; } //TODO

        [TdfField("PGSC")]
        public byte[] Pgsc { get; set; } //TODO

        [TdfField("PMAX")]
        public ushort PlayerMax { get; set; }

        [TdfField("QCAP")]
        public ushort QueueCapacity { get; set; } //TODO queue capacity?

        [TdfField("RGID")]
        public uint Rgid { get; set; } //TODO

        [TdfField("SLOT")]
        public int Slot { get; set; } //TODO: enum

        [TdfField("TEAM")]
        public ushort Team { get; set; } //TODO team capacity?

        [TdfField("VOIP")]
        public VoipTopology VoipTopology { get; set; }

        [TdfField("VSTR")]
        public string VersionString { get; set; }
    }
}
