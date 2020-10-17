using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Redirector.Messages;

namespace Skate3Server.Blaze.Handlers.Redirector
{
    public class ServerInfoHandler : IRequestHandler<ServerInfoRequest, ServerInfoResponse>
    {
        public async Task<ServerInfoResponse> Handle(ServerInfoRequest request, CancellationToken cancellationToken)
        {
            var response = new ServerInfoResponse
            {
                Address = new KeyValuePair<NetworkAddressType, NetworkAddress>(NetworkAddressType.Client, new NetworkAddress
                {
                    Host = "localhost",
                    Ip = Convert.ToUInt32(IPAddress.Parse("127.0.0.1").Address),
                    Port = 10744
                }),
                Secure = false,
                Xdns = 0
            };
            return response;
        }
    }
}