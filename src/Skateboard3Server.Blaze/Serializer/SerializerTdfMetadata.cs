using System.Reflection;
using Skateboard3Server.Blaze.Serializer.Attributes;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Serializer;

public class SerializerTdfMetadata
{
    public PropertyInfo Property { get; set; }
    public TdfFieldAttribute Attribute { get; set; }
}