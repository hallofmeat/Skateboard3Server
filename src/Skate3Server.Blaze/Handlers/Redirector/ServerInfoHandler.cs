using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Handlers.Redirector.Messages;

namespace Skate3Server.Blaze.Handlers.Redirector
{
    public class ServerInfoHandler : IRequestHandler<ServerInfoRequest, ServerInfoResponse>
    {
        public async Task<ServerInfoResponse> Handle(ServerInfoRequest request, CancellationToken cancellationToken)
        {
            var response = new ServerInfoResponse
            {
                Address = new KeyValuePair<NetworkAddressType, ServerNetworkAddress>(NetworkAddressType.Server, new ServerNetworkAddress
                {
                    Host = BlazeConfig.BlazeHost,
                    Ip = Convert.ToUInt32(IPAddress.Parse(BlazeConfig.BlazeIp).Address),
                    Port = 10744
                }),
                Secure = false,
                Xdns = 0
            };
            return response;
        }
    }
}