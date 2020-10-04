using System;
using System.Buffers;
using System.Text;
using NLog;

namespace SkateServer.Blaze
{
    public interface IBlazeRequestParser
    {
        bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out BlazeRequest request, out SequencePosition endPosition);
    }

    public class BlazeRequestParser : IBlazeRequestParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out BlazeRequest request,
            out SequencePosition endPosition)
        {
            var reader = new SequenceReader<byte>(buffer);

            request = new BlazeRequest();

            //Parse header
            if (!reader.TryReadBigEndian(out short messageLength))
            {
                endPosition = reader.Position;
                return false;
            }

            request.Length = messageLength;

            if (!reader.TryReadBigEndian(out short component))
            {
                endPosition = reader.Position;
                return false;
            }

            request.Component = (BlazeComponent) component;

            if (!reader.TryReadBigEndian(out short command))
            {
                endPosition = reader.Position;
                return false;
            }

            request.Command = command;

            if (!reader.TryReadBigEndian(out short errorCode))
            {
                endPosition = reader.Position;
                return false;
            }

            request.ErrorCode = errorCode;

            if (!reader.TryReadBigEndian(out int message))
            {
                endPosition = reader.Position;
                return false;
            }

            request.MessageType = (BlazeMessageType) (message >> 28);
            request.MessageId = message & 0xFFFFF;

            //Read body
            request.Payload = reader.Sequence.Slice(reader.Position, request.Length);
            reader.Advance(request.Length);

            Logger.Debug(
                $"Request; Component:{request.Component} Command:{request.Command} ErrorCode:{request.ErrorCode} MessageType:{request.MessageType} MessageId:{request.MessageId}");

            //Parse body TODO: move
            var payloadHex = BitConverter.ToString(request.Payload.ToArray()).Replace("-", " ");
            Logger.Trace(payloadHex);

            var payloadReader = new SequenceReader<byte>(request.Payload);

            var payloadStringBuilder = new StringBuilder();

            var inStruct = false;

            while (!payloadReader.End)
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
                        payloadStringBuilder.AppendLine($"<start struct>");
                        inStruct = true;
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
                        //TODO
                        payloadReader.Advance(length);
                        payloadStringBuilder.AppendLine($"<array>");
                        break;
                    case TdfType.Blob:
                        payloadReader.Advance(length);
                        payloadStringBuilder.AppendLine($"<blob>");
                        break;
                    case TdfType.Map:
                        payloadReader.Advance(length);
                        payloadStringBuilder.AppendLine($"<map>");
                        break;
                    case TdfType.Union:
                        payloadReader.Advance(length);
                        payloadReader.TryRead(out byte key);
                        //TODO: handle VALU better
                        payloadStringBuilder.AppendLine($"<union>");
                        break;
                    default:
                        Logger.Debug($"Partial Decode:{Environment.NewLine}{payloadStringBuilder}");
                        throw new ArgumentOutOfRangeException();
                }

                //end of struct detection (not great and may break)
                if (inStruct && payloadReader.TryPeek(out var nextByte) && nextByte == 0x0)
                {
                    payloadStringBuilder.AppendLine($"<end struct>");
                    payloadReader.Advance(1);
                    inStruct = false;
                }
            }

            Logger.Debug($"Decoded:{Environment.NewLine}{payloadStringBuilder}");

        endPosition = reader.Position;
            return true;
        }
    }
}
