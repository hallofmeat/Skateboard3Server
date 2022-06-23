using System;
using System.Buffers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using NLog;

namespace Skateboard3Server.Host;

public class DummyConnectionHandler : ConnectionHandler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var reader = connection.Transport.Input;
        while (true)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;
            reader.AdvanceTo(ReadStuff(ref buffer));

            if (result.IsCompleted) break;
        }
    }

    public SequencePosition ReadStuff(ref ReadOnlySequence<byte> buffer)
    {
        var seqReader = new SequenceReader<byte>(buffer);
        seqReader.Advance(seqReader.Length);
        return seqReader.Position;
    }

}