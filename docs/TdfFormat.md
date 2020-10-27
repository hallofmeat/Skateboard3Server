# TDF Format

The TDF format is a tag based binary format used by the Blaze server. It resembles other tag based formats like [NBT](https://minecraft.gamepedia.com/NBT_format).

## Data Types

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
//TODO: format rules for the different types

## Packet Format

### Header

Header is always 12 bytes, length of body does not include the length of the header.

| `03 F9`        | `00 09`              | `00 08`                | `00 00`        | `10 00 00 03`                               |
| -------------- | -------------------- | ---------------------- | -------------- | ------------------------------------------- |
| Length of body | Component 0x9 (Util) | Command 0x8 (PostAuth) | Error Code 0x0 | MessageType 0x3 (Response), MessageId (0x1) |

### Body

| `CE B9 79` | `1F 35`       | `31 32 34 2C 31 32 37 2E 30 2E 30 2E 31 3A 38 39 39 39 2C 73 6B 61 74 65 2D 32 30 31 30 2D 70 73 33 2C 31 30 2C 35 30 2C 35 30 2C 35 30 2C 35 30 2C 30 2C 30 00` |
| ---------- | ------------- | ------------------------------------------------------------ |
| Tag (SKEY) | Type / Length | 124,127.0.0.1:8999,skate-2010-ps3,10,50,50,50,50,0,0         |



#### Tag

Tag uses 3 bytes to contain a four character tag name
//TODO: explain conversion

* `92 9C E1`
  * DISA
* `CE B9 79`
  * SKEY

#### Type / Length

Type and Length are put into one byte, unless the length is longer than 0xE than it stored in multiple bytes as a [variable length quantity](https://en.wikipedia.org/wiki/Variable-length_quantity).

* `1F 3E`
  * Type: String
  * Length: 0x3E
* `74`
  * Type: UInt32
  * Length: 0x4

#### Value

Read the Length of bytes and interpreted by the Type.