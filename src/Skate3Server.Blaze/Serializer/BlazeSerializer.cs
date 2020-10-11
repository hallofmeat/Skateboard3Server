using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using NLog;
using NLog.Fluent;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Serializer
{
    public interface IBlazeSerializer
    {
        object Deserialize(ref ReadOnlySequence<byte> payload, Type requestType);
        void Serialize(Stream output, BlazeHeader requestHeader, object payload);
    }

    public class BlazeSerializer : IBlazeSerializer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public object Deserialize(ref ReadOnlySequence<byte> payload, Type requestType)
        {
            var request = Activator.CreateInstance(requestType);

            var props = GetTdfMetadata(requestType);

            var payloadReader = new SequenceReader<byte>(payload);
            var payloadStringBuilder = new StringBuilder();
            var state = new SerializerState();

            while (!payloadReader.End)
            {
                //Todo: removing passing props
                ReadTdf(request, ref payloadReader, payloadStringBuilder, state, props);
            }

            Logger.Debug($"Decoded:{Environment.NewLine}{payloadStringBuilder}");
            return request;
        }

        //TODO: this is gross
        public void ReadTdf(object request, ref SequenceReader<byte> payloadReader, StringBuilder payloadStringBuilder,
            SerializerState state, Dictionary<string, SerializerTdfMetadata> props)
        {
            var label = TdfHelper.ParseLabel(ref payloadReader);
            var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
            var type = typeData.Item1;
            var length = typeData.Item2;

            payloadStringBuilder.AppendLine($"{label} {type} {length}");

            switch (type)
            {
                case TdfType.Struct:
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine("<start struct>");
                    state.InStruct = true;
                    break;
                case TdfType.String:
                    var byteStr = payloadReader.Sequence.Slice(payloadReader.Position, length);
                    payloadReader.Advance(length);
                    //TODO: figure out if utf8 is correct
                    var str = Encoding.UTF8.GetString(byteStr.ToArray());
                    payloadStringBuilder.AppendLine($"{str}");
                    //TODO: error checking (check property type)
                    //TODO: trim null byte
                    props[label].Property.SetValue(request, str);
                    break;
                case TdfType.Int8:
                    payloadReader.TryRead(out var int8);
                    payloadStringBuilder.AppendLine($"{int8}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, int8);
                    break;
                case TdfType.Uint8:
                    payloadReader.TryRead(out var uint8);
                    payloadStringBuilder.AppendLine($"{uint8}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, uint8);
                    break;
                case TdfType.Int16:
                    payloadReader.TryReadBigEndian(out short int16);
                    payloadStringBuilder.AppendLine($"{int16}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, int16);
                    break;
                case TdfType.Uint16:
                    payloadReader.TryReadBigEndian(out short uint16);
                    payloadStringBuilder.AppendLine($"{Convert.ToUInt16(uint16)}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, Convert.ToUInt16(uint16));
                    break;
                case TdfType.Int32:
                    payloadReader.TryReadBigEndian(out int int32);
                    payloadStringBuilder.AppendLine($"{int32}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, int32);
                    break;
                case TdfType.Uint32:
                    payloadReader.TryReadBigEndian(out int uint32);
                    payloadStringBuilder.AppendLine($"{Convert.ToUInt32(uint32)}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, Convert.ToUInt32(uint32));
                    break;
                case TdfType.Int64:
                    payloadReader.TryReadBigEndian(out long int64);
                    payloadStringBuilder.AppendLine($"{int64}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, int64);
                    break;
                case TdfType.Uint64:
                    payloadReader.TryReadBigEndian(out long uint64);
                    payloadStringBuilder.AppendLine($"{Convert.ToUInt64(uint64)}");
                    //TODO: error checking (check property type)
                    props[label].Property.SetValue(request, Convert.ToUInt64(uint64));
                    break;
                case TdfType.Array:
                    //TODO
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine("<array>");
                    break;
                case TdfType.Blob:
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine("<blob>");
                    break;
                case TdfType.Map:
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine("<map>");
                    break;
                case TdfType.Union:
                    payloadReader.Advance(length);
                    payloadReader.TryRead(out var key);
                    payloadStringBuilder.AppendLine("<union>");
                    //TODO: error checking (check property type)
                    var unionMetadata = props[label];
                    unionMetadata.Property.SetValue(request, key);
                    //TODO: handle VALU
                    break;
                default:
                    Logger.Debug($"Partial Decode:{Environment.NewLine}{payloadStringBuilder}");
                    throw new ArgumentOutOfRangeException();
            }

            //end of struct detection (not great and may break)
            if (state.InStruct && payloadReader.TryPeek(out var nextByte) && nextByte == 0x0)
            {
                payloadStringBuilder.AppendLine("<end struct>");
                payloadReader.Advance(1);
                state.InStruct = false;
            }
        }

        public void Serialize(Stream output, BlazeHeader requestHeader, object payload)
        {
            //TODO: so many streams
            var bodyStream = new MemoryStream();
            SerializeObjectProperties(bodyStream, payload);

            //Header
            //Length
            var length = BitConverter.GetBytes(Convert.ToUInt16(bodyStream.Length));
            Array.Reverse(length);//big endian
            output.Write(length);
            //Component
            var component = BitConverter.GetBytes((ushort)requestHeader.Component);
            Array.Reverse(component);//big endian
            output.Write(component);
            //Command
            var command = BitConverter.GetBytes(requestHeader.Command);
            Array.Reverse(command);//big endian
            output.Write(command);
            //ErrorCode
            var errorCode = BitConverter.GetBytes(Convert.ToUInt16(0));
            Array.Reverse(errorCode);//big endian
            output.Write(errorCode);
            //MessageType/MessageId
            var messageData =
                BitConverter.GetBytes(Convert.ToUInt32((int)BlazeMessageType.Reply << 28 | requestHeader.MessageId));
            Array.Reverse(messageData);//big endian
            output.Write(messageData);
            //Body TODO: dont use ToArray
            output.Write(bodyStream.ToArray());
        }

        private void SerializeObjectProperties(Stream output, object target)
        {
            var metadata = GetTdfMetadata(target.GetType());
            foreach (var meta in metadata.Values)
            {
                SerializeProperty(output, target, meta);
            }
        }

        private void SerializeProperty(Stream output, object target, SerializerTdfMetadata metadata)
        {
            var propertyType = metadata.Property.PropertyType;
            var propertyValue = metadata.Property.GetValue(target);

            if (metadata.Attribute.GetType() == typeof(TdfUnionKeyAttribute))
            {
                var value = (byte)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Union, 0x0);
                output.WriteByte(value);
            }
            else if (propertyType == typeof(string)) //string
            {
                var value = (string)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.String, Convert.ToUInt32(value.Length + 1));
                //TODO: double check utf8
                output.Write(Encoding.UTF8.GetBytes(value));
                output.WriteByte(0x0); //terminate string
            }
            else if (propertyType == typeof(sbyte)) //Int8
            {
                var value = (sbyte)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Int8, 0x1);
                output.WriteByte(Convert.ToByte(value));
            }
            else if (propertyType == typeof(byte)) //Uint8
            {
                var value = (byte)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Uint8, 0x1);
                output.WriteByte(value);
            }
            //TODO: condense int conversion
            else if (propertyType == typeof(short)) //Int16
            {
                var value = (short)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Int16, 0x2);
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(ushort)) //Uint16
            {
                var value = (ushort)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Uint16, 0x2);
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(int)) //Int32
            {
                var value = (int)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Int32, 0x4);
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(uint)) //Uint32
            {
                var value = (uint)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Uint32, 0x4);
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(long)) //Int64
            {
                var value = (long)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Int32, 0x8);
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(ulong)) //Uint64
            {
                var value = (ulong)propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Uint32, 0x8);
                var valueBytes = BitConverter.GetBytes(value);
                Array.Reverse(valueBytes); //big endian
                output.Write(valueBytes);
            }
            else if (propertyType == typeof(List<>)) //Array
            {
                //TODO: array
                Logger.Warn($"TODO: Property {metadata.Property.Name} is Array, skipping");
            }
            else if (propertyType == typeof(byte[])) //Blob
            {
                var value = (byte[])propertyValue;
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Blob, Convert.ToUInt32(value.Length));
                output.Write(value);
            }
            else if (propertyType == typeof(Dictionary<,>)) //Map
            {
                //TODO: map
                Logger.Warn($"TODO: Property {metadata.Property.Name} is Map, skipping");
            }
            else if (propertyType.IsClass) //Struct?
            {
                TdfHelper.WriteLabel(output, metadata.Attribute.Tag);
                TdfHelper.WriteTypeAndLength(output, TdfType.Struct, 0x0);
                SerializeObjectProperties(output, propertyValue);
                output.WriteByte(0x0); //terminate struct
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

    public class SerializerState
    {
        public bool InStruct { get; set; }

    }

    public class SerializerTdfMetadata
    {
        public PropertyInfo Property { get; set; }
        public TdfFieldAttribute Attribute { get; set; }
    }
}