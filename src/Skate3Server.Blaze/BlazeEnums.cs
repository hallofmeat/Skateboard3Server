﻿namespace Skate3Server.Blaze
{
    //0x01, 0x04, 0x07, 0x09, 0x0B, 0x0C, 0x0F, 0x19, 0x7800, 0x7802, 0x7803

    public enum BlazeComponent : ushort
    {
        Authentication = 0x1, //Guess?
        GameManager = 0x4, //Guess?
        Redirector = 0x5,
        Stats = 0x7, //Guess?
        Util = 0x9, //Guess?
        Unknown0B = 0xB,
        Unknown0C = 0xC,
        Unknown0F = 0xF,
        Unknown19 = 0x19,
        Unknown7800 = 0x7800,
        Unknown7802 = 0x7802,
        Unknown7803 = 0x7803,
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
