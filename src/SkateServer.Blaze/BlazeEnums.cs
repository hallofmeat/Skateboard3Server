namespace SkateServer.Blaze
{
    public enum BlazeComponent
    {
        Redirector = 5,
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
        Struct = 0x0,
        String = 0x1,
        Int8 = 0x2,
        Uint8 = 0x3,
        Int16 = 0x4,
        Uint16 = 0x5,
        Int32 = 0x6,
        Uint32 = 0x7,
        Int64 = 0x8,
        Uint64 = 0x9,
        Array = 0xa,
        Blob = 0xb,
        Map = 0xc,
        Union = 0xd,
    }
}
