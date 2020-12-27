using System.Xml.Serialization;

namespace Skate3Server.Api.Controllers.WebServices.Common
{
    public class IntegerContainer
    {
        public IntegerContainer()
        {
        }

        public IntegerContainer(int value)
        {
            Value = value;
        }
        
        [XmlElement(ElementName = "value")]
        public int Value { get; set; }
    }
}