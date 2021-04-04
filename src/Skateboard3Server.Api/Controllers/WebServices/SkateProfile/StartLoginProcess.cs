using System.Runtime.Serialization;
using Skateboard3Server.Api.Controllers.WebServices.Common;

namespace Skateboard3Server.Api.Controllers.WebServices.SkateProfile
{
    [DataContract]
    public class StartLoginProcess
    {
        [DataMember]
        public PlatformType PlatformType { get; set; }
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public string SessionKey { get; set; }
        [DataMember]
        public bool DeleteThumbs { get; set; }
        [DataMember]
        public int Difficulty { get; set; } //TODO: enum?
        [DataMember]
        public ulong TotalBoardSales { get; set; }
    }
}
