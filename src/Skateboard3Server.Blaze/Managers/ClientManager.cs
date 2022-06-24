using System;
using System.Collections.Concurrent;
using NLog;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Managers;

public interface IClientManager
{
    ClientContext? this[string connectionId] { get; }
    int Count { get; }
    void Add(ClientContext client);
    void Remove(ClientContext client);
    bool PersonaConnected(uint personaId);
    ClientContext? GetByPersonaId(uint personaId);
}

public class ClientManager : IClientManager
{
    private readonly IUserSessionManager _userSessionManager;

    private readonly ConcurrentDictionary<string, ClientContext> _clients =
        new ConcurrentDictionary<string, ClientContext>(StringComparer.Ordinal);

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public ClientManager(IUserSessionManager userSessionManager)
    {
        _userSessionManager = userSessionManager;
    }

    public ClientContext? this[string connectionId]
    {
        get
        {
            _clients.TryGetValue(connectionId, out var client);
            return client;
        }
    }

    public int Count => _clients.Count;

    public void Add(ClientContext client)
    {
        //TODO: remove
        Logger.Debug($"Adding client {client.ConnectionId}");

        if (client.ConnectionId == null)
        {
            Logger.Warn("Tried to Add ClientContext with null ConnectionId");
            return;
        }
        _clients.TryAdd(client.ConnectionId, client);
    }

    public void Remove(ClientContext client)
    {
        //TODO: remove
        Logger.Debug($"Removing client {client.ConnectionId}");

        if (client.ConnectionId == null)
        {
            Logger.Warn("Tried to Remove ClientContext with null ConnectionId");
            return;
        }

        if (client.UserSessionId != null)
        {
            _userSessionManager.RemoveSession(client.UserSessionId.Value);
        }

        _clients.TryRemove(client.ConnectionId, out _);
    }

    public bool PersonaConnected(uint personaId)
    {
        return GetByPersonaId(personaId) != null;
    }

    public ClientContext? GetByPersonaId(uint personaId)
    {
        //TODO: this isnt great, hash map maybe?
        foreach (var (_, client) in _clients)
        {
            if (client.PersonaId == personaId)
            {
                return client;
            }
        }

        return null;
    }

}