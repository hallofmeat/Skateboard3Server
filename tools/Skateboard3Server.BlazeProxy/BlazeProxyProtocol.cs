using System;
using System.Buffers;
using Bedrock.Framework.Protocols;
using NLog;
using Skateboard3Server.Blaze;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.BlazeProxy
{
    public class BlazeProxyProtocol : IMessageReader<BlazeMessageData>, IMessageWriter<BlazeMessageData>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined,
            out BlazeMessageData messageData)
        {
            var reader = new SequenceReader<byte>(input);

            var header = new BlazeHeader();

            //Parse header
            if (!reader.TryReadBigEndian(out short messageLength))
            {
                messageData = default;
                return false;
            }

            header.Length = (ushort)messageLength;

            if (!reader.TryReadBigEndian(out short component))
            {
                messageData = default;
                return false;
            }

            header.Component = (BlazeComponent)(ushort)component;

            if (!reader.TryReadBigEndian(out short command))
            {
                messageData = default;
                return false;
            }

            header.Command = (ushort)command;

            if (!reader.TryReadBigEndian(out short errorCode))
            {
                messageData = default;
                return false;
            }

            header.ErrorCode = (ushort)errorCode;

            if (!reader.TryReadBigEndian(out int messageInfo))
            {
                messageData = default;
                return false;
            }

            header.MessageType = (BlazeMessageType)(messageInfo >> 28);
            header.MessageId = messageInfo & 0xFFFFF;

            Logger.Trace(
                $"Read header; Length:{header.Length} Component:{header.Component} Command:0x{header.Command:X2} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

            //Not enough data in the buffer
            if (reader.Remaining < header.Length)
            {
                Logger.Warn($"Not enough data in the buffer {reader.Remaining} < {header.Length}");
                messageData = default;
                return false;
            }

            //Read body
            var payload = input.Slice(reader.Position, header.Length);
            messageData = new BlazeMessageData
            {
                Header = header,
                Payload = payload
            };

            consumed = payload.End;
            examined = consumed;
            return true;
        }

        public void WriteMessage(BlazeMessageData message, IBufferWriter<byte> output)
        {
            //Header
            var header = message.Header;
            //Length
            var length = BitConverter.GetBytes(Convert.ToUInt16(message.Payload.Length));
            Array.Reverse(length);//big endian
            output.Write(length);
            //Component
            var component = BitConverter.GetBytes((ushort)header.Component);
            Array.Reverse(component);//big endian
            output.Write(component);
            //Command
            var command = BitConverter.GetBytes(header.Command);
            Array.Reverse(command);//big endian
            output.Write(command);
            //ErrorCode
            var errorCode = BitConverter.GetBytes(header.ErrorCode);
            Array.Reverse(errorCode);//big endian
            output.Write(errorCode);
            //MessageType/MessageId
            var messageData =
                BitConverter.GetBytes((int)header.MessageType << 28 | header.MessageId);
            Array.Reverse(messageData);//big endian
            output.Write(messageData);

            //Body
            foreach (var memory in message.Payload)
            {
                output.Write(memory.Span);
            }
        }
    }
}
