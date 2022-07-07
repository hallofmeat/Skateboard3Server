using System.Collections.Generic;
using Skateboard3Server.Blaze.Serializer.Attributes;

namespace Skateboard3Server.Blaze.Tests.Serializer.Testing;

internal record TestByteArray
{
    [TdfField("TSTA")]
    public byte[] ByteArrayTest { get; set; }
}

internal record TestArrayStrings
{
    [TdfField("TSTA")]
    public List<string> ListTest { get; set; }
}

internal record TestArrayInts
{
    [TdfField("TSTA")]
    public List<int> ListTest { get; set; }
}

internal record TestMapStrings
{
    [TdfField("TSTA")]
    public Dictionary<int, string> MapTest { get; set; }
}

internal record TestMapStructs
{
    [TdfField("TSTA")]
    public Dictionary<string, TestStruct> MapTest { get; set; }
}

internal record TestNestedStructs
{
    [TdfField("TSTA")]
    public NestedStruct1 NestedStruct { get; set; }

}

internal record NestedStruct1
{
    [TdfField("TSTA")]
    public TestStruct NestedStruct { get; set; }
}

internal record TestStruct
{
    [TdfField("TSTA")]
    public string StringTest { get; set; }
}


internal record TestBasicTypesBlaze
{
    [TdfField("TSTA")]
    public string StringTest { get; set; }

    [TdfField("TSTB")]
    public bool BoolTest { get; set; }

    [TdfField("TSTC")]
    public byte ByteTest { get; set; }

    [TdfField("TSTD")]
    public short ShortTest { get; set; }

    [TdfField("TSTE")]
    public ushort UShortTest { get; set; }

    [TdfField("TSTF")]
    public int IntTest { get; set; }

    [TdfField("TSTG")]
    public uint UIntTest { get; set; }

    [TdfField("TSTH")]
    public long LongTest { get; set; }

    [TdfField("TSTI")]
    public ulong ULongTest { get; set; }

    [TdfField("TSTJ")]
    public TestBlazeEnum EnumTest { get; set; }
}

internal enum TestBlazeEnum : ushort
{
    Hello = 0x1,
    World = 0x2
}