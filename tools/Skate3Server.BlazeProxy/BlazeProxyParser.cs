using System;
using System.Buffers;
using System.Text;
using NLog;
using Skate3Server.Blaze;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.BlazeProxy
{
    /// <summary>
    /// Just logs decoded data
    /// </summary>
    public class BlazeProxyParser : BlazeDebugParser
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

            header.Length = (ushort) messageLength;

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
            catch(Exception e)
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

    }
}
