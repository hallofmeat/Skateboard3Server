using System;
using System.Buffers;
using NLog;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze
{
    public interface IBlazeRequestParser
    {
        bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out SequencePosition endPosition, out BlazeHeader header, out object request);
    }

    public class BlazeRequestParser : IBlazeRequestParser
    {
        private readonly IBlazeDeserializer _blazeDeserializer;
        private readonly IBlazeTypeLookup _blazeTypeLookup;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BlazeRequestParser(IBlazeDeserializer blazeDeserializer, IBlazeTypeLookup blazeTypeLookup)
        {
            _blazeDeserializer = blazeDeserializer;
            _blazeTypeLookup = blazeTypeLookup;
        }

        public bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out SequencePosition endPosition, out BlazeHeader header,
            out object request)
        {
            //For Debug
            var messageHex = BitConverter.ToString(buffer.ToArray()).Replace("-", " ");
            Logger.Trace(messageHex);

            var reader = new SequenceReader<byte>(buffer);

            header = new BlazeHeader();
            request = null;

            Logger.Debug($"Reader Pos Start:{reader.Position.GetInteger()} length: {reader.Length}");

            //Parse header
            if (!reader.TryReadBigEndian(out short messageLength))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Length = (ushort) messageLength;

            if (!reader.TryReadBigEndian(out short component))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Component = (BlazeComponent) (ushort) component;

            if (!reader.TryReadBigEndian(out short command))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Command = (ushort) command;

            if (!reader.TryReadBigEndian(out short errorCode))
            {
                endPosition = reader.Position;
                return false;
            }

            header.ErrorCode = (ushort) errorCode;

            if (!reader.TryReadBigEndian(out int message))
            {
                endPosition = reader.Position;
                return false;
            }

            header.MessageType = (BlazeMessageType) (message >> 28);
            header.MessageId = message & 0xFFFFF;

            Logger.Debug(
                $"Request ^; Length:{header.Length} Component:{header.Component} Command:{header.Command} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

            //Read body
            var payload = reader.Sequence.Slice(reader.Position, header.Length);
            reader.Advance(header.Length);
            endPosition = reader.Position;

            Logger.Debug($"Reader Pos End:{reader.Position.GetInteger()} length: {reader.Length}");

            if (_blazeTypeLookup.TryGetRequestType(header.Component, header.Command, out var requestType))
            {
                try
                {
                    request = _blazeDeserializer.Deserialize(ref payload, requestType);
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return false;
                }
            }

            Logger.Error($"Unknown component: {header.Component} and command: {header.Command}");
            return false;
        }
    }
}
