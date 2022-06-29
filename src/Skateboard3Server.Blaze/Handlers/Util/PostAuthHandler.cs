using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Skateboard3Server.Blaze.Handlers.Util.Messages;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util;

public class PostAuthHandler : IRequestHandler<PostAuthRequest, PostAuthResponse>
{
    private readonly ClientContext _clientContext;
    private readonly BlazeConfig _blazeConfig;

    public PostAuthHandler(IOptions<BlazeConfig> blazeConfig, ClientContext clientContext)
    {
        _clientContext = clientContext;
        _blazeConfig = blazeConfig.Value;
    }

    public Task<PostAuthResponse> Handle(PostAuthRequest request, CancellationToken cancellationToken)
    {
        var response = new PostAuthResponse
        {
            TelemetryServer = new TelemetryServer
            {
                Ip = _blazeConfig.PublicIp,
                Anonymous = false,
                Disable = "AD,AF,AG,AI,AL,AM,AN,AO,AQ,AR,AS,AW,AX,AZ,BA,BB,BD,BF,BH,BI,BJ,BM,BN,BO,BR,BS,BT,BV,BW,BY,BZ,CC,CD,CF,CG,CI,CK,CL,CM,CN,CO,CR,CU,CV,CX,DJ,DM,DO,DZ,EC,EG,EH,ER,ET,FJ,FK,FM,FO,GA,GD,GE,GF,GG,GH,GI,GL,GM,GN,GP,GQ,GS,GT,GU,GW,GY,HM,HN,HT,ID,IL,IM,IN,IO,IQ,IR,IS,JE,JM,JO,KE,KG,KH,KI,KM,KN,KP,KR,KW,KY,KZ,LA,LB,LC,LI,LK,LR,LS,LY,MA,MC,MD,ME,MG,MH,ML,MM,MN,MO,MP,MQ,MR,MS,MU,MV,MW,MY,MZ,NA,NC,NE,NF,NG,NI,NP,NR,NU,OM,PA,PE,PF,PG,PH,PK,PM,PN,PS,PW,PY,QA,RE,RS,RW,SA,SB,SC,SD,SG,SH,SJ,SL,SM,SN,SO,SR,ST,SV,SY,SZ,TC,TD,TF,TG,TH,TJ,TK,TL,TM,TN,TO,TT,TV,TZ,UA,UG,UM,UY,UZ,VA,VC,VE,VG,VN,VU,WF,WS,YE,YT,ZM,ZW,ZZ",
                Filter = "",
                Locale = 1701729619, //enUS
                Nook = "US,CA,MX",
                Port = 9946,
                Delay = 15000,
                Key = "some-telemetry-key",
                Spct = 75
            },
            TickServer = new TickServer
            {
                Ip = _blazeConfig.PublicIp,
                Port = 8999,
                Key = $"{_clientContext.UserId},{_blazeConfig.PublicIp}:8999,skate-2010-ps3,10,50,50,50,50,0,0"
            }
        };
        return Task.FromResult(response);
    }
}