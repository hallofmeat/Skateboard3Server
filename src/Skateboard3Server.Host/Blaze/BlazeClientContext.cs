using System.Collections.Concurrent;
using System.Collections.Generic;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Host.Blaze;

public class BlazeClientContext : ClientContext
{
    public BlazeClientContext()
    {
        PendingNotifications = new ConcurrentQueue<BlazeMessageData>();
    }

    internal ConnectionContext ConnectionContext { get; set; }
    internal ProtocolWriter Writer { get; set; }
    internal ProtocolReader Reader { get; set; }
    internal ConcurrentQueue<BlazeMessageData> PendingNotifications { get; }

    public override string ConnectionId => ConnectionContext?.ConnectionId;

    public override uint? UserId { get; set; }
    public override uint? PersonaId { get; set; }
    public override uint? UserSessionId { get; set; }

    public override IDictionary<object, object> Items => ConnectionContext?.Items;

}