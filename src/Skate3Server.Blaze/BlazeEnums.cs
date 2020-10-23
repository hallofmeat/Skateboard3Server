namespace Skate3Server.Blaze
{
    //0x01, 0x04, 0x07, 0x09, 0x0B, 0x0C, 0x0F, 0x19, 0x7800, 0x7802, 0x7803

    public enum BlazeComponent : ushort
    {
        Authentication = 0x1,
        Unknown04 = 0x4,
        Redirector = 0x5,
        Unknown07 = 0x7,
        Unknown08 = 0x8,
        Util = 0x9,
        Unknown0B = 0xB,
        Stats = 0xC,
        Unknown0F = 0xF,
        Social = 0x19, //Might be metadata?
        Unknown7800 = 0x7800,
        UserSession = 0x7802,
        Unknown7803 = 0x7803,
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
