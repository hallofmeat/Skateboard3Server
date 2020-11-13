using System.Collections.Generic;

namespace Skate3Server.Blaze.Server
{
    public abstract class ClientContext
    {
        public abstract string ConnectionId { get; }
        
        public abstract uint UserId { get; set; }

        public abstract IDictionary<object, object> Items { get; }
    }
}