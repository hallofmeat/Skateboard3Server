﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Serializer;

public interface IBlazeSerializer
{
    void Serialize(Stream output, BlazeResponseMessage payload);
}

public class BlazeSerializer : IBlazeSerializer
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public void Serialize(Stream output, BlazeResponseMessage payload)
    {
        var responseSb = new StringBuilder();

        SerializeObjectProperties(output, payload, responseSb);

        Logger.Trace($"Response generated:{Environment.NewLine}{responseSb}");
    }
    
    //Public for testing
    public void SerializeObjectProperties(Stream output, object target, StringBuilder responseSb)
    {
        var metadata = TdfHelper.GetTdfMetadata(target.GetType());
        foreach (var meta in metadata.Values)
        {
            var propertyType = meta.Property.PropertyType;
            var propertyValue = meta.Property.GetValue(target);
            var tdfData = TdfHelper.GetTdfTypeAndLength(propertyType, propertyValue);
            if (!tdfData.HasValue)
            {
                responseSb.AppendLine($"{meta.Attribute.Tag} null");
                continue;
            }

            TdfHelper.WriteTag(output, meta.Attribute.Tag);
            TdfHelper.WriteTypeAndLength(output, tdfData.Value.Type, tdfData.Value.Length);

            responseSb.AppendLine($"{meta.Attribute.Tag} {tdfData.Value.Type} {tdfData.Value.Length}");

            SerializeType(output, propertyType, propertyValue, responseSb);
        }
    }

    public void SerializeType(Stream output, Type propertyType, object? propertyValue, StringBuilder responseSb)
    {
        //handle enum types
        if (propertyType.IsEnum)
        {
            propertyType = Enum.GetUnderlyingType(propertyType);
        }

        //type handling
        if (propertyType == typeof(string)) //string
        {
            var value = (string?)propertyValue;
            responseSb.AppendLine($"{value}");
            //TODO: double check if utf8
            if (value != null)
            {
                output.Write(Encoding.ASCII.GetBytes(value));
            }
            output.WriteByte(0x0); //terminate string
        }
        else if (propertyType == typeof(bool) && propertyValue != null) //Int8 (bool)
        {
            var value = (bool)propertyValue;
            responseSb.AppendLine($"{Convert.ToByte(value)}");
            output.WriteByte(Convert.ToByte(value));
        }
        else if (propertyType == typeof(byte) && propertyValue != null) //Uint8
        {
            var value = (byte)propertyValue;
            responseSb.AppendLine($"{value}");
            output.WriteByte(value);
        }
        //TODO: condense int conversion
        else if (propertyType == typeof(short) && propertyValue != null) //Int16
        {
            var value = (short)propertyValue;
            responseSb.AppendLine($"{value}");
            var valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes); //big endian
            output.Write(valueBytes);
        }
        else if (propertyType == typeof(ushort) && propertyValue != null) //Uint16
        {
            var value = (ushort)propertyValue;
            responseSb.AppendLine($"{value}");
            var valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes); //big endian
            output.Write(valueBytes);
        }
        else if (propertyType == typeof(int) && propertyValue != null) //Int32
        {
            var value = (int)propertyValue;
            responseSb.AppendLine($"{value}");
            var valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes); //big endian
            output.Write(valueBytes);
        }
        else if (propertyType == typeof(uint) && propertyValue != null) //Uint32
        {
            var value = (uint)propertyValue;
            responseSb.AppendLine($"{value}");
            var valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes); //big endian
            output.Write(valueBytes);
        }
        else if (propertyType == typeof(long) && propertyValue != null) //Int64
        {
            var value = (long)propertyValue;
            responseSb.AppendLine($"{value}");
            var valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes); //big endian
            output.Write(valueBytes);
        }
        else if (propertyType == typeof(ulong) && propertyValue != null) //Uint64
        {
            var value = (ulong)propertyValue;
            responseSb.AppendLine($"{value}");
            var valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes); //big endian
            output.Write(valueBytes);
        }
        else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>)) //Array
        {
            var listValues = (ICollection?)propertyValue;
            if (listValues == null) //Should not be null because we dont write the tag and continue if its null
            {
                throw new Exception($"Got null value type for {propertyType}");
            }

            TdfHelper.WriteLength(output, Convert.ToUInt32(listValues.Count));

            //TODO: double check this works with strings
            var listValueType = propertyType.GetGenericArguments()[0];
            var index = 0;
            foreach (var item in listValues)
            {
                var tdfData = TdfHelper.GetTdfTypeAndLength(listValueType, item);
                if (!tdfData.HasValue)
                {
                    continue;
                }
                //first value
                if (index == 0)
                {
                    TdfHelper.WriteTypeAndLength(output, tdfData.Value.Type, tdfData.Value.Length);
                    responseSb.AppendLine($"{tdfData.Value.Type} {tdfData.Value.Length} {listValues.Count}");
                    SerializeType(output, listValueType, item, responseSb);
                }
                else
                {
                    if (tdfData.Value.Type == TdfType.String || tdfData.Value.Type == TdfType.Blob)
                    {
                        TdfHelper.WriteLength(output, tdfData.Value.Length);
                    }

                    SerializeType(output, listValueType, item, responseSb);
                }

                index++;
            }
        }
        else if (propertyType == typeof(byte[])) //Blob
        {
            var value = (byte[]?)propertyValue;
            responseSb.AppendLine("<blob>");
            output.Write(value);
        }
        else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) //Map
        {
            var mapKeyType = propertyType.GetGenericArguments()[0];
            var mapValueType = propertyType.GetGenericArguments()[1];

            var mapValues = (ICollection?) propertyValue;

            if (mapValues == null) //Should not be null because we dont write the tag and continue if its null
            {
                throw new Exception($"Got null value type for {mapValues}");
            }

            var index = 0;
            foreach (var item in mapValues)
            {
                var keyValue = item.GetType().GetProperty("Key")?.GetValue(item);
                var valueValue = item.GetType().GetProperty("Value")?.GetValue(item);

                var keyTdfData = TdfHelper.GetTdfTypeAndLength(mapKeyType, keyValue);
                var valueTdfData = TdfHelper.GetTdfTypeAndLength(mapValueType, valueValue);

                if (!keyTdfData.HasValue || !valueTdfData.HasValue)
                {
                    continue;
                }

                //first value
                if (index == 0)
                {
                    //1F 06
                    if (keyTdfData.Value.Type != TdfType.Struct)
                    {
                        TdfHelper.WriteType(output, keyTdfData.Value.Type);
                        TdfHelper.WriteLength(output, keyTdfData.Value.Length);
                    }
                    else
                    {
                        TdfHelper.WriteTypeAndLength(output, keyTdfData.Value.Type, keyTdfData.Value.Length);
                    }
                    responseSb.AppendLine($"{keyTdfData.Value.Type} {keyTdfData.Value.Length}");
                    SerializeType(output, mapKeyType, keyValue, responseSb);

                    if (valueTdfData.Value.Type != TdfType.Struct)
                    {
                        TdfHelper.WriteType(output, valueTdfData.Value.Type);
                        TdfHelper.WriteLength(output, valueTdfData.Value.Length);
                    }
                    else
                    {
                        TdfHelper.WriteTypeAndLength(output, valueTdfData.Value.Type, valueTdfData.Value.Length);
                    }
                    responseSb.AppendLine($"{valueTdfData.Value.Type} {valueTdfData.Value.Length}");
                    SerializeType(output, mapValueType, valueValue, responseSb);
                }
                else
                {
                    if (keyTdfData.Value.Type == TdfType.String || keyTdfData.Value.Type == TdfType.Blob)
                    {
                        TdfHelper.WriteLength(output, keyTdfData.Value.Length);
                    }
                    responseSb.AppendLine($"{keyTdfData.Value.Type} {keyTdfData.Value.Length}");
                    SerializeType(output, mapKeyType, keyValue, responseSb);

                    if (valueTdfData.Value.Type == TdfType.String || valueTdfData.Value.Type == TdfType.Blob)
                    {
                        TdfHelper.WriteLength(output, valueTdfData.Value.Length);
                    }
                    responseSb.AppendLine($"{valueTdfData.Value.Type} {valueTdfData.Value.Length}");
                    SerializeType(output, mapValueType, valueValue, responseSb);
                }
                index++;
            }
        }
        else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) //Union
        {
            var keyValue = propertyType.GetProperty("Key")?.GetValue(propertyValue);
            var valueValue = propertyType.GetProperty("Value")?.GetValue(propertyValue);

            if (keyValue == null)
            {
                throw new Exception("Key value is null");
            }

            output.WriteByte((byte)keyValue);
            responseSb.AppendLine($"{(byte)keyValue}");
            if (valueValue != null) //VALU is not set if value is null
            {
                var unionValueType = propertyType.GetGenericArguments()[1];
                var tdfData = TdfHelper.GetTdfTypeAndLength(unionValueType, valueValue);
                if (!tdfData.HasValue)
                {
                    return;
                }
                TdfHelper.WriteTag(output, "VALU");
                TdfHelper.WriteTypeAndLength(output, tdfData.Value.Type, tdfData.Value.Length);
                responseSb.AppendLine($"{tdfData.Value.Type} {tdfData.Value.Length}");

                SerializeType(output, unionValueType, valueValue, responseSb);
            }
        }
        else if (propertyType.IsClass) //Struct
        {
            if (propertyValue != null)
            {
                responseSb.AppendLine($"<start struct>");
                SerializeObjectProperties(output, propertyValue, responseSb);
                output.WriteByte(0x0); //terminate struct
                responseSb.AppendLine($"<end struct>");
            }
        }
    }

}