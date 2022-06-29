using System.Collections.Generic;
using Skateboard3Server.Blaze.Common;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Managers.Models;

public class Game
{
    public uint GameId { get; set; }
    public string Uuid { get; set; }
    public string Name { get; set; }
    public GameState State { get; set; }
    public uint Settings { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
    public string Version { get; set; }
    public ushort Capacity { get; set; }


    //TODO: threadsafety?
    public List<Player> Players { get; set; }
    public uint AdminId { get; set; }
    public uint HostId { get; set; }

    public PairNetworkAddress HostNetwork { get; set; }
}