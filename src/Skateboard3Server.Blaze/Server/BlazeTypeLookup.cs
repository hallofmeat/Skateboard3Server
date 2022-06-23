using System;
using System.Collections.Generic;
using System.Linq;
using Skateboard3Server.Blaze.Serializer.Attributes;

namespace Skateboard3Server.Blaze.Server;

public interface IBlazeTypeLookup
{
    bool TryGetRequestType(BlazeComponent component, ushort command, out Type requestType);

    bool TryGetResponseComponentCommand(Type responseType, out BlazeComponent component, out ushort command);
}

public class BlazeTypeLookup : IBlazeTypeLookup
{
    private readonly Dictionary<(BlazeComponent, ushort), Type> _requestLookup;
    private readonly Dictionary<Type, (BlazeComponent, ushort)> _responseLookup;

    public BlazeTypeLookup()
    {
        var assembly = typeof(IBlazeTypeLookup).Assembly;
        //TODO: cleanup
        _requestLookup = (from type in assembly.GetTypes()
            where Attribute.IsDefined(type, typeof(BlazeRequestAttribute))
            let attributes = type.GetCustomAttributes(typeof(BlazeMessageAttribute), true)
            where attributes.Length == 1
            let attr = attributes.Single() as BlazeRequestAttribute
            select new {Type = type, Attribute = attr}).ToDictionary(
            key => (key.Attribute.Component, key.Attribute.Command),
            val => val.Type);

        _responseLookup = (from type in assembly.GetTypes()
            where Attribute.IsDefined(type, typeof(BlazeResponseAttribute))
            let attributes = type.GetCustomAttributes(typeof(BlazeResponseAttribute), true)
            where attributes.Length == 1
            let attr = attributes.Single() as BlazeResponseAttribute
            select new {Type = type, Attribute = attr}).ToDictionary(key => key.Type,
            val => (val.Attribute.Component, val.Attribute.Command));
    }

    public bool TryGetRequestType(BlazeComponent component, ushort command, out Type requestType)
    {
        return _requestLookup.TryGetValue((component, command), out requestType);
    }

    public bool TryGetResponseComponentCommand(Type responseType, out BlazeComponent component, out ushort command)
    {
        var result = _responseLookup.TryGetValue(responseType, out var output);
        component = output.Item1;
        command = output.Item2;

        return result;
    }
}