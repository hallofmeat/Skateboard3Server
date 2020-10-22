using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NLog;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Serializer
{
    public interface IBlazeSerializer
    {
        void Serialize(Stream output, BlazeHeader requestHeader, BlazeMessageType messageType, object payload);
    }

    public class BlazeSerializer : IBlazeSerializer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Serialize(Stream output, BlazeHeader requestHeader, BlazeMessageType messageType, object payload)
        {
            //Debug
            var responseSb = new StringBuilder();

            //TODO: so many streams
            var bodyStream = new MemoryStream();
            SerializeObjectProperties(bodyStream, payload, responseSb);

            var bodyStreamBytes = bodyStream.ToArray();

            //Header
            var headerStream = new MemoryStream();
            //Length
            var length = BitConverter.GetBytes(Convert.ToUInt16(bodyStreamBytes.Length));
            Array.Reverse(length);//big endian
            headerStream.Write(length);
            //Component
            var component = BitConverter.GetBytes((ushort)requestHeader.Component);
            Array.Reverse(component);//big endian
            headerStream.Write(component);
            //Command
            var command = BitConverter.GetBytes(requestHeader.Command);
            Array.Reverse(command);//big endian
            headerStream.Write(command);
            //ErrorCode
            var errorCode = BitConverter.GetBytes(Convert.ToUInt16(0));
            Array.Reverse(errorCode);//big endian
            headerStream.Write(errorCode);
            //MessageType/MessageId
            var messageData =
                BitConverter.GetBytes((int)messageType << 28 | requestHeader.MessageId);
            Array.Reverse(messageData);//big endian
            headerStream.Write(messageData);
            var headerStreamBytes = headerStream.ToArray();

            //For Debug
            var headerHex = BitConverter.ToString(headerStreamBytes).Replace("-", " ");
            var payloadHex = BitConverter.ToString(bodyStreamBytes).Replace("-", " ");
            Logger.Trace($"{headerHex} {payloadHex}");

            Logger.Debug(
                $"Response ^; Length:{bodyStreamBytes.Length} Component:{requestHeader.Component} Command:{requestHeader.Command} ErrorCode:{requestHeader.ErrorCode} MessageType:{messageType} MessageId:{requestHeader.MessageId}");

            Logger.Trace($"Response generated:{Environment.NewLine}{responseSb}");

            output.Write(headerStreamBytes);
            output.Write(bodyStreamBytes);
        }

        private void SerializeObjectProperties(Stream output, object target, StringBuilder responseSb)
        {
            var metadata = TdfHelper.GetTdfMetadata(target.GetType());
            foreach (var meta in metadata.Values)
            {
                var propertyType = meta.Property.PropertyType;
                var propertyValue = meta.Property.GetValue(target);
                var tdfData = TdfHelper.GetTdfTypeAndLength(propertyType, propertyValue);


                TdfHelper.WriteLabel(output, meta.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, tdfData.Type, tdfData.Length);

                responseSb.AppendLine($"{meta.Attribute.Tag} {tdfData.Type} {tdfData.Length}");

                SerializeType(output, propertyType, propertyValue, responseSb);
            }
        }

        private void SerializeType(Stream output, Type propertyType, object propertyValue, StringBuilder responseSb)
        {
            //handle enum types
            if (propertyType.IsEnum)
            {
                propertyType = Enum.GetUnderlyingType(propertyType);
            }

            //type handling
            if (propertyType == typeof(string)) //string
            {
                var value = (string)propertyValue;
                responseSb.AppendLine($"{value}");
                //TODO: double check utf8
                output.Write(Encoding.UTF8.GetBytes(value));
                output.WriteByte(0x0); //terminate string
            }
            else if (propertyType == typeof(bool)) //Int8 (bool)
            {
                var value = (bool)propertyValue;
                responseSb.AppendLine($"{Convert.ToByte(value)}");
                output.WriteByte(Convert.ToByte(value));
            }
            else if (propertyType == typeof(byte)) //Uint8
            {
                var value = (byte)propertyValue;
                responseSb.AppendLine($"{value}");
                output.WriteByte(value);
            }
            //TODO: condense int conversion
            else if (propertyType == typeof(short)) //Int16
            {
                var value = (short)propertyValue;
                responseSb.AppendLine($"{value}");
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(ushort)) //Uint16
            {
                var value = (ushort)propertyValue;
                responseSb.AppendLine($"{value}");
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(int)) //Int32
            {
                var value = (int)propertyValue;
                responseSb.AppendLine($"{value}");
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(uint)) //Uint32
            {
                var value = (uint)propertyValue;
                responseSb.AppendLine($"{value}");
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(long)) //Int64
            {
                var value = (long)propertyValue;
                responseSb.AppendLine($"{value}");
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(ulong)) //Uint64
            {
                var value = (ulong)propertyValue;
                responseSb.AppendLine($"{value}");
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>)) //Array
            {
                var listValues = (ICollection)propertyValue;
                TdfHelper.WriteLength(output, Convert.ToUInt32(listValues.Count));

                //TODO: I think struct or string will fail here
                var listValueType = propertyType.GetGenericArguments()[0];
                var tdfData = TdfHelper.GetTdfTypeAndLength(listValueType, null);
                TdfHelper.WriteTypeAndLength(output, tdfData.Type, tdfData.Length);
                responseSb.AppendLine($"{tdfData.Type} {tdfData.Length} {listValues.Count}");

                foreach (var item in listValues)
                {
                    SerializeType(output, listValueType, item, responseSb);
                }
            }
            else if (propertyType == typeof(byte[])) //Blob
            {
                var value = (byte[])propertyValue;
                responseSb.AppendLine("<blob>");
                output.Write(value);
            }
            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) //Map
            {
                var mapKeyType = propertyType.GetGenericArguments()[0];
                var mapValueType = propertyType.GetGenericArguments()[1];

                var mapValues = (ICollection) propertyValue;

                var index = 0;
                foreach (var item in mapValues)
                {
                    //TODO: fix getting values
                    var keyValue = item.GetType().GetProperty("Key")?.GetValue(item);
                    var valueValue = item.GetType().GetProperty("Value")?.GetValue(item);

                    var keyTdfData = TdfHelper.GetTdfTypeAndLength(mapKeyType, keyValue);
                    var valueTdfData = TdfHelper.GetTdfTypeAndLength(mapValueType, valueValue);

                    //first value
                    if (index == 0)
                    {
                        //1F 06
                        if (keyTdfData.Type != TdfType.Struct)
                        {
                            TdfHelper.WriteType(output, keyTdfData.Type);
                            TdfHelper.WriteLength(output, keyTdfData.Length);
                        }
                        else
                        {
                            TdfHelper.WriteTypeAndLength(output, keyTdfData.Type, keyTdfData.Length);
                        }
                        responseSb.AppendLine($"{keyTdfData.Type} {keyTdfData.Length}");
                        SerializeType(output, mapKeyType, keyValue, responseSb);

                        if (valueTdfData.Type != TdfType.Struct)
                        {
                            TdfHelper.WriteType(output, valueTdfData.Type);
                            TdfHelper.WriteLength(output, valueTdfData.Length);
                        }
                        else
                        {
                            TdfHelper.WriteTypeAndLength(output, valueTdfData.Type, valueTdfData.Length);
                        }
                        responseSb.AppendLine($"{valueTdfData.Type} {valueTdfData.Length}");
                        SerializeType(output, mapValueType, valueValue, responseSb);
                    }
                    else
                    {
                        if (keyTdfData.Type != TdfType.Struct)
                        {
                            TdfHelper.WriteLength(output, keyTdfData.Length);
                        }
                        responseSb.AppendLine($"{keyTdfData.Type} {keyTdfData.Length}");
                        SerializeType(output, mapKeyType, keyValue, responseSb);

                        if (valueTdfData.Type != TdfType.Struct)
                        {
                            TdfHelper.WriteLength(output, valueTdfData.Length);
                        }
                        responseSb.AppendLine($"{valueTdfData.Type} {valueTdfData.Length}");
                        SerializeType(output, mapValueType, valueValue, responseSb);
                    }
                    index++;
                }
            }
            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) //Union
            {
                var keyValue = (byte) propertyType.GetProperty("Key")?.GetValue(propertyValue);
                var valueValue = propertyType.GetProperty("Value")?.GetValue(propertyValue);
                var unionValueType = propertyType.GetGenericArguments()[1];
                var tdfData = TdfHelper.GetTdfTypeAndLength(unionValueType, valueValue);

                output.WriteByte(keyValue);
                TdfHelper.WriteLabel(output, "VALU");
                TdfHelper.WriteTypeAndLength(output, tdfData.Type, tdfData.Length);
                responseSb.AppendLine($"{keyValue}");
                responseSb.AppendLine($"{tdfData.Type} {tdfData.Length}");

                SerializeType(output, unionValueType, valueValue, responseSb);
            }
            else if (propertyType.IsClass) //Struct?
            {
                responseSb.AppendLine($"<start struct>");
                SerializeObjectProperties(output, propertyValue, responseSb);
                output.WriteByte(0x0); //terminate struct
                responseSb.AppendLine($"<end struct>");
            }
        }

    }

    public class SerializerTdfMetadata
    {
        public PropertyInfo Property { get; set; }
        public TdfFieldAttribute Attribute { get; set; }
    }
}