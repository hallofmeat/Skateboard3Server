using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.Redirector.Messages;

namespace Skateboard3Server.Blaze.Handlers.Redirector;

public class ServerInfoHandler : IRequestHandler<ServerInfoRequest, ServerInfoResponse>
{
    private readonly BlazeConfig _blazeConfig;

    public ServerInfoHandler(IOptions<BlazeConfig> blazeConfig)
    {
        _blazeConfig = blazeConfig.Value;
    }
    public Task<ServerInfoResponse> Handle(ServerInfoRequest request, CancellationToken cancellationToken)
    {
        var response = new ServerInfoResponse
        {
            Address = new KeyValuePair<NetworkAddressType, ServerNetworkAddress>(NetworkAddressType.Server, new ServerNetworkAddress
            {
                Host = _blazeConfig.PublicHost,
                Ip = Convert.ToUInt32(IPAddress.Parse(_blazeConfig.PublicIp).Address),
                Port = 10744
            }),
            Secure = false,
            Xdns = 0
        };
        return Task.FromResult(response);
    }
}