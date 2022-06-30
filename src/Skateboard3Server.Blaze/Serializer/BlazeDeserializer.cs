using System;
using System.Buffers;
using System.Reflection;
using System.Text;
using NLog;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Serializer;

public interface IBlazeDeserializer
{
    BlazeRequestMessage Deserialize(in ReadOnlySequence<byte> payload, Type requestType);
}

public class BlazeDeserializer : IBlazeDeserializer
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public BlazeRequestMessage Deserialize(in ReadOnlySequence<byte> payload, Type requestType)
    {
        var request = Activator.CreateInstance(requestType);

        if (request == null)
        {
            throw new Exception($"Failed to create {requestType}");
        }

        var requestSb = new StringBuilder();

        var payloadReader = new SequenceReader<byte>(payload);
        var state = new ParserState();

        while (!payloadReader.End)
        {
            ParseObject(ref payloadReader, request, state, requestSb);
        }

        Logger.Trace($"Request parsed:{Environment.NewLine}{requestSb}");

        return (BlazeRequestMessage) request;
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
                requestSb.AppendLine($"<start struct {state.StructDepth}>");
                state.StructDepth++;
                var subTarget = Activator.CreateInstance(currentType);
                if (subTarget == null)
                {
                    throw new Exception($"Failed to create type {currentType}");
                }
                while (!EndOfStruct(ref payloadReader, state, requestSb))
                {
                    ParseObject(ref payloadReader, subTarget, state, requestSb);
                }
                return subTarget;
            case TdfType.String:
                var byteStr = payloadReader.Sequence.Slice(payloadReader.Position, length);
                payloadReader.Advance(length);
                //TODO: figure out if utf8
                var str = Encoding.ASCII.GetString(byteStr.ToArray()).TrimEnd('\0');
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
                if (listTarget == null)
                {
                    throw new Exception($"Failed to create type {currentType}");
                }
                payloadReader.TryRead(out byte elementCount);
                var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                requestSb.AppendLine($"{typeData.Type} {typeData.Length} {elementCount}");
                //first element
                var firstParsed = ParseType(ref payloadReader, listElementType, typeData.Type, typeData.Length,
                    state, requestSb);
                currentType.GetMethod("Add")?.Invoke(listTarget, new[] { firstParsed });
                for (var i = 1; i < elementCount; i++)
                {
                    var elementLength = typeData.Length;
                    if (typeData.Type == TdfType.String || typeData.Type == TdfType.Blob)
                    {
                        elementLength = TdfHelper.ParseLength(ref payloadReader);
                    }
                    var elementParsed = ParseType(ref payloadReader, listElementType, typeData.Type, elementLength,
                        state, requestSb);
                    currentType.GetMethod("Add")?.Invoke(listTarget, new[] {elementParsed});
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
                if (dictTarget == null)
                {
                    throw new Exception($"Failed to create type {currentType}");
                }
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
                    if (keyType == TdfType.String || keyType == TdfType.Blob)
                    {
                        keyLength = TdfHelper.ParseLength(ref payloadReader);
                    }

                    requestSb.AppendLine($"{keyTypeData.Type} {keyLength}");
                    parsedKey = ParseType(ref payloadReader, dictKeyType, keyTypeData.Type, keyLength, state,
                        requestSb);
                    var valueLength = valueTypeData.Length;
                    if (valueType == TdfType.String || valueType == TdfType.Blob)
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
                if (unionTarget == null)
                {
                    throw new Exception($"Failed to create type {currentType}");
                }
                payloadReader.Advance(length);
                payloadReader.TryRead(out byte unionKey);
                requestSb.AppendLine($"{unionKey}");
                currentType.GetField("key", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(unionTarget, unionKey);
                //VALU
                var unionValueType = currentType.GetGenericArguments()[1];
                byte readTemp; //TODO this is gross, but VALU can be optional if value is null
                if (payloadReader.TryPeek(out readTemp) && readTemp == 0xDA)
                {
                    payloadReader.Advance(1);
                    if (payloadReader.TryPeek(out readTemp) && readTemp == 0x1B)
                    {
                        payloadReader.Advance(1);
                        if (payloadReader.TryPeek(out readTemp) && readTemp == 0x35)
                        {
                            payloadReader.Advance(1);
                            //TdfHelper.ParseTag(ref payloadReader);
                            var valuTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                            requestSb.AppendLine($"{valuTypeData.Type} {valuTypeData.Length}");
                            var unionParsed = ParseType(ref payloadReader, unionValueType, valuTypeData.Type,
                                valuTypeData.Length, state, requestSb);
                            currentType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic)
                                ?.SetValue(unionTarget, unionParsed);
                        }
                    }
                }
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
            payloadReader.Advance(1);
            state.StructDepth--;
            requestSb.AppendLine($"<end struct {state.StructDepth}>");
            return true;
        }

        return false;
    }
}