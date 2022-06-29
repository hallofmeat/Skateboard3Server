using Skateboard3Server.Blaze.Common;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Managers.Models;

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