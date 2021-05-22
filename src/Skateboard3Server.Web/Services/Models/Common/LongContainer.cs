using System.Xml.Serialization;

namespace Skateboard3Server.Web.Services.Models.Common
{
    public class LongContainer
    {
        public LongContainer()
        {
        }

        public LongContainer(long value)
        {
            Value = value;
        }
        
        [XmlElement(ElementName = "value")]
        public long Value { get; set; }
    }
}