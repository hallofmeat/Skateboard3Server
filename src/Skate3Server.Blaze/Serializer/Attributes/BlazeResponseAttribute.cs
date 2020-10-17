using System;

namespace Skate3Server.Blaze.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlazeResponseAttribute : BlazeMessageAttribute
    {
        public BlazeResponseAttribute(BlazeComponent component, ushort command) : base(component, command)
        {
        }
    }
}