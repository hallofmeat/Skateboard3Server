﻿// ReSharper disable EnumUnderlyingTypeIsInt
namespace Skateboard3Server.Blaze;

public enum BlazeComponent : ushort
{
    Authentication = 0x1,
    GameManager = 0x4,
    Redirector = 0x5,
    Stats = 0x7,
    Util = 0x9,
    Teams = 0xB, //Clubs in other blaze server implementations 
    SkateStats = 0xC, //Game Report in other blaze server implementations
    Social = 0x19, //Association Lists in other blaze server implementations
    UserSession = 0x7802,
}

public enum AuthenticationCommand : ushort
{
    Dlc = 0x20,
    Login = 0xC8, //TODO: might be just login for ps3?
    SessionData = 0xE6
}

public enum GameManagerCommand : ushort
{
    SetGameState = 0x03,
    SetGameSettings = 0x04,
    SetGameAttributes = 0x07,
    RemovePlayer = 0x0B,
    StartMatchmaking = 0x0D,
    FinalizeGameCreation = 0x0F,
    CreateGame = 0x19,
    UpdateGameSession = 0x1A,
    UpdatePlayerConnection = 0x1D,
}

public enum GameManagerNotification : ushort
{
    MatchmakingFinished = 0x0A,
    MatchmakingStatus = 0x0C,
    GameSetup = 0x14,
    PlayerJoinCompleted = 0x1E,
    PlayerRemoved = 0x28,
    GameAttributeChange = 0x50,
    GameStateChange = 0x64,
    GameSettingsChange = 0x6E,
}

public enum RedirectorCommand : ushort
{
    ServerInfo = 0x1
}

public enum StatsCommand : ushort
{
    //TODO 0x02 UpdateStats?
}

public enum UtilCommand : ushort
{
    Ping = 0x2,
    PreAuth = 0x7,
    PostAuth = 0x8,
    ClientMetrics = 0x16,
}

public enum TeamsCommand : ushort
{
    TeamMembership = 0xA8C,
    TeamInvitations = 0x640,
    TeamMembers = 0x7D0
}

public enum SkateStatsCommand : ushort
{
    UpdateStats = 0x2,
}

public enum SkateStatsNotification : ushort
{
    StatsReport = 0x72
}

public enum SocialCommand : ushort
{
    //TODO 0x1
    FriendsList = 0x6
}

public enum UserSessionCommand : ushort
{
    HardwareFlags = 0x8,
    LookupUsers = 0x0D,
    NetworkInfo = 0x14
}

public enum UserSessionNotification : ushort
{
    UserExtendedData = 0x1,
    UserAdded = 0x2,
    UserRemoved = 0x3,
}

public enum BlazeMessageType
{
    Message,
    Reply,
    Notification,
    ErrorReply
}

public enum TdfType
{
    // TDF Type  /  C# Type
    Struct = 0x0,   //class
    String = 0x1,   //string
    Int8 = 0x2,     //bool
    UInt8 = 0x3,    //byte
    Int16 = 0x4,    //short
    UInt16 = 0x5,   //ushort
    Int32 = 0x6,    //int
    UInt32 = 0x7,   //uint
    Int64 = 0x8,    //long
    UInt64 = 0x9,   //ulong
    Array = 0xa,    //List<T>
    Blob = 0xb,     //byte[]
    Map = 0xc,      //Dictionary<T,T>
    Union = 0xd,    //KeyValuePair<T,T>
}

public enum FirstPartyIdType : byte
{
    Ps3 = 0x0,
    Xbox = 0x1
}

public enum NetworkAddressType : byte
{
    Server = 0x0,
    Client = 0x1,
    Pair = 0x2,
    IpAddress = 0x3,
    HostnameAddress = 0x4,
    Unset = 0x7F
}

public enum ExternalIdType : int
{
    Unknown = 0x0,
    Xbox = 0x1,
    Ps3 = 0x2
};

public enum NatType : int
{
    Open,
    Moderate,
    Sequential,
    Strict,
    Unknown
}

public enum MatchmakingResult : int
{
    CreatedGame = 0x0,
    JoinedNewGame = 0x1,
    JoinedExistingGame = 0x2,
    TimedOut = 0x3,
    Cancelled = 0x4,
    Terminated = 0x5,
    Error = 0x6
}

public enum GameState : int
{
    Init = 0x1,
    InGame = 0x82
}

public enum PlayerState : int
{
    Disconnected = 0x0,
    Connecting = 0x2,
    Connected = 0x4,
}

public enum PlayerRemoveReason : int
{
    JoinTimeout = 0x0,
    ConnectionLost = 0x01,
    Leave = 0x06,
}

public enum VoipTopology : int
{
    Disabled = 0x0,
    Host = 0x1, //TODO: dirtybot relay?
    PeerToPeer = 0x2,
}

public enum NetworkTopology : int //TODO nat type? figure out what the maps to on the qos endpoint
{
    PeerToPeerFullMesh = 0x82, //TODO: is this the right name? 
    PeerToPeerPartialMesh = 0x83, //TODO: is this the right name?
    PeerToPeerDirtyCastFailover = 0x84 //TODO: is this the right name?
}

public enum UserLookupType : int
{
    UserId = 0x0,
    PersonaName = 0x1,
    ExternalId = 0x2,
}