using System.Runtime.Serialization;
using Skateboard3Server.Web.WebServices.Common;

namespace Skateboard3Server.Web.WebServices.SkateProfile
{
    [DataContract]
    public class SetUserDlc
    {
        [DataMember]
        public PlatformType PlatformId { get; set; }
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public string OfferIds { get; set; }
    }
}