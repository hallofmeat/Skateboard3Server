using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Skate3Server.Blaze.Server
{
    public abstract class ClientContext
    {
        public abstract string ConnectionId { get; }
        
        public abstract uint UserId { get; set; }
        public abstract string Username { get; set; }

        public abstract IDictionary<object, object> Items { get; }

        //TODO not sure if this should live here, also is this is the right type
        public abstract ConcurrentQueue<ValueTuple<BlazeHeader, IBlazeNotification>> Notifications { get; }
    }
}