using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading;
using NLog;
using Skateboard3Server.Blaze.Common;

namespace Skateboard3Server.Blaze.Managers;

public interface IUserSessionManager
{
    (uint SessionId, string SessionKey) StoreSession(UserSessionData sessionData);
    UserSessionData GetSession(uint id);
    void RemoveSession(uint id);
    UserSessionData GetUserSessionDataForKey(string key);
    string GetSessionKey(uint id);
}

/// <summary>
/// Current sessions connected to the server
/// the sessions die with the server so we are fine with keeping them in memory
/// </summary>
public class UserSessionManager : IUserSessionManager
{

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly ConcurrentDictionary<uint, UserSessionData> _sessions =
        new ConcurrentDictionary<uint, UserSessionData>();

    private int _currentSessionCount = 0;

    public (uint SessionId, string SessionKey) StoreSession(UserSessionData sessionData)
    {
        var rawKey = new byte[16];
        RandomNumberGenerator.Fill(rawKey);
        var id = (uint) Interlocked.Increment(ref _currentSessionCount); //generate a session id
        sessionData.SessionId = id;
        sessionData.SessionKey = HexString(rawKey);

        if (!_sessions.TryAdd(id, sessionData))
        {
            throw new Exception($"Could not add session SessionId:{id}");
        }

        var sessionKey = GenerateSessionKey(id, sessionData.SessionKey);
        return (id, sessionKey);
    }

    public UserSessionData GetSession(uint id)
    {
        if (!_sessions.TryGetValue(id, out var sessionData))
        {
            throw new ArgumentException($"SessionId:{id} does not exist");
        }

        return sessionData;
    }

    public void RemoveSession(uint id)
    {
        _sessions.TryRemove(id, out _);
    }

    public UserSessionData GetUserSessionDataForKey(string key)
    {
        //split lookup id, validate byte
        var parts = key.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            Logger.Warn("Invalid session key!");
            return null;
        }

        var encodedSessionId = parts[0];
        var encodedKey = parts[1];

        var sessionId = Convert.ToUInt32(encodedSessionId, 16);

        if (_sessions.TryGetValue(sessionId, out var sessionData))
        {
            if (encodedKey.Equals(sessionData.SessionKey))
            {
                return sessionData;
            }
        }

        return null;
    }

    public string GetSessionKey(uint id)
    {
        if (!_sessions.TryGetValue(id, out var sessionData))
        {
            throw new ArgumentException($"SessionId:{id} does not exist");
        }
         
        return GenerateSessionKey(id, sessionData.SessionKey);
    }

    private string GenerateSessionKey(uint id, string key)
    {
        return id.ToString("X8") + "_" + key;
    }

    private string HexString(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}

public class UserSessionData
{
    public uint SessionId { get; set; }
    public string SessionKey { get; set; }
    public long AccountId { get; set; }
    public uint UserId { get; set; }
    public uint PersonaId { get; set; }
    public string Username { get; set; }
    public ulong ExternalId { get; set; }
    public byte[] ExternalBlob { get; set; }

    public PairNetworkAddress NetworkAddress { get; set; }
}