using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Connections;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Host
{
    public class BlazeClientContext : ClientContext
    {
        public BlazeClientContext()
        {
            Notifications = new ConcurrentQueue<ValueTuple<BlazeHeader, IBlazeNotification>>();
        }

        internal ConnectionContext ConnectionContext { get; set; }

        public override string ConnectionId => ConnectionContext?.ConnectionId;

        public override uint UserId { get; set; }

        public override string Username { get; set; }
        public override ulong ExternalId { get; set; }

        public override IDictionary<object, object> Items => ConnectionContext?.Items;

        public override ConcurrentQueue<ValueTuple<BlazeHeader, IBlazeNotification>> Notifications { get; }
    }
}
