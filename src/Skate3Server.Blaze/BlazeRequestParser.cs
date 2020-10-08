using System;
using System.Buffers;
using System.Collections.Generic;
using NLog;
using Skate3Server.Blaze.Requests;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze
{
    public interface IBlazeRequestParser
    {
        bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out SequencePosition endPosition, out BlazeHeader header, out object request);
    }

    public class BlazeRequestParser : IBlazeRequestParser
    {
        private readonly IBlazeSerializer _blazeSerializer;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //TODO: pull from blaze request attribute
        private static readonly Dictionary<(BlazeComponent, int), Type> RequestLookup =
            new Dictionary<(BlazeComponent, int), Type>
            {
                { (BlazeComponent.Redirector, 0x1), typeof(RedirectorServerInfoRequest) },
            };

        public BlazeRequestParser(IBlazeSerializer blazeSerializer)
        {
            _blazeSerializer = blazeSerializer;
        }

        public bool TryParseRequest(ref ReadOnlySequence<byte> buffer, out SequencePosition endPosition, out BlazeHeader header,
            out object request)
        {
            var reader = new SequenceReader<byte>(buffer);

            header = new BlazeHeader();
            request = null;

            //Parse header
            if (!reader.TryReadBigEndian(out short messageLength))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Length = messageLength;

            if (!reader.TryReadBigEndian(out short component))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Component = (BlazeComponent) component;

            if (!reader.TryReadBigEndian(out short command))
            {
                endPosition = reader.Position;
                return false;
            }

            header.Command = command;

            if (!reader.TryReadBigEndian(out short errorCode))
            {
                endPosition = reader.Position;
                return false;
            }

            header.ErrorCode = errorCode;

            if (!reader.TryReadBigEndian(out int message))
            {
                endPosition = reader.Position;
                return false;
            }

            header.MessageType = (BlazeMessageType) (message >> 28);
            header.MessageId = message & 0xFFFFF;

            //Read body
            var payload = reader.Sequence.Slice(reader.Position, header.Length);
            reader.Advance(header.Length);
            endPosition = reader.Position;

            Logger.Debug(
                $"Request; Component:{header.Component} Command:{header.Command} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

            //For Debug
            var payloadHex = BitConverter.ToString(payload.ToArray()).Replace("-", " ");
            Logger.Trace(payloadHex);

            if (RequestLookup.TryGetValue((header.Component, header.Command), out var requestType))
            {
                request = _blazeSerializer.Deserialize(ref payload, requestType);
                return true;
            }

            throw new ArgumentOutOfRangeException($"Unknown component: {header.Component} and command: {header.Command}");
        }
    }
}
