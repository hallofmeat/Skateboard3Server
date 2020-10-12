using System;
using System.Buffers;
using System.Text;
using NLog;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze
{
    /// <summary>
    /// Just logs decoded data
    /// </summary>
    public class BlazeDebugParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool TryParse(ref ReadOnlySequence<byte> buffer)
        {
            var messageHex = BitConverter.ToString(buffer.ToArray()).Replace("-", " ");
            Logger.Trace(messageHex);

            var reader = new SequenceReader<byte>(buffer);
            var header = new BlazeHeader();

            //Parse header
            if (!reader.TryReadBigEndian(out short messageLength))
            {
                return false;
            }

            header.Length = Convert.ToUInt16(messageLength);

            if (!reader.TryReadBigEndian(out short component))
            {
                return false;
            }

            header.Component = (BlazeComponent)Convert.ToUInt16(component);

            if (!reader.TryReadBigEndian(out short command))
            {
                return false;
            }

            header.Command = Convert.ToUInt16(command);

            if (!reader.TryReadBigEndian(out short errorCode))
            {
                return false;
            }

            header.ErrorCode = Convert.ToUInt16(errorCode);

            if (!reader.TryReadBigEndian(out int message))
            {
                return false;
            }

            header.MessageType = (BlazeMessageType)(message >> 28);
            header.MessageId = message & 0xFFFFF;

            //Read body
            var payload = reader.Sequence.Slice(reader.Position, header.Length);
            reader.Advance(header.Length);

            Logger.Debug(
                $"Request; Component:{header.Component} Command:{header.Command} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

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
            catch(Exception e)
            {
                Logger.Error(e);
                Logger.Debug($"Partial Decode:{Environment.NewLine}{payloadStringBuilder}");
            }

            return true;
        }

        private void ParseObject(ref SequenceReader<byte> payloadReader, StringBuilder payloadStringBuilder, ParserState state)
        {
            var label = TdfHelper.ParseLabel(ref payloadReader);
            var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
            var type = typeData.Type;
            var length = typeData.Length;

            payloadStringBuilder.AppendLine($"{label} {type} {length}");

            ParseType(ref payloadReader, payloadStringBuilder, type, length, state);
        }

        private void ParseType(ref SequenceReader<byte> payloadReader, StringBuilder payloadStringBuilder, TdfType type, uint length, ParserState state)
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
                    var str = Encoding.UTF8.GetString(byteStr.ToArray());
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
                    payloadStringBuilder.AppendLine($"{Convert.ToUInt16(uint16)}");
                    break;
                case TdfType.Int32:
                    payloadReader.TryReadBigEndian(out int int32);
                    payloadStringBuilder.AppendLine($"{int32}");
                    break;
                case TdfType.Uint32:
                    payloadReader.TryReadBigEndian(out int uint32);
                    payloadStringBuilder.AppendLine($"{Convert.ToUInt32(uint32)}");
                    break;
                case TdfType.Int64:
                    payloadReader.TryReadBigEndian(out long int64);
                    payloadStringBuilder.AppendLine($"{int64}");
                    break;
                case TdfType.Uint64:
                    payloadReader.TryReadBigEndian(out long uint64);
                    payloadStringBuilder.AppendLine($"{Convert.ToUInt64(uint64)}");
                    break;
                case TdfType.Array:
                    //Length is the number of dimensions //TODO: handle multidimensional
                    payloadStringBuilder.AppendLine($"<array start>");
                    payloadReader.TryRead(out byte elementCount);
                    var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
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
                    //Length is the number of elements
                    payloadStringBuilder.AppendLine($"<map start>");
                    var keyTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    ParseType(ref payloadReader, payloadStringBuilder, keyTypeData.Type, keyTypeData.Length, state);
                    var valueTypeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                    ParseType(ref payloadReader, payloadStringBuilder, valueTypeData.Type, valueTypeData.Length, state);
                    var keyType = keyTypeData.Type;
                    var valueType = valueTypeData.Type;
                    //skip first key/value
                    for (var i = 1; i < length; i++)
                    {
                        byte keyLength = 0;
                        if (keyType != TdfType.Array && keyType != TdfType.Map && keyType != TdfType.Struct)
                        {
                            payloadReader.TryRead(out keyLength);
                        }
                        ParseType(ref payloadReader, payloadStringBuilder, keyTypeData.Type, keyLength, state);
                        byte valueLength = 0;
                        if (valueType != TdfType.Array && valueType != TdfType.Map && valueType != TdfType.Struct)
                        {
                            payloadReader.TryRead(out valueLength);
                        }
                        ParseType(ref payloadReader, payloadStringBuilder, valueTypeData.Type, valueLength, state);
                    }
                    payloadStringBuilder.AppendLine($"<map end>");
                    break;
                case TdfType.Union:
                    payloadReader.Advance(length);
                    payloadReader.TryRead(out byte key);
                    payloadStringBuilder.AppendLine($"<union key>");
                    payloadStringBuilder.AppendLine($"{key}");
                    //VALU
                    payloadStringBuilder.AppendLine($"<union value>");
                    ParseObject(ref payloadReader, payloadStringBuilder, state);
                    break;
                default:
                    Logger.Debug($"Partial Decode:{Environment.NewLine}{payloadStringBuilder}");
                    throw new ArgumentOutOfRangeException();

            }
            
            state.Depth--;
        }

        private bool EndOfStruct(ref SequenceReader<byte> payloadReader, StringBuilder payloadStringBuilder, ParserState state)
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

    public class ParserState
    {
        public int StructDepth { get; set; }

        public int Depth { get; set; }

    }
}
