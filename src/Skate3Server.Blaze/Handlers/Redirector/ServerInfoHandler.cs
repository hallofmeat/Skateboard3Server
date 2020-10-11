using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Redirector.Messages;

namespace Skate3Server.Blaze.Handlers.Redirector
{
    public class ServerInfoHandler : IRequestHandler<RedirectorServerInfoRequest, RedirectorServerInfoResponse>
    {
        public async Task<RedirectorServerInfoResponse> Handle(RedirectorServerInfoRequest request, CancellationToken cancellationToken)
        {
            var response = new RedirectorServerInfoResponse
            {
                Address = new NetworkAddress
                {
                    Host = "localhost",
                    Ip = Convert.ToUInt32(IPAddress.Parse("127.0.0.1").Address),
                    Port = 10744
                },
                AddressType = NetworkAddressType.Client,
                Secure = 0,
                Xdns = 0
            };
            return response;
        }
    }
}