using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using NLog;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze.Server
{
    public interface IBlazeMessageHandler
    {
        Task<IList<BlazeMessageData>> ProcessMessage(BlazeMessageData requestMessageData, ClientContext clientContext);
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

        public async Task<IList<BlazeMessageData>> ProcessMessage(BlazeMessageData requestMessageData, ClientContext clientContext)
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
                        ErrorCode = 0,
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

                    var responseMessages = new List<BlazeMessageData>
                    {
                        responseMessage
                    };

                    //Send new notifications
                    foreach (var note in clientContext.Notifications)
                    {
                        //TODO: remove stream
                        var notificationOutput = new MemoryStream();
                        _blazeSerializer.Serialize(notificationOutput, note.Value);
                        var notification = new BlazeMessageData
                        {
                            Header = note.Key,
                            Payload = new ReadOnlySequence<byte>(notificationOutput.ToArray())
                        };
                        responseMessages.Add(notification);
                    }
                    return responseMessages;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return null;
                }
            }

            Logger.Error($"Unknown component: {requestHeader.Component} and command: {requestHeader.Command}");
            _debugParser.TryParseBody(requestPayload);
            return null;
        }
    }
}
