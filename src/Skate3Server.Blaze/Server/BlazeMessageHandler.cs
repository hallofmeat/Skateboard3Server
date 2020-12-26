using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using NLog;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze.Server
{
    public interface IBlazeMessageHandler
    {
        Task<BlazeMessageData> ProcessMessage(BlazeMessageData requestMessageData);
    }

    public class BlazeMessageHandler : IBlazeMessageHandler
    {
        private readonly IBlazeDeserializer _blazeDeserializer;
        private readonly IBlazeTypeLookup _blazeTypeLookup;
        private readonly IMediator _mediator;
        private readonly IBlazeSerializer _blazeSerializer;
        private readonly IBlazeDebugParser _debugParser;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BlazeMessageHandler(IBlazeDeserializer blazeDeserializer, IBlazeSerializer blazeSerializer, IBlazeDebugParser debugParser,  IBlazeTypeLookup blazeTypeLookup, IMediator mediator)
        {
            _blazeDeserializer = blazeDeserializer;
            _blazeTypeLookup = blazeTypeLookup;
            _mediator = mediator;
            _blazeSerializer = blazeSerializer;
            _debugParser = debugParser;
        }

        [CanBeNull]
        public async Task<BlazeMessageData> ProcessMessage(BlazeMessageData requestMessageData)
        {
            var requestHeader = requestMessageData.Header;
            var requestPayload = requestMessageData.Payload;
            if (_blazeTypeLookup.TryGetRequestType(requestHeader.Component, requestHeader.Command, out var requestType))
            {
                try
                {
                    var parsedRequest = _blazeDeserializer.Deserialize(requestPayload, requestType);

                    var response = (IBlazeResponse) await _mediator.Send(parsedRequest);
                    var responseHeader = new BlazeHeader
                    {
                        Component = requestHeader.Component,
                        Command = requestHeader.Command,
                        ErrorCode = 0, //TODO: have a better way to handle error code set by the handlers
                        MessageType = BlazeMessageType.Reply,
                        MessageId = requestHeader.MessageId
                    };

                    //TODO: remove stream
                    var output = new MemoryStream();
                    _blazeSerializer.Serialize(output, response);

                    var responseMessage = new BlazeMessageData
                    {
                        Header = responseHeader,
                        Payload = new ReadOnlySequence<byte>(output.ToArray())
                    };

                    return responseMessage;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return null;
                }
            }

            Logger.Error($"Unknown Component: {requestHeader.Component} and Command: 0x{requestHeader.Command:X2}");
            _debugParser.TryParseBody(requestPayload);
            return null;
        }
    }
}
