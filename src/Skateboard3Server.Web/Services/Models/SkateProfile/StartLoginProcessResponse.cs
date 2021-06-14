using System.Xml.Serialization;

namespace Skateboard3Server.Web.Services.Models.SkateProfile
{
    [XmlRoot(ElementName = "LoginInfoContainer")]
    public class StartLoginProcessResponse
    {

        [XmlElement(ElementName = "privacyFlagContainer")]
        public string PrivacyFlagContainer { get; set; } //TODO: string probably isnt right, but need it to force a selfclosing tag

        [XmlElement(ElementName = "awardedBoardSales")]
        public ulong AwardedBoardSales { get; set; }

        [XmlElement(ElementName = "teamInfo")]
        public TeamInfo TeamInfo { get; set; }
    }

    public class TeamInfo
    {
        [XmlElement(ElementName = "teamId")]
        public uint TeamId { get; set; } //TODO: long correct type?

        [XmlElement(ElementName = "numMembers")]
        public uint NumMembers { get; set; } //TODO: long correct type?
    }
}
