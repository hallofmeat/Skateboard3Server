using System;

namespace Skate3Server.Blaze.Serializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TdfUnionValueAttribute : TdfFieldAttribute
    {
        public TdfUnionValueAttribute() : base("VALU")
        {
        }
    }
}