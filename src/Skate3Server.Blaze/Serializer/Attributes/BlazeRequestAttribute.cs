using System;

namespace Skate3Server.Blaze.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlazeRequestAttribute : BlazeMessageAttribute
    {
        public BlazeRequestAttribute(BlazeComponent component, ushort command) : base(component, command)
        {
        }
    }
}