using Skateboard3Server.Blaze.Common;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Managers.Models;

public class Player
{
    public byte SlotId { get; set; }
    public long AccountId { get; set; }
    public uint UserId { get; set; }
    public uint PersonaId { get; set; }
    public ulong ExternalId { get; set; }
    public byte[] ExternalBlob { get; set; }
    public string Username { get; set; }
    public PlayerState State { get; set; }
    public long ConnectTime { get; set; } //microseconds
    public PairNetworkAddress NetworkAddress { get; set; }
}