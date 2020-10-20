using System.Buffers.Binary;
using System.IO;

namespace Skate3Server.Common
{
    public static class BinaryReaderExtensions
    {
        public static short ReadInt16Be(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt16BigEndian(reader.ReadBytes(2));
        }

        public static ushort ReadUInt16Be(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2));
        }

        public static int ReadInt32Be(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
        }

        public static uint ReadUInt32Be(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadUInt32BigEndian(reader.ReadBytes(4));
        }

        public static long ReadInt64Be(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt64BigEndian(reader.ReadBytes(8));
        }

        public static ulong ReadUInt64Be(this BinaryReader reader)
        {
            return BinaryPrimitives.ReadUInt64BigEndian(reader.ReadBytes(8));
        }

    }
}
