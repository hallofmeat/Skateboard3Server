using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Unknown0B.Messages;

namespace Skate3Server.Blaze.Handlers.Unknown0B
{
    public class Unknown2700Handler : IRequestHandler<Unknown2700Request, Unknown2700Response>
    {
        public async Task<Unknown2700Response> Handle(Unknown2700Request request, CancellationToken cancellationToken)
        {
            var response = new Unknown2700Response();
            return response;
        }
    }
}