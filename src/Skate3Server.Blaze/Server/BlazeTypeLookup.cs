using System;
using System.Collections.Generic;
using System.Linq;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Server
{
    public interface IBlazeTypeLookup
    {
        bool TryGetRequestType(BlazeComponent component, ushort command, out Type requestType);
    }

    public class BlazeTypeLookup : IBlazeTypeLookup
    {
        private readonly Dictionary<(BlazeComponent, ushort), Type> _requestTypeLookup;

        public BlazeTypeLookup()
        {
            var assembly = typeof(IBlazeTypeLookup).Assembly;
            _requestTypeLookup = (from type in assembly.GetTypes()
                    where Attribute.IsDefined(type, typeof(BlazeRequestAttribute))
                    let attributes = type.GetCustomAttributes(typeof(BlazeRequestAttribute), true)
                    where attributes.Length == 1
                    let attr = attributes.Single() as BlazeRequestAttribute
                    select new {Type = type, Attribute = attr})
                .ToDictionary(key => (key.Attribute.Component, key.Attribute.Command), val => val.Type);
        }

        public bool TryGetRequestType(BlazeComponent component, ushort command, out Type requestType)
        {
            return _requestTypeLookup.TryGetValue((component, command), out requestType);
        }
    }
}