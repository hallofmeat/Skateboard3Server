using System.IO;
using System.Text;
using FluentAssertions;
using Skate3Server.Blaze.Serializer;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;
using Xunit;

namespace Skate3Server.Blaze.Tests.Serializer
{
    public class BlazeSerializerFacts
    {
        //[Fact]
        //public void Generates_header()
        //{
        //    var blazeHeader = new BlazeHeader
        //    {
        //        Component = BlazeComponent.Redirector,
        //        Command = 0x1,
        //        ErrorCode = 0x12,
        //        MessageType = BlazeMessageType.Reply,
        //        MessageId = 100,
        //    };

        //    var serial = new BlazeSerializer();
        //    var resultStream = new MemoryStream();

        //    //Act
        //    serial.Serialize(resultStream, new TestEmptyBlaze());

        //    //Assert
        //    var validHeader = new byte[]
        //    {
        //        0x00, 0x00, //Length
        //        0x00, 0x05, //Component
        //        0x00, 0x01, //Command
        //        0x00, 0x12, //Error Code
        //        0x10, 0x00, 0x00, 0x64 //MessageType/MessageId
        //    };

        //    resultStream.ToArray().Should().BeEquivalentTo(validHeader);
        //}


        [Fact]
        public void Generates_basic_types()
        {
            var basicTypes = new TestBasicTypesBlaze
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
            serial.SerializeObjectProperties(resultStream, basicTypes, new StringBuilder());

            //Assert
            var validHeader = new byte[]
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

            resultStream.ToArray().Should().BeEquivalentTo(validHeader);
        }


        public class TestBasicTypesBlaze
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


        public class TestEmptyBlaze
        {
        }

        public enum TestBlazeEnum : ushort
        {
            Hello = 0x1,
            World = 0x2
        }
    }
}
