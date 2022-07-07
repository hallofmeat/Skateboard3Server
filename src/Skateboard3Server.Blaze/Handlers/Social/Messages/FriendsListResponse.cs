using System.Collections.Generic;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Social.Messages;

[BlazeResponse(BlazeComponent.Social, (ushort)SocialCommand.FriendsList)]
public record FriendsListResponse : BlazeResponseMessage
{
    [TdfField("ALMP")]
    public Dictionary<string, ResponseList> ResponseLists { get; set; }
}

public record ResponseList
{
    [TdfField("ALML")]
    public List<ListData>? ListData { get; init; } //TODO user list? (optional)

    [TdfField("BOID")]
    public ulong Boid { get; set; } //TODO objectid? (short 0x19, short 0x01, int unknown)

    [TdfField("LID")]
    public uint Lid { get; set; } //TODO listid?

    [TdfField("LMS")]
    public uint Lms { get; set; } //TODO limit?

    [TdfField("LMN")]
    public string Name { get; set; }

    [TdfField("RFLG")]
    public bool RFlag { get; set; } //TODO read flag?

    [TdfField("SFLG")]
    public bool SFlag { get; set; } //TODO set flag?
}

public record ListData
{
    [TdfField("MATS")]
    public long Mats { get; set; } //TODO name format?

    [TdfField("MUIF")]
    public Muif Muif { get; set; } //TODO name
}

public record Muif
{
    [TdfField("EDAT")]
    public UserExtendedData UserExtendedData { get; set; }

    [TdfField("FLGS")]
    public uint Flags { get; set; } //always 1?

    [TdfField("USER")]
    public UserInformation UserInformation { get; set; }
}
