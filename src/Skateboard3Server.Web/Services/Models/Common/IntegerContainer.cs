using System.Xml.Serialization;

namespace Skateboard3Server.Web.Services.Models.Common
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