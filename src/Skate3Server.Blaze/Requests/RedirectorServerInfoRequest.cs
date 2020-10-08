using System;
using MediatR;
using Skate3Server.Blaze.Responses;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Requests
{
    //[BlazeRequest(BlazeComponent.Redirector, 0x1)]
    public class RedirectorServerInfoRequest : IRequest<RedirectorServerInfoResponse>
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
        
        [TdfUnion("FPID", nameof(FirstPartyId))]
        public FirstPartyIdType FirstPartyIdType { get; set; }
        public byte[] FirstPartyId { get; set; }

        [TdfField("LOC")]
        public UInt32 Locale { get; set; }

        [TdfField("NAME")]
        public string Name { get; set; }
        
        [TdfField("PLAT")]
        public string Platform { get; set; }

        [TdfField("PROF")]
        public string Profile { get; set; }

    }
}
