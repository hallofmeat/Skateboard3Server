using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Skate3Server.Blaze.Server
{
    public interface IClientManager
    {
        ClientContext this[string connectionId] { get; }
        int Count { get; }
        void Add(ClientContext client);
        void Remove(ClientContext client);
    }

    public class ClientManager : IClientManager
    {
        private readonly ConcurrentDictionary<string, ClientContext> _clients =
            new ConcurrentDictionary<string, ClientContext>(StringComparer.Ordinal);

        [CanBeNull]
        public ClientContext this[string connectionId]
        {
            get
            {
                _clients.TryGetValue(connectionId, out var connection);
                return connection;
            }
        }

        public int Count => _clients.Count;

        public void Add(ClientContext client)
        {
            _clients.TryAdd(client.ConnectionId, client);
        }

        public void Remove(ClientContext client)
        {
            _clients.TryRemove(client.ConnectionId, out _);
        }

    }
}
