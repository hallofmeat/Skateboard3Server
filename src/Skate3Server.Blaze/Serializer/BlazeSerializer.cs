using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Serializer
{
    public interface IBlazeSerializer
    {
        object Deserialize(ref ReadOnlySequence<byte> payload, Type requestType);
        void Serialize(Stream output, BlazeHeader requestHeader, BlazeMessageType messageType, object payload);
    }

    public class BlazeSerializer : IBlazeSerializer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public object Deserialize(ref ReadOnlySequence<byte> payload, Type requestType)
        {
            var request = Activator.CreateInstance(requestType);

            var requestSb = new StringBuilder();

            var payloadReader = new SequenceReader<byte>(payload);
            var state = new ParserState();

            while (!payloadReader.End)
            {
                ParseObject(ref payloadReader, request, state, requestSb);
            }

            Logger.Trace($"Request parsed:{Environment.NewLine}{requestSb}");

            return request;
        }

        private void ParseObject(ref SequenceReader<byte> payloadReader, object target, ParserState state,
            StringBuilder requestSb)
        {
            //TODO: this whole thing is using a lot of non cached reflection (fix this)
            var tdfMetadata = GetTdfMetadata(target.GetType());

            var tag = TdfHelper.ParseTag(ref payloadReader);
            var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
            
            requestSb.AppendLine($"{tag} {typeData.Type} {typeData.Length}");

            var parsed = ParseType(ref payloadReader, tdfMetadata[tag].Property.PropertyType, typeData.Type, typeData.Length, state, requestSb);
            try
            {
                tdfMetadata[tag].Property.SetValue(target, parsed);
            }
            catch (ArgumentException e)
            {
                Logger.Error($"Failed to set tag {tag}: {e}");
                throw;
            }
        }

        private object ParseType(ref SequenceReader<byte> payloadReader, Type currentType, TdfType type, uint length, ParserState state,
            StringBuilder requestSb)
        {
            //TODO add type checking against currentType
            switch (type)
            {
                case TdfType.Struct:
                    payloadReader.Advance(length);
                    state.StructDepth++;
                    var subTarget = Activator.CreateInstance(currentType);
                    requestSb.AppendLine("<start struct>");
                    do
                    {
                        ParseObject(ref payloadReader, subTarget, state, requestSb);

                    } while (!EndOfStruct(ref payloadReader, state, requestSb));
                    return subTarget;
                case TdfType.String:
                    var byteStr = payloadReader.Sequence.Slice(payloadReader.Position, length);
                    payloadReader.Advance(length);
                    //TODO: figure out if utf8
                    var str = Encoding.UTF8.GetString(byteStr.ToArray()).TrimEnd('\0');
                    requestSb.AppendLine($"{str}");
                    return str;
                case TdfType.Int8: //bool
                    payloadReader.TryRead(out byte int8);
                    requestSb.AppendLine($"{int8}");
                    return Convert.ToBoolean(int8);
                case TdfType.Uint8:
                    payloadReader.TryRead(out byte uint8);
                    requestSb.AppendLine($"{uint8}");
                    return uint8;
                case TdfType.Int16:
                    payloadReader.TryReadBigEndian(out short int16);
                    requestSb.AppendLine($"{int16}");
                    return int16;
                case TdfType.Uint16:
                    payloadReader.TryReadBigEndian(out short uint16);
                    requestSb.AppendLine($"{uint16}");
                    return (ushort) uint16;
                case TdfType.Int32:
                    payloadReader.TryReadBigEndian(out int int32);
                    requestSb.AppendLine($"{int32}");
                    return int32;
                case TdfType.Uint32:
                    payloadReader.TryReadBigEndian(out int uint32);
                    requestSb.AppendLine($"{uint32}");
                    return (uint) uint32;
                case TdfType.Int64:
                    payloadReader.TryReadBigEndian(out long int64);
                    requestSb.AppendLine($"{int64}");
                    return int64;
                case TdfType.Uint64:
                    payloadReader.TryReadBigEndian(out long uint64);
                    requestSb.AppendLine($"{uint64}");
                    return (ulong) uint64;
                case TdfType.Array:
                    //Length is the number of dimensions //TODO: handle multidimensional
                    var listElementType = currentType.GetGenericArguments()[0];
                    var listTarget = Activator.CreateInstance(currentType);
                    payloadReader.TryRead(out byte elementCount);
                    var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    requestSb.AppendLine($"{typeData.Type} {typeData.Length} {elementCount}");

                    for (var i = 0; i < elementCount; i++)
                    {
                        //TODO: I think struct or string will fail here
                        var listParsed = ParseType(ref payloadReader, listElementType, typeData.Type, typeData.Length, state, requestSb);
                        currentType.GetMethod("Add")?.Invoke(listTarget, new[] { listParsed });
                    }
                    return listTarget;
                case TdfType.Blob:
                    var blobSeq = payloadReader.Sequence.Slice(payloadReader.Position, length);
                    payloadReader.Advance(length);
                    requestSb.AppendLine("<blob>");
                    return blobSeq.ToArray();
                case TdfType.Map:
                    //Length is the number of elements
                    var dictTarget = Activator.CreateInstance(currentType);
                    var dictKeyType = currentType.GetGenericArguments()[0];
                    var dictValueType = currentType.GetGenericArguments()[1];

                    var keyTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    requestSb.AppendLine($"{keyTypeData.Type} {keyTypeData.Length}");
                    var parsedKey = ParseType(ref payloadReader, dictKeyType, keyTypeData.Type, keyTypeData.Length, state, requestSb);
                    var valueTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    requestSb.AppendLine($"{valueTypeData.Type} {valueTypeData.Length}");
                    var parsedValue = ParseType(ref payloadReader, dictValueType, valueTypeData.Type, valueTypeData.Length, state, requestSb);
                    currentType.GetMethod("Add")?.Invoke(dictTarget, new[] { parsedKey, parsedValue });

                    var keyType = keyTypeData.Type;
                    var valueType = valueTypeData.Type;
                    //skip first key/value
                    for (var i = 1; i < length; i++)
                    {
                        var keyLength = keyTypeData.Length;
                        if (keyType == TdfType.String)
                        {
                            keyLength = TdfHelper.ParseLength(ref payloadReader);
                        }
                        requestSb.AppendLine($"{keyTypeData.Type} {keyLength}");
                        parsedKey = ParseType(ref payloadReader, dictKeyType, keyTypeData.Type, keyLength, state, requestSb);
                        var valueLength = valueTypeData.Length;
                        if (valueType == TdfType.String)
                        {
                            valueLength = TdfHelper.ParseLength(ref payloadReader);
                        }
                        requestSb.AppendLine($"{valueTypeData.Type} {valueLength}");
                        parsedValue = ParseType(ref payloadReader, dictValueType, valueTypeData.Type, valueLength, state, requestSb);
                        currentType.GetMethod("Add")?.Invoke(dictTarget, new[] { parsedKey, parsedValue });
                    }
                    return dictTarget;
                case TdfType.Union:
                    var unionTarget = Activator.CreateInstance(currentType);
                    payloadReader.Advance(length);
                    payloadReader.TryRead(out byte unionKey);
                    requestSb.AppendLine($"{unionKey}");
                    //VALU
                    var unionValueType = currentType.GetGenericArguments()[1];
                    TdfHelper.ParseTag(ref payloadReader);
                    var valuTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    requestSb.AppendLine($"{valuTypeData.Type} {valuTypeData.Length}");
                    var unionParsed = ParseType(ref payloadReader, unionValueType, valuTypeData.Type, valuTypeData.Length, state, requestSb);
                    currentType.GetField("key", BindingFlags.Instance|BindingFlags.NonPublic)?.SetValue(unionTarget, unionKey);
                    currentType.GetField("value", BindingFlags.Instance|BindingFlags.NonPublic)?.SetValue(unionTarget,unionParsed);
                    return unionTarget;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool EndOfStruct(ref SequenceReader<byte> payloadReader, ParserState state,
            StringBuilder requestSb)
        {
            //end of struct detection (not great and may break)
            if (state.StructDepth > 0 && payloadReader.TryPeek(out var nextByte) && nextByte == 0x0)
            {
                requestSb.AppendLine("<end struct>");
                payloadReader.Advance(1);
                state.StructDepth--;
                return true;
            }

            return false;
        }

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
            var metadata = GetTdfMetadata(target.GetType());
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



        private Dictionary<string, SerializerTdfMetadata> GetTdfMetadata(Type sourceType)
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

    public class SerializerTdfMetadata
    {
        public PropertyInfo Property { get; set; }
        public TdfFieldAttribute Attribute { get; set; }
    }
}