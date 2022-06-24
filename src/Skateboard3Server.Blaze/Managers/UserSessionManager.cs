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
        var hexKey = HexString(rawKey);

        sessionData.SetSessionData(id, hexKey);

        if (!_sessions.TryAdd(id, sessionData))
        {
            throw new Exception($"Could not add session SessionId:{id}");
        }

        return (sessionData.SessionId, sessionData.SessionKey);
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
            if (encodedKey.Equals(sessionData.RawSessionKey))
            {
                return sessionData;
            }
        }

        return null;
    }

    private string HexString(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}

public class UserSessionData
{
    public long AccountId { get; set; }
    public uint UserId { get; set; }
    public uint PersonaId { get; set; }
    public string Username { get; set; }
    public ulong ExternalId { get; set; }
    public byte[] ExternalBlob { get; set; }

    public PairNetworkAddress NetworkAddress { get; set; }

    //Set by UserSessionManager
    public void SetSessionData(uint sessionId, string rawSessionKey)
    {
        SessionId = sessionId;
        RawSessionKey = rawSessionKey;
        SessionKey = GenerateSessionKey(sessionId, rawSessionKey);
    }
    private string GenerateSessionKey(uint id, string key)
    {
        return id.ToString("X8") + "_" + key;
    }
    public uint SessionId { get; private set; }
    public string RawSessionKey { get; private set; }
    public string SessionKey { get; private set; } //SessionId(hex)_SessionKey

}