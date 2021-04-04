using System;

namespace Skateboard3Server.Blaze.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TdfFieldAttribute : Attribute
    {
        public TdfFieldAttribute(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; }

    }
}