namespace Skate3Server.Blaze
{
    public enum BlazeComponent : ushort
    {
        Authentication = 0x1,
        GameManager = 0x4,
        Redirector = 0x5,
        Stats = 0x7,
        Util = 0x9,
        Teams = 0xB, //Clubs in other blaze server implementations 
        SkateStats = 0xC, //TODO: not sure if correct
        Social = 0x19, //TODO: Metadata?
        UserSession = 0x7802,
    }

    public enum AuthenticationCommand : ushort
    {
        Dlc = 0x20,
        Login = 0xC8,
        SessionData = 0xE6
    }

    public enum GameManagerCommand : ushort
    {
    }

    public enum RedirectorCommand : ushort
    {
        ServerInfo = 0x1
    }

    public enum StatsCommand : ushort
    {
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
        Unknown640 = 0x640 //TODO I think this is pending invites
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
        FriendsList = 0x6
    }

    public enum UserSessionCommand : ushort
    {
        HardwareFlags = 0x8,
        NetworkInfo = 0x14
    }

    public enum UserSessionNotification : ushort
    {
        UserExtendedData = 0x1,
        UserAdded = 0x2,
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
        PS3 = 0x0,
        Xbox = 0x1
    }

    public enum NetworkAddressType : byte
    {
        Server = 0x0,
        Client = 0x1,
        Pair = 0x2,
        Unset = 0x7F
    }

    public enum ExternalProfileType : int
    {
        Unknown = 0x0,
        Xbox = 0x1,
        PS3 = 0x2
    };

    public enum NatType : int
    {
        Open,
        Moderate,
        Sequential,
        Strict,
        Unknown
    }
}
