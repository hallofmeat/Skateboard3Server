using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    public class PreAuthRequest : IRequest<PreAuthResponse>
    {
        [TdfField("CDAT")]
        public ClientData ClientData { get; set; }

        [TdfField("CINF")]
        public ClientInfo ClientInfo { get; set; }

        [TdfField("FCCR")]
        public FccrData Fccr { get; set; } //TODO: figure out what this stands for

    }

    public class ClientData
    {
        [TdfField("LANG")]
        public uint Language { get; set; }

        [TdfField("TYPE")]
        public uint Type { get; set; }
    }

    public class ClientInfo
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

        [TdfField("LOC")]
        public uint Locale { get; set; }

        [TdfField("MAC")]
        public string MacAddress { get; set; }

        [TdfField("PLAT")]
        public string Platform { get; set; }

    }

    public class FccrData
    {
        [TdfField("CFID")]
        public string CfId { get; set; } //TODO: figure out what this stands for
    }

}
