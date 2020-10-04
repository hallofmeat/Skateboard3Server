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

        public bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out BlazeRequest request, out SequencePosition endPosition)
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
            var payloadReader = new SequenceReader<byte>(request.Payload);

            while (!payloadReader.End)
            {
                var label = TdfHelper.ParseLabel(ref payloadReader);
                var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                var type = typeData.Item1;
                var length = typeData.Item2;

                Logger.Debug($"{label} {type} {length}");

                switch (type)
                {
                    case TdfType.Struct:
                        //TODO
                        payloadReader.Advance(length);
                        Logger.Debug($"<struct>");
                        break;
                    case TdfType.String:
                        var byteStr = payloadReader.Sequence.Slice(payloadReader.Position, length);
                        payloadReader.Advance(length);
                        //TODO: figure out if utf8
                        var str = Encoding.UTF8.GetString(byteStr.ToArray());
                        Logger.Debug($"{str}");
                        break;
                    case TdfType.Int8:
                        payloadReader.TryRead(out byte int8);
                        Logger.Debug($"{int8}");
                        break;
                    case TdfType.Uint8:
                        payloadReader.TryRead(out byte uint8);
                        Logger.Debug($"{uint8}");
                        break;
                    case TdfType.Int16:
                        payloadReader.TryReadBigEndian(out short int16);
                        Logger.Debug($"{int16}");
                        break;
                    case TdfType.Uint16:
                        payloadReader.TryReadBigEndian(out short uint16);
                        Logger.Debug($"{Convert.ToUInt16(uint16)}");
                        break;
                    case TdfType.Int32:
                        payloadReader.TryReadBigEndian(out int int32);
                        Logger.Debug($"{int32}");
                        break;
                    case TdfType.Uint32:
                        payloadReader.TryReadBigEndian(out int uint32);
                        Logger.Debug($"{Convert.ToUInt32(uint32)}");
                        break;
                    case TdfType.Int64:
                        payloadReader.TryReadBigEndian(out long int64);
                        Logger.Debug($"{int64}");
                        break;
                    case TdfType.Uint64:
                        payloadReader.TryReadBigEndian(out long uint64);
                        Logger.Debug($"{Convert.ToUInt64(uint64)}");
                        break;
                    case TdfType.Array:
                        //TODO
                        payloadReader.Advance(length);
                        Logger.Debug($"<array>");
                        break;
                    case TdfType.Blob:
                        payloadReader.Advance(length);
                        Logger.Debug($"<blob>");
                        break;
                    case TdfType.Map:
                        payloadReader.Advance(length);
                        Logger.Debug($"<map>");
                        break;
                    case TdfType.Union:
                        payloadReader.Advance(length);
                        payloadReader.TryRead(out byte key);
                        //TODO: handle VALU better
                        Logger.Debug($"<union>");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


            }

            endPosition = reader.Position;
            return true;
        }
    }
}
