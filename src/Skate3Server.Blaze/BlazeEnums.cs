namespace Skate3Server.Blaze
{
    public enum BlazeComponent : ushort
    {
        Redirector = 0x5,
        Authentication = 0x9,
    }

    public enum BlazeRedirectorCommand
    {
        ServerInfo = 0x1,
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
        Int8 = 0x2,     //sbyte
        Uint8 = 0x3,    //byte
        Int16 = 0x4,    //short
        Uint16 = 0x5,   //ushort
        Int32 = 0x6,    //int
        Uint32 = 0x7,   //uint
        Int64 = 0x8,    //long
        Uint64 = 0x9,   //ulong
        Array = 0xa,    //List<T>
        Blob = 0xb,     //byte[]
        Map = 0xc,      //Dictionary<T,T>
        Union = 0xd,    //Custom TODO: make this more intuitive
    }

    public enum FirstPartyIdType : byte
    {
        Ps3 = 0x0,
        Xbox = 0x1
    }

    public enum NetworkAddressType : byte
    {
        Client = 0x0,
        Server = 0x1
    }
}
