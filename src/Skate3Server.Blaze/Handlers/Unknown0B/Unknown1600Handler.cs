using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Unknown0B.Messages;

namespace Skate3Server.Blaze.Handlers.Unknown0B
{
    public class Unknown1600Handler : IRequestHandler<Unknown1600Request, Unknown1600Response>
    {
        public async Task<Unknown1600Response> Handle(Unknown1600Request request, CancellationToken cancellationToken)
        {
            var response = new Unknown1600Response();
            return response;
        }
    }
}