using System;
using System.Buffers;
using System.IO;
using System.Text.RegularExpressions;

namespace SkateServer.Blaze
{
    public static class TdfHelper
    {
        //TODO: test
        public static string ParseLabel(ref SequenceReader<byte> reader)
        {
            //Storing a 4 character string in 3 bytes

            var label = "";
            var tagBytes = new byte[4];
            reader.TryRead(out tagBytes[0]);
            reader.TryRead(out tagBytes[1]);
            reader.TryRead(out tagBytes[2]);
            //leave the last one empty

            //convert endianness
            Array.Reverse(tagBytes);
            var tag = BitConverter.ToUInt32(tagBytes, 0) >> 8;

            //Convert to string
            for (var i = 0; i < tagBytes.Length; ++i)
            {
                var val = (tag >> ((3 - i) * 6)) & 0x3F;
                if (val > 0)
                {
                    label += Convert.ToChar(0x40 | (val & 0x1F));
                }
            }

            //cleanup
            label = Regex.Replace(label, "[^A-Z]+", "");

            return label;
        }

        public static uint ParseLength(ref SequenceReader<byte> reader)
        {
            //Length is a https://en.wikipedia.org/wiki/Variable-length_quantity

            var index = 0;
            uint buffer = 0;
            byte current;
            do
            {
                if (index++ == 8) throw new FormatException("Could not read variable-length, too long?");

                buffer <<= 7;

                if (!reader.TryRead(out current))
                    throw new FormatException("Could not read variable-length, too short?");

                buffer |= current & 0x7FU;
            } while ((current & 0x80) != 0);

            return buffer;
        }

        public static Tuple<TdfType, uint> ParseTypeAndLength(ref SequenceReader<byte> reader)
        {
            //First half of the byte is the type, second half is the length (if its 0xF then read the variable length)

            reader.TryRead(out byte typeData);

            var type = typeData >> 4;
            var lengthByte = (uint)typeData & 0x0F;
            
            var length = lengthByte == 0xF ? ParseLength(ref reader) : lengthByte;
            return new Tuple<TdfType, uint>((TdfType)type, length);
        }

        public static void WriteLabel(Stream output, string label)
        {
            //Storing a 4 character string in 3 bytes

            var tag = 0;

            for (var i = 0; i < label.Length; i++)
            {
                tag |= (0x20 | (label[i] & 0x1F)) << (3 - i) * 6;
            }

            var tagBytes = BitConverter.GetBytes(Convert.ToUInt32(tag));
            //shrink and convert endianness
            Array.Resize(ref tagBytes, 3);
            Array.Reverse(tagBytes);
            
            output.Write(tagBytes);
        }

        public static void WriteLength(Stream output, uint length)
        {
            //Length is a https://en.wikipedia.org/wiki/Variable-length_quantity

            if (length > Math.Pow(2, 56))
                throw new OverflowException("Could not write variable-length, exceeds max value");

            var index = 3;
            var significantBitReached = false;
            var mask = 0x7fUL << (index * 7);
            while (index >= 0)
            {
                var buffer = (mask & length);
                if (buffer > 0 || significantBitReached)
                {
                    significantBitReached = true;
                    buffer >>= index * 7;
                    if (index > 0)
                        buffer |= 0x80;
                    output.WriteByte((byte)buffer);
                }
                mask >>= 7;
                index--;
            }

            if (!significantBitReached)
                output.WriteByte(new byte());
        }

        public static void WriteTypeAndLength(Stream output, TdfType type, uint length)
        {
            //TODO

        }

    }
}