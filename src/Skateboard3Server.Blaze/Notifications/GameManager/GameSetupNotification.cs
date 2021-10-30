using System.Collections.Generic;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager
{
    [BlazeNotification(BlazeComponent.GameManager, (ushort)GameManagerNotification.GameSetup)]
    public class GameSetupNotification : BlazeNotification
    {
        [TdfField("ERR")]
        public uint Error { get; set; }

        [TdfField("GAME")]
        public GameData GameData { get; set; }

        [TdfField("MMID")]
        public uint MatchmakingId { get; set; }

        [TdfField("PROS")]
        public List<PlayerData> Players { get; set; }
    }

    public class GameData
    {
        [TdfField("ADMN")]
        public List<uint> Admins { get; set; }

        [TdfField("ATTR")]
        public Dictionary<string, string> Attributes { get; set; }

        [TdfField("CAP")]
        public List<ushort> Cap { get; set; } //TODO capacity?

        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("GNAM")]
        public string GameName { get; set; }

        [TdfField("GPVH")]
        public ulong Gpvh { get; set; } //TODO

        [TdfField("GSET")]
        public uint GameSettings { get; set; }

        [TdfField("GSID")]
        public uint Gsid { get; set; } //TODO gameSessionId?

        [TdfField("GSTA")]
        public GameState GameState { get; set; }

        [TdfField("GVER")]
        public int Gver { get; set; } //TODO: enum gameVersion?

        [TdfField("HNET")]
        public List<KeyValuePair<NetworkAddressType, PairNetworkAddress>> HostNetwork { get; set; }

        [TdfField("HSES")]
        public uint Hses { get; set; } //TODO hostSession?

        [TdfField("IGNO")]
        public bool Ignore { get; set; }

        [TdfField("MATR")]
        public Dictionary<string, string> Matr { get; set; } //TODO

        [TdfField("MCAP")]
        public ushort Mcap { get; set; } //TODO max capacity?

        [TdfField("NQOS")]
        public QosNetworkData NetworkData { get; set; }

        [TdfField("NTOP")]
        public NetworkTopology NetworkTopology { get; set; }

        [TdfField("PGID")]
        public string Pgid { get; set; } //TODO platform gameid?

        [TdfField("PGSR")]
        public byte[] Pgsr { get; set; } //TODO platform gameserver?

        [TdfField("PHST")]
        public HostData PlatformHost { get; set; }

        [TdfField("PSAS")]
        public string PingServerName { get; set; }

        [TdfField("QCAP")]
        public ushort QueueCapacity { get; set; } //TODO

        [TdfField("SEED")]
        public uint Seed { get; set; } //TODO

        [TdfField("THST")]
        public HostData TopologyHost { get; set; } //normally dirtybot

        [TdfField("UUID")]
        public string Uuid { get; set; } //TODO

        [TdfField("VOIP")]
        public VoipTopology VoipTopology { get; set; }

        [TdfField("VSTR")]
        public string VersionString { get; set; }

        [TdfField("XNNC")]
        public byte[] Xnnc { get; set; } //TODO

        [TdfField("XSES")]
        public byte[] Xses { get; set; } //TODO
    }

    public class HostData //TODO: better name
    {
        [TdfField("HPID")]
        public uint HostPersonaId { get; set; }

        [TdfField("HSLT")]
        public byte HostSlot { get; set; }
    }

    public class PlayerData
    {
        [TdfField("BLOB")]
        public byte[] Blob { get; set; }

        [TdfField("EXID")]
        public ulong ExternalId { get; set; }

        [TdfField("GID")]
        public uint Gid { get; set; } //TODO

        [TdfField("LOC")]
        public uint Locale { get; set; }
        
        [TdfField("NAME")]
        public string Username { get; set; }

        [TdfField("NQOS")]
        public QosNetworkData NetworkData { get; set; }

        [TdfField("PATT")]
        public Dictionary<string, string> PlayerAttributes { get; set; }

        [TdfField("PID")]
        public uint PersonaId { get; set; }

        [TdfField("PNET")]
        public KeyValuePair<NetworkAddressType, PairNetworkAddress> PlayerNetwork { get; set; }

        [TdfField("SID")]
        public byte Sid { get; set; } //TODO

        [TdfField("SLOT")]
        public int Slot { get; set; } //TODO: enum public/private?

        [TdfField("STAT")]
        public PlayerState PlayerState { get; set; }

        [TdfField("TEAM")]
        public ushort Team { get; set; } //TODO

        [TdfField("TIDX")]
        public ushort Tidx { get; set; } //TODO

        [TdfField("TIME")]
        public long Time { get; set; } //TODO

        [TdfField("UID")]
        public uint UserId { get; set; }
    }
}
