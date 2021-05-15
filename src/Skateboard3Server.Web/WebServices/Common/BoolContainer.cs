using System.Xml.Serialization;

namespace Skateboard3Server.Web.WebServices.Common
{
    public class BoolContainer
    {
        public BoolContainer()
        {
        }

        public BoolContainer(bool value)
        {
            Value = value;
        }
        
        [XmlElement(ElementName = "value")]
        public bool Value { get; set; }
    }
}