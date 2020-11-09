using System;
using System.Buffers;
using System.Text;
using NLog;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze
{
    public interface IBlazeDebugParser
    { 
        bool TryParseBody(in ReadOnlySequence<byte> buffer);
    }

    /// <summary>
    /// Just logs decoded data
    /// </summary>
    public class BlazeDebugParser : IBlazeDebugParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool TryParseBody(in ReadOnlySequence<byte> buffer)
        {
            var bodyHex = BitConverter.ToString(buffer.ToArray()).Replace("-", " ");
            Logger.Trace($"Raw Body: {bodyHex}");

            var payloadReader = new SequenceReader<byte>(buffer);

            var payloadStringBuilder = new StringBuilder();

            var state = new ParserState();

            try
            {
                while (!payloadReader.End)
                {
                    ParseObject(ref payloadReader, payloadStringBuilder, state);
                }

                Logger.Debug($"Decoded:{Environment.NewLine}{payloadStringBuilder}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                Logger.Debug($"Partial Decode:{Environment.NewLine}{payloadStringBuilder}");
            }

            return true;
        }

        public void ParseObject(ref SequenceReader<byte> payloadReader, StringBuilder payloadStringBuilder, ParserState state)
        {
            var label = TdfHelper.ParseTag(ref payloadReader);
            var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
            var type = typeData.Type;
            var length = typeData.Length;

            payloadStringBuilder.AppendLine($"{label} {type} {length}");

            ParseType(ref payloadReader, payloadStringBuilder, type, length, state);
        }

        public void ParseType(ref SequenceReader<byte> payloadReader, StringBuilder payloadStringBuilder, TdfType type, uint length, ParserState state)
        {
            if (state.Depth++ >= 20)
            {
                throw new Exception("TOO DEEP!!");
            }
            switch (type)
            {
                case TdfType.Struct:
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine($"<start struct {state.StructDepth}>");
                    state.StructDepth++;
                    while (!EndOfStruct(ref payloadReader, payloadStringBuilder, state))
                    {
                        ParseObject(ref payloadReader, payloadStringBuilder, state);
                    }
                    break;
                case TdfType.String:
                    var byteStr = payloadReader.Sequence.Slice(payloadReader.Position, length);
                    payloadReader.Advance(length);
                    //TODO: figure out if utf8
                    var str = Encoding.ASCII.GetString(byteStr.ToArray()).TrimEnd('\0');
                    payloadStringBuilder.AppendLine($"{str}");
                    break;
                case TdfType.Int8:
                    payloadReader.TryRead(out byte int8);
                    payloadStringBuilder.AppendLine($"{int8}");
                    break;
                case TdfType.UInt8:
                    payloadReader.TryRead(out byte uint8);
                    payloadStringBuilder.AppendLine($"{uint8}");
                    break;
                case TdfType.Int16:
                    payloadReader.TryReadBigEndian(out short int16);
                    payloadStringBuilder.AppendLine($"{int16}");
                    break;
                case TdfType.UInt16:
                    payloadReader.TryReadBigEndian(out short uint16);
                    payloadStringBuilder.AppendLine($"{(ushort) uint16}");
                    break;
                case TdfType.Int32:
                    payloadReader.TryReadBigEndian(out int int32);
                    payloadStringBuilder.AppendLine($"{int32}");
                    break;
                case TdfType.UInt32:
                    payloadReader.TryReadBigEndian(out int uint32);
                    payloadStringBuilder.AppendLine($"{(uint) uint32}");
                    break;
                case TdfType.Int64:
                    payloadReader.TryReadBigEndian(out long int64);
                    payloadStringBuilder.AppendLine($"{int64}");
                    break;
                case TdfType.UInt64:
                    payloadReader.TryReadBigEndian(out long uint64);
                    payloadStringBuilder.AppendLine($"{(ulong) uint64}");
                    break;
                case TdfType.Array:
                    //Length is the number of dimensions //TODO: handle multidimensional
                    payloadStringBuilder.AppendLine($"<array start>");
                    payloadReader.TryRead(out byte elementCount);
                    var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    payloadStringBuilder.AppendLine($"{typeData.Type} {typeData.Length} {elementCount}");
                    //Parse first element
                    ParseType(ref payloadReader, payloadStringBuilder, typeData.Type, typeData.Length, state);
                    for (var i = 1; i < elementCount; i++)
                    {
                        var elementLength = typeData.Length;
                        if (typeData.Type == TdfType.String || typeData.Type == TdfType.Blob)
                        {
                            elementLength = TdfHelper.ParseLength(ref payloadReader);
                        }
                        ParseType(ref payloadReader, payloadStringBuilder, typeData.Type, elementLength, state);
                    } 
                    payloadStringBuilder.AppendLine($"<array end>");
                    break;
                case TdfType.Blob:
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine($"<blob>");
                    break;
                case TdfType.Map:
                    //Length is the number of elements
                    payloadStringBuilder.AppendLine($"<map start>");
                    var keyTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    payloadStringBuilder.AppendLine($"{keyTypeData.Type} {keyTypeData.Length}");
                    ParseType(ref payloadReader, payloadStringBuilder, keyTypeData.Type, keyTypeData.Length, state);
                    var valueTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    payloadStringBuilder.AppendLine($"{valueTypeData.Type} {valueTypeData.Length}");
                    ParseType(ref payloadReader, payloadStringBuilder, valueTypeData.Type, valueTypeData.Length, state);
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
                        payloadStringBuilder.AppendLine($"{keyTypeData.Type} {keyLength}");
                        ParseType(ref payloadReader, payloadStringBuilder, keyTypeData.Type, keyLength, state);
                        var valueLength = valueTypeData.Length;
                        if (valueType == TdfType.String || valueType == TdfType.Blob)
                        {
                            valueLength = TdfHelper.ParseLength(ref payloadReader);
                        }
                        payloadStringBuilder.AppendLine($"{valueTypeData.Type} {valueLength}");
                        ParseType(ref payloadReader, payloadStringBuilder, valueTypeData.Type, valueLength, state);
                    }
                    payloadStringBuilder.AppendLine($"<map end>");
                    break;
                case TdfType.Union:
                    payloadStringBuilder.AppendLine($"<union start>");
                    payloadReader.Advance(length);
                    payloadReader.TryRead(out byte key);
                    payloadStringBuilder.AppendLine($"{key}");
                    //VALU
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
                                var valuTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                                payloadStringBuilder.AppendLine($"{valuTypeData.Type} {valuTypeData.Length}");
                                ParseType(ref payloadReader, payloadStringBuilder, valuTypeData.Type,
                                    valuTypeData.Length, state);
                            }
                        }
                    }
                    break;
                default:
                    Logger.Debug($"Partial Decode:{Environment.NewLine}{payloadStringBuilder}");
                    throw new ArgumentOutOfRangeException();

            }
            
            state.Depth--;
        }

        public bool EndOfStruct(ref SequenceReader<byte> payloadReader, StringBuilder payloadStringBuilder, ParserState state)
        {
            //end of struct detection (not great and may break)
            if (state.StructDepth > 0 && payloadReader.TryPeek(out var nextByte) && nextByte == 0x0)
            {
                payloadReader.Advance(1);
                state.StructDepth--;
                payloadStringBuilder.AppendLine($"<end struct {state.StructDepth}>");
                return true;
            }

            return false;
        }
    }
}
