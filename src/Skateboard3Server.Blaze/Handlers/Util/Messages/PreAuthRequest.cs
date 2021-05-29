using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util.Messages
{
    [BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.PreAuth)]
    public class PreAuthRequest : BlazeRequest, IRequest<PreAuthResponse>
    {
        [TdfField("CDAT")]
        public ClientData ClientData { get; set; }

        [TdfField("CINF")]
        public ClientInfo ClientInfo { get; set; }

        [TdfField("FCCR")]
        public ClientConfigData ClientConfig { get; set; }

    }

    public class ClientData
    {
        [TdfField("LANG")]
        public uint Language { get; set; }

        [TdfField("TYPE")]
        public int Type { get; set; }
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

    public class ClientConfigData
    {
        [TdfField("CFID")]
        public string ConfigId { get; set; }
    }

}
