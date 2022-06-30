using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Skateboard3Server.Blaze.Serializer;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Skateboard3Server.Blaze.Tests.Serializer;

public class BlazeSerializerFacts
{
    [Fact]
    public void Generates_basic_types()
    {
        var input = new TestBasicTypesBlaze
        {
            StringTest = "testing",
            BoolTest = true,
            ByteTest = 128,
            ShortTest = 1234,
            UShortTest = 1234,
            IntTest = 1234,
            UIntTest = 1234,
            LongTest = 1234,
            ULongTest = 1234,
            EnumTest = TestBlazeEnum.World
        };

        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());

        //Assert
        var validBody = new byte[]
        {
            0xd3, 0x3d, 0x21, //TSTA
            0x18, //string, 8
            0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x00, //testing
            0xd3, 0x3d, 0x22, //TSTB
            0x21, //int8, 1
            0x01, //1
            0xd3, 0x3d, 0x23, //TSTC
            0x31, //uint8, 1
            0x80,
            0xd3, 0x3d, 0x24, //TSTD
            0x42, //int16, 2
            0x04, 0xd2, //1234
            0xd3, 0x3d, 0x25, //TSTE
            0x52, //uint16, 2
            0x04, 0xd2, //1234
            0xd3, 0x3d, 0x26, //TSTF
            0x64, //int32, 4
            0x00, 0x00, 0x04, 0xd2, //1234
            0xd3, 0x3d, 0x27, //TSTG
            0x74, //uint32, 4
            0x00, 0x00, 0x04, 0xd2, //1234
            0xd3, 0x3d, 0x28, //TSTH
            0x88, //int64, 8
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xd2, //1234
            0xd3, 0x3d, 0x29, //TSTI
            0x98, //uint64, 8
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xd2, //1234
            0xd3, 0x3d, 0x2a, //TSTJ
            0x52, //ushort16, 2
            0x00, 0x02, //2

        };
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(validBody);
    }

    [Fact]
    public void Generates_array_strings()
    {
        var input = new TestArrayStrings
        {
            ListTest = new List<string>
            {
                "Test1",
                "areallylongstring",
                "testing"
            }
        };

        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());

            
        //Assert
        var validBody = new byte[]
        {
            0xd3, 0x3d, 0x21, //TSTA
            0xa1,  //array, 1 dimension
            0x03,  // 3 elements
            0x16,  // string, length 6
            0x54, 0x65, 0x73, 0x74, 0x31, 0x00, //Test1
            0x12, //18 length 
            0x61, 0x72, 0x65, 0x61, 0x6C, 0x6C, 0x79, 0x6C, 0x6F, 0x6E, 0x67, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x00, //areallylongstring
            0x08, //8 length
            0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x00,
        };
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(validBody);
    }

    [Fact]
    public void Generate_array_null_serialize_empty()
    {
        var input = new TestArrayStrings
        {
            ListTest = null
        };
        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());


        //Assert
        var result = resultStream.ToArray();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Generates_array_ints()
    {
        var input = new TestArrayInts
        {
            ListTest = new List<int>
            {
                1234,
                10000,
            }
        };

        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());


        //Assert
        var validBody = new byte[]
        {
            0xd3, 0x3d, 0x21, //TSTA
            0xa1,  //array, 1 dimension
            0x02,  // 2 elements
            0x64,  // int, length 4
            0x00, 0x00, 0x04, 0xD2, //1234
            0x00, 0x00, 0x27, 0x10 //10000
        };
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(validBody);
    }

    [Fact]
    public void Generates_map_strings()
    {
        var input = new TestMapStrings
        {
            MapTest = new Dictionary<int, string>
            {
                {1, "Test1"},
                {2, "areallylongstring"},
                {3, "testing"}
            }
        };

        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());


        //Assert
        var validBody = new byte[]
        {
            0xd3, 0x3d, 0x21, //TSTA
            0xc3,  //map, 3 elements
            0x6f, 0x04, //int length 4,
            0x00, 0x00, 0x00, 0x01,
            0x1F, 0x06,  // string, length 6
            0x54, 0x65, 0x73, 0x74, 0x31, 0x00, //Test1
            0x00, 0x00, 0x00, 0x02,
            0x12, //18 length 
            0x61, 0x72, 0x65, 0x61, 0x6C, 0x6C, 0x79, 0x6C, 0x6F, 0x6E, 0x67, 0x73, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x00, //areallylongstring
            0x00, 0x00, 0x00, 0x03,
            0x08, //8 length
            0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x00,
        };
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(validBody);
    }

    [Fact]
    public void Generate_map_null_serialize_empty()
    {
        var input = new TestMapStrings
        {
            MapTest = null
        };

        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());


        //Assert
        var result = resultStream.ToArray();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Generates_map_structs()
    {
        var input = new TestMapStructs
        {
            MapTest = new Dictionary<string, TestStruct>
            {
                {"a", new TestStruct
                {
                    StringTest = "testing1"
                }},
                {"b", new TestStruct
                {
                    StringTest = "testing2"
                }}
            }
        };

        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());


        //Assert
        var validBody = new byte[]
        {
            0xd3, 0x3d, 0x21, //TSTA
            0xc2,  //map, 2 elements
            0x1f, 0x02, //string length 2
            0x61, 0x00, //a
            0x00, // struct
            0xd3, 0x3d, 0x21, //TSTA
            0x19, //string length 9
            0x74, 0x65, 0x73, 0x74, 0x69, 0x6E, 0x67, 0x31, 0x00, //testing1
            0x00, //end struct
            0x02, //string, length 2
            0x62, 0x00, //b
            0xd3, 0x3d, 0x21, //TSTA
            0x19, //string length 9
            0x74, 0x65, 0x73, 0x74, 0x69, 0x6E, 0x67, 0x32, 0x00, //testing2
            0x00, //end struct
        };
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(validBody);
    }

    [Fact]
    public void Generates_nested_structs()
    {
        var input = new TestNestedStructs
        {
            NestedStruct = new NestedStruct1
            {
                NestedStruct = new TestStruct
                {
                    StringTest = "testing"
                }
            }
        };

        var serial = new BlazeSerializer();
        var resultStream = new MemoryStream();

        //Act
        serial.SerializeObjectProperties(resultStream, input, new StringBuilder());


        //Assert
        var validBody = new byte[]
        {
            0xd3, 0x3d, 0x21, //TSTA
            0x00, //NestedStruct1
            0xd3, 0x3d, 0x21, //TSTA
            0x00, //TestStruct
            0xd3, 0x3d, 0x21, //TSTA
            0x18, //string length 8
            0x74, 0x65, 0x73, 0x74, 0x69, 0x6E, 0x67, 0x00, //testing
            0x00, //end TestStruct
            0x00, //end NestedStruct1
        };
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(validBody);
    }

    //TODO: test union (pending refactor)
    //TODO: test empty struct

    private class TestArrayStrings
    {
        [TdfField("TSTA")]
        public List<string> ListTest { get; set; }
    }

    private class TestArrayInts
    {
        [TdfField("TSTA")]
        public List<int> ListTest { get; set; }
    }

    private class TestMapStrings
    {
        [TdfField("TSTA")]
        public Dictionary<int, string> MapTest { get; set; }
    }

    private class TestMapStructs
    {
        [TdfField("TSTA")]
        public Dictionary<string, TestStruct> MapTest { get; set; }
    }

    private class TestNestedStructs
    {
        [TdfField("TSTA")]
        public NestedStruct1 NestedStruct { get; set; }

    }

    private class NestedStruct1
    {
        [TdfField("TSTA")]
        public TestStruct NestedStruct { get; set; }
    }

    private class TestStruct
    {
        [TdfField("TSTA")]
        public string StringTest { get; set; }
    }


    private class TestBasicTypesBlaze
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

    private enum TestBlazeEnum : ushort
    {
        Hello = 0x1,
        World = 0x2
    }
}