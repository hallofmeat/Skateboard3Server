using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Social.Messages;

[BlazeRequest(BlazeComponent.Social, (ushort)SocialCommand.FriendsList)]
[UsedImplicitly]
public record FriendsListRequest : BlazeRequestMessage, IRequest<FriendsListResponse>
{
    [TdfField("ALST")]
    public List<RequestList> RequestLists { get; init; } //TODO
}

public record RequestList
{
    [TdfField("ALNM")]
    public string Name { get; init; } //TODO

    [TdfField("SUBF")]
    public bool SubF { get; init; } //TODO
}