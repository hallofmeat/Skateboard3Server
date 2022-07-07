using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.Social.Messages;

namespace Skateboard3Server.Blaze.Handlers.Social;

public class FriendsListHandler : IRequestHandler<FriendsListRequest, FriendsListResponse>
{
    public Task<FriendsListResponse> Handle(FriendsListRequest request, CancellationToken cancellationToken)
    {
        var response = new FriendsListResponse
        {
            ResponseLists = new Dictionary<string, ResponseList>
            {
                //TODO: temp, fix later
                {"friendList", new ResponseList
                {
                    ListData = null,
                    Boid = 1234, //TODO
                    Lid = 1,
                    Lms = 100,
                    Name = "friendList",
                    RFlag = false,
                    SFlag = false
                }},
                {"recentPlayerList", new ResponseList
                {
                    ListData = null,
                    Boid = 1234, //TODO
                    Lid = 1,
                    Lms = 100,
                    Name = "recentPlayerList",
                    RFlag = false,
                    SFlag = false
                }},
            }
        };
        return Task.FromResult(response);
    }
}