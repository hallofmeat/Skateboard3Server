using System;

namespace Skate3Server.Blaze.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TdfUnionKeyAttribute : TdfFieldAttribute
    {
        public TdfUnionKeyAttribute(string tag, string valueProperty) : base(tag)
        {
        }
    }
}