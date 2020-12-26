using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Teams.Messages;

namespace Skate3Server.Blaze.Handlers.Teams
{
    public class Unknown640Handler : IRequestHandler<Unknown640Request, Unknown640Response> //TODO: I think this is invites
    {
        public async Task<Unknown640Response> Handle(Unknown640Request request, CancellationToken cancellationToken)
        {
            var response = new Unknown640Response();
            return response;
        }
    }
}