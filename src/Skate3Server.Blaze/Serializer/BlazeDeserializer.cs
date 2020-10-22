using System;
using System.Buffers;
using System.Reflection;
using System.Text;
using NLog;

namespace Skate3Server.Blaze.Serializer
{
    public interface IBlazeDeserializer
    {
        object Deserialize(ref ReadOnlySequence<byte> payload, Type requestType);
    }

    public class BlazeDeserializer : IBlazeDeserializer
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
            var tdfMetadata = TdfHelper.GetTdfMetadata(target.GetType());

            var tag = TdfHelper.ParseTag(ref payloadReader);
            var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);

            requestSb.AppendLine($"{tag} {typeData.Type} {typeData.Length}");

            var parsed = ParseType(ref payloadReader, tdfMetadata[tag].Property.PropertyType, typeData.Type,
                typeData.Length, state, requestSb);
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

        private object ParseType(ref SequenceReader<byte> payloadReader, Type currentType, TdfType type, uint length,
            ParserState state,
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
                case TdfType.UInt8:
                    payloadReader.TryRead(out byte uint8);
                    requestSb.AppendLine($"{uint8}");
                    return uint8;
                case TdfType.Int16:
                    payloadReader.TryReadBigEndian(out short int16);
                    requestSb.AppendLine($"{int16}");
                    return int16;
                case TdfType.UInt16:
                    payloadReader.TryReadBigEndian(out short uint16);
                    requestSb.AppendLine($"{uint16}");
                    return (ushort) uint16;
                case TdfType.Int32:
                    payloadReader.TryReadBigEndian(out int int32);
                    requestSb.AppendLine($"{int32}");
                    return int32;
                case TdfType.UInt32:
                    payloadReader.TryReadBigEndian(out int uint32);
                    requestSb.AppendLine($"{uint32}");
                    return (uint) uint32;
                case TdfType.Int64:
                    payloadReader.TryReadBigEndian(out long int64);
                    requestSb.AppendLine($"{int64}");
                    return int64;
                case TdfType.UInt64:
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
                        var listParsed = ParseType(ref payloadReader, listElementType, typeData.Type, typeData.Length,
                            state, requestSb);
                        currentType.GetMethod("Add")?.Invoke(listTarget, new[] {listParsed});
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
                    var parsedKey = ParseType(ref payloadReader, dictKeyType, keyTypeData.Type, keyTypeData.Length,
                        state, requestSb);
                    var valueTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    requestSb.AppendLine($"{valueTypeData.Type} {valueTypeData.Length}");
                    var parsedValue = ParseType(ref payloadReader, dictValueType, valueTypeData.Type,
                        valueTypeData.Length, state, requestSb);
                    currentType.GetMethod("Add")?.Invoke(dictTarget, new[] {parsedKey, parsedValue});

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
                        parsedKey = ParseType(ref payloadReader, dictKeyType, keyTypeData.Type, keyLength, state,
                            requestSb);
                        var valueLength = valueTypeData.Length;
                        if (valueType == TdfType.String)
                        {
                            valueLength = TdfHelper.ParseLength(ref payloadReader);
                        }

                        requestSb.AppendLine($"{valueTypeData.Type} {valueLength}");
                        parsedValue = ParseType(ref payloadReader, dictValueType, valueTypeData.Type, valueLength,
                            state, requestSb);
                        currentType.GetMethod("Add")?.Invoke(dictTarget, new[] {parsedKey, parsedValue});
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
                    var unionParsed = ParseType(ref payloadReader, unionValueType, valuTypeData.Type,
                        valuTypeData.Length, state, requestSb);
                    currentType.GetField("key", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.SetValue(unionTarget, unionKey);
                    currentType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.SetValue(unionTarget, unionParsed);
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
    }
}