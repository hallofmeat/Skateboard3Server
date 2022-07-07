using System.Collections.Generic;
using Skateboard3Server.Blaze.Serializer.Attributes;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Common;

public record UserExtendedData
{
    [TdfField("ADDR")]
    public KeyValuePair<NetworkAddressType, NetworkAddress?> Address { get; set; }

    [TdfField("BPS")]
    public string PingServerName { get; set; } //maps to qos ping server name

    [TdfField("CTY")]
    public string Cty { get; set; } //TODO city?

    [TdfField("DMAP")]
    public Dictionary<uint, int> DataMap { get; set; }

    [TdfField("HWFG")]
    public uint HardwareFlags { get; set; }

    [TdfField("PSLM")]
    public List<int> Pings { get; set; } //One entry for each qos ping server

    [TdfField("QDAT")]
    public QosNetworkData NetworkData { get; set; }

    [TdfField("ULST")]
    public List<ulong> UserGameList { get; set; } //(short 0x04, short 0x01, int gameid) //TODO: is this right?

    [TdfField("UATT")]
    public ulong UserAttributes { get; set; }
}