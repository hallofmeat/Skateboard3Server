using System;
using System.Buffers;
using System.Text;
using NLog;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze
{
    public interface IBlazeDebugParser
    {
        bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out SequencePosition endPosition);
        bool TryParseResponseHeader(ref ReadOnlySequence<byte> buffer, out BlazeHeader header);
        bool TryParseResponseBody(ref ReadOnlySequence<byte> buffer);
    }

    /// <summary>
    /// Just logs decoded data
    /// </summary>
    public class BlazeDebugParser : IBlazeDebugParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out SequencePosition endPosition)
        {
            var messageHex = BitConverter.ToString(buffer.ToArray()).Replace("-", " ");
            Logger.Trace($"Request Message: {messageHex}");

            var reader = new SequenceReader<byte>(buffer);
            var header = new BlazeHeader();

            //Parse header
            if (!reader.TryReadBigEndian(out short messageLength))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Length = (ushort)messageLength;

            if (!reader.TryReadBigEndian(out short component))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Component = (BlazeComponent)(ushort)component;

            if (!reader.TryReadBigEndian(out short command))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Command = (ushort)command;

            if (!reader.TryReadBigEndian(out short errorCode))
            {
                endPosition = reader.Position;
                return false;
            }

            header.ErrorCode = (ushort)errorCode;

            if (!reader.TryReadBigEndian(out int message))
            {
                endPosition = reader.Position;
                return false;
            }

            header.MessageType = (BlazeMessageType)(message >> 28);
            header.MessageId = message & 0xFFFFF;

            Logger.Debug(
                $"Request; Component:{header.Component} Command:{header.Command} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

            //Empty message
            if (header.Length == 0)
            {
                endPosition = reader.Position;
                return true;
            }

            //we read a bigger header than is in the buffer
            if (header.Length > reader.Remaining)
            {
                Logger.Debug($"Header is longer than reader has remaining:{reader.Remaining}");
                endPosition = reader.Position;
                return true;
            }

            //Read body
            var payload = reader.Sequence.Slice(reader.Position, header.Length);
            reader.Advance(header.Length);
            endPosition = reader.Position;

            var payloadReader = new SequenceReader<byte>(payload);

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


        public bool TryParseResponseHeader(ref ReadOnlySequence<byte> buffer, out BlazeHeader header)
        {
            var headerHex = BitConverter.ToString(buffer.ToArray()).Replace("-", " ");
            Logger.Trace($"Response Header: {headerHex}");

            var reader = new SequenceReader<byte>(buffer);
            header = new BlazeHeader();

            //Parse header
            if (!reader.TryReadBigEndian(out short messageLength))
            {
                return false;
            }

            header.Length = (ushort)messageLength;

            Logger.Debug(
                $"Response; Header Length:{header.Length}");

            if (!reader.TryReadBigEndian(out short component))
            {
                return false;
            }

            header.Component = (BlazeComponent)(ushort)component;

            if (!reader.TryReadBigEndian(out short command))
            {
                return false;
            }

            header.Command = (ushort)command;

            if (!reader.TryReadBigEndian(out short errorCode))
            {
                return false;
            }

            header.ErrorCode = (ushort)errorCode;

            if (!reader.TryReadBigEndian(out int message))
            {
                return false;
            }

            header.MessageType = (BlazeMessageType)(message >> 28);
            header.MessageId = message & 0xFFFFF;

            Logger.Debug(
                $"Response; Component:{header.Component} Command:{header.Command} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

            //Empty message
            return true;
        }

        public bool TryParseResponseBody(ref ReadOnlySequence<byte> buffer)
        {
            var bodyHex = BitConverter.ToString(buffer.ToArray()).Replace("-", " ");
            Logger.Trace($"Response Body: {bodyHex}");

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
            if (state.Depth++ >= 100)
            {
                throw new Exception("TOO DEEP!!");
            }
            switch (type)
            {
                case TdfType.Struct:
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine($"<start struct>");
                    state.StructDepth++;
                    do
                    {
                        ParseObject(ref payloadReader, payloadStringBuilder, state);
                    } while (!EndOfStruct(ref payloadReader, payloadStringBuilder, state));
                    break;
                case TdfType.String:
                    var byteStr = payloadReader.Sequence.Slice(payloadReader.Position, length);
                    payloadReader.Advance(length);
                    //TODO: figure out if utf8
                    var str = Encoding.UTF8.GetString(byteStr.ToArray()).TrimEnd('\0');
                    payloadStringBuilder.AppendLine($"{str}");
                    break;
                case TdfType.Int8:
                    payloadReader.TryRead(out byte int8);
                    payloadStringBuilder.AppendLine($"{int8}");
                    break;
                case TdfType.Uint8:
                    payloadReader.TryRead(out byte uint8);
                    payloadStringBuilder.AppendLine($"{uint8}");
                    break;
                case TdfType.Int16:
                    payloadReader.TryReadBigEndian(out short int16);
                    payloadStringBuilder.AppendLine($"{int16}");
                    break;
                case TdfType.Uint16:
                    payloadReader.TryReadBigEndian(out short uint16);
                    payloadStringBuilder.AppendLine($"{(ushort) uint16}");
                    break;
                case TdfType.Int32:
                    payloadReader.TryReadBigEndian(out int int32);
                    payloadStringBuilder.AppendLine($"{int32}");
                    break;
                case TdfType.Uint32:
                    payloadReader.TryReadBigEndian(out int uint32);
                    payloadStringBuilder.AppendLine($"{(uint) uint32}");
                    break;
                case TdfType.Int64:
                    payloadReader.TryReadBigEndian(out long int64);
                    payloadStringBuilder.AppendLine($"{int64}");
                    break;
                case TdfType.Uint64:
                    payloadReader.TryReadBigEndian(out long uint64);
                    payloadStringBuilder.AppendLine($"{(ulong) uint64}");
                    break;
                case TdfType.Array:
                    //TODO print key/value type
                    //Length is the number of dimensions //TODO: handle multidimensional
                    payloadStringBuilder.AppendLine($"<array start>");
                    payloadReader.TryRead(out byte elementCount);
                    var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    payloadStringBuilder.AppendLine($"{typeData.Type} {typeData.Length} {elementCount}");
                    for (var i = 0; i < elementCount; i++)
                    {
                        ParseType(ref payloadReader, payloadStringBuilder, typeData.Type, typeData.Length, state);
                    } 
                    payloadStringBuilder.AppendLine($"<array end>");
                    break;
                case TdfType.Blob:
                    payloadReader.Advance(length);
                    payloadStringBuilder.AppendLine($"<blob>");
                    break;
                case TdfType.Map:
                    //TODO print key/value type
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
                        if (keyType == TdfType.String)
                        {
                            keyLength = TdfHelper.ParseLength(ref payloadReader);
                        }
                        payloadStringBuilder.AppendLine($"{keyTypeData.Type} {keyLength}");
                        ParseType(ref payloadReader, payloadStringBuilder, keyTypeData.Type, keyLength, state);
                        var valueLength = valueTypeData.Length;
                        if (valueType == TdfType.String)
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
                    //TODO print key/value type
                    payloadReader.Advance(length);
                    payloadReader.TryRead(out byte key);
                    payloadStringBuilder.AppendLine($"{key}");
                    //VALU
                    var valuTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    payloadStringBuilder.AppendLine($"{valuTypeData.Type} {valuTypeData.Length}");
                    ParseType(ref payloadReader, payloadStringBuilder, valuTypeData.Type, valuTypeData.Length, state);
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
                payloadStringBuilder.AppendLine($"<end struct>");
                payloadReader.Advance(1);
                state.StructDepth--;
                return true;
            }

            return false;
        }
    }
}
