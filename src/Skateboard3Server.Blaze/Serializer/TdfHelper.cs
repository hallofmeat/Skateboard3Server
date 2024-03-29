﻿using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Skateboard3Server.Blaze.Serializer.Attributes;

namespace Skateboard3Server.Blaze.Serializer;

public static class TdfHelper
{
    public static string ParseTag(ref SequenceReader<byte> reader)
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

        //convert to string
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

    public static (TdfType Type, uint Length) ParseTypeAndLength(ref SequenceReader<byte> reader)
    {
        //First half of the byte is the type, second half is the length (if its 0xF then read the variable length)

        reader.TryRead(out byte typeData);

        var type = typeData >> 4;
        var lengthByte = (uint)typeData & 0x0F;
            
        var length = lengthByte == 0xF ? ParseLength(ref reader) : lengthByte;
        return ((TdfType)type, length);
    }

    public static void WriteTag(Stream output, string tag)
    {
        //Storing a 4 character string in 3 bytes

        var tagValue = 0;

        for (var i = 0; i < tag.Length; i++)
        {
            tagValue |= (0x20 | (tag[i] & 0x1F)) << (3 - i) * 6;
        }

        var tagBytes = BitConverter.GetBytes(Convert.ToUInt32(tagValue));
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

    public static void WriteType(Stream output, TdfType type)
    {
        output.WriteByte((byte)(((uint)type << 4) | 0xF));
    }

    public static void WriteTypeAndLength(Stream output, TdfType type, uint length)
    {
        //length is 15 or more
        if (length >= 0xF)
        {
            WriteType(output, type);
            WriteLength(output, length);
        }
        else
        {
            output.WriteByte((byte)(((uint)type << 4) | length));
        }
    }

    public static (TdfType Type, uint Length)? GetTdfTypeAndLength(Type type, object? value)
    {
        //Convert enum to base type (ushort/int etc)
        if (type.IsEnum)
        {
            type = Enum.GetUnderlyingType(type);
        }

        if (type == typeof(string)) //string
        {
            var strValue = (string?)value;
            return (TdfType.String, Convert.ToUInt32(strValue?.Length + 1));
        }
        if (type == typeof(bool)) //Int8
        {
            return (TdfType.Int8, 0x1);
        }
        if (type == typeof(byte)) //Uint8
        {
            return (TdfType.UInt8, 0x1);
        }
        if (type == typeof(short)) //Int16
        {
            return (TdfType.Int16, 0x2);
        }
        if (type == typeof(ushort)) //Uint16
        {
            return (TdfType.UInt16, 0x2);
        }
        if (type == typeof(int)) //Int32
        {
            return (TdfType.Int32, 0x4);
        }
        if (type == typeof(uint)) //Uint32
        {
            return (TdfType.UInt32, 0x4);
        }
        if (type == typeof(long)) //Int64
        {
            return (TdfType.Int64, 0x8);
        }
        if (type == typeof(ulong)) //Uint64
        {
            return (TdfType.UInt64, 0x8);
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) //Array
        {
            if (value == null)
            {
                return null; //Dont generate a tag/array for this
            }
            return (TdfType.Array, 0x1); //1 dimension, TODO: handle multidimensional
        }
        if (type == typeof(byte[])) //Blob
        {
            var blobValue = (byte[]?)value;
            uint length = 0;
            if (blobValue != null)
            {
                length = Convert.ToUInt32(blobValue.Length);
            }
            return (TdfType.Blob, length);
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) //Map
        {
            if (value == null)
            {
                return null; //Dont generate a tag/array for this
            }
            var mapValue = (ICollection)value;
            return (TdfType.Map, Convert.ToUInt32(mapValue.Count));
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) //Union
        {
            if (value == null)
            {
                return null; //Dont generate a tag/array for this
            }
            return (TdfType.Union, 0x0);
        }
        if (type.IsClass) //Struct?
        {
            //TODO figure out if this should be empty or not written when value is null
            return (TdfType.Struct, 0x0);
        }

        throw new ArgumentOutOfRangeException($"Unknown TdfType mapping for  {type}");

    }

    public static Dictionary<string, SerializerTdfMetadata> GetTdfMetadata(Type sourceType)
    {
        //Get dictionary for tag -> property/attribute
        //TODO: cache this
        return (from p in sourceType.GetProperties()
            let attributes = p.GetCustomAttributes(typeof(TdfFieldAttribute), true)
            where attributes.Length == 1
            let attr = attributes.Single() as TdfFieldAttribute
            select new SerializerTdfMetadata { Property = p, Attribute = attr }).ToDictionary(key => key.Attribute.Tag);
    }

}