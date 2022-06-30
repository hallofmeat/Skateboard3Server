using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618 //Constructed via DI and Connected sets connection specific bits

namespace Skateboard3Server.Host.Blaze;

public class BlazeClientContext : ClientContext
{
    public BlazeClientContext()
    {
        PendingNotifications = new ConcurrentQueue<BlazeMessageData>();
    }

    [MemberNotNull(nameof(ConnectionContext), nameof(Writer), nameof(Reader))]
    internal void Connected(ConnectionContext context, ProtocolWriter writer, ProtocolReader reader)
    {
        ConnectionContext = context;
        Writer = writer;
        Reader = reader;
    }

    private ConnectionContext ConnectionContext { get; set; }
    internal ProtocolWriter? Writer { get; set; }
    internal ProtocolReader Reader { get; set; }
    internal ConcurrentQueue<BlazeMessageData> PendingNotifications { get; }

    public override string ConnectionId => ConnectionContext.ConnectionId;

    public override uint? UserId { get; set; }
    public override uint? PersonaId { get; set; }
    public override uint? UserSessionId { get; set; }

    public override IDictionary<object, object?> Items => ConnectionContext.Items;

}