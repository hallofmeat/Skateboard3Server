using System.Collections.Generic;

namespace Skateboard3Server.Blaze.Server
{
    public abstract class ClientContext
    {
        public abstract string ConnectionId { get; }
        
        public abstract uint UserId { get; set; }
        public abstract string Username { get; set; }
        public abstract ulong ExternalId { get; set; }

        public abstract IDictionary<object, object> Items { get; }

    }
}