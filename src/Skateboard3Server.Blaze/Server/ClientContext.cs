using System.Collections.Generic;

namespace Skateboard3Server.Blaze.Server;

public abstract class ClientContext
{
    public abstract string ConnectionId { get; }

    public abstract uint? UserId { get; set; }
    public abstract uint? PersonaId { get; set; }
    public abstract uint? UserSessionId { get; set; }

    public abstract IDictionary<object, object> Items { get; }

}