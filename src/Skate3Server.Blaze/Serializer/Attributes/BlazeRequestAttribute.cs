using System;

namespace Skate3Server.Blaze.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlazeRequestAttribute : Attribute
    {
        public BlazeRequestAttribute(BlazeComponent component, int command)
        {
            
        }
    }
}