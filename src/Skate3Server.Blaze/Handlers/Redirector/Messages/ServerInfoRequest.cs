using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Redirector.Messages
{
    [BlazeRequest(BlazeComponent.Redirector, 0x1)]
    public class ServerInfoRequest : IRequest<ServerInfoResponse>
    {
        [TdfField("BSDK")]
        public string BlazeSdk { get; set; }

        [TdfField("BTIM")]
        public string BlazeTime { get; set; }

        [TdfField("CLNT")]
        public string ClientId { get; set; }

        [TdfField("CSKU")]
        public string ClientSku { get; set; }

        [TdfField("CVER")]
        public string ClientVersion { get; set; }

        [TdfField("DSDK")]
        public string SdkVersion { get; set; }

        [TdfField("ENV")]
        public string Environment { get; set; }

        [TdfField("FPID")]
        public KeyValuePair<FirstPartyIdType, byte[]> FirstPartyId { get; set; }

        [TdfField("LOC")]
        public uint Locale { get; set; }

        [TdfField("NAME")]
        public string Name { get; set; }
        
        [TdfField("PLAT")]
        public string Platform { get; set; }

        [TdfField("PROF")]
        public string Profile { get; set; }

    }
}
