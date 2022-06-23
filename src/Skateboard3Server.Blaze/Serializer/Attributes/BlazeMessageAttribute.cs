using System;

namespace Skateboard3Server.Blaze.Serializer.Attributes;

public abstract class BlazeMessageAttribute : Attribute
{
    protected BlazeMessageAttribute(BlazeComponent component, ushort command)
    {
        Command = command;
        Component = component;
    }

    public BlazeComponent Component { get; }

    public ushort Command { get; }

}