using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using NLog;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze
{
    public interface IBlazeMessageHandler
    {
        Task<IList<BlazeMessage>> ProcessMessage(BlazeMessage requestMessage);
    }

    public class BlazeMessageHandler : IBlazeMessageHandler
    {
        private readonly IBlazeDeserializer _blazeDeserializer;
        private readonly IBlazeTypeLookup _blazeTypeLookup;
        private readonly IMediator _mediator;
        private readonly IBlazeSerializer _blazeSerializer;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BlazeMessageHandler(IBlazeDeserializer blazeDeserializer, IBlazeTypeLookup blazeTypeLookup, IMediator mediator, IBlazeSerializer blazeSerializer)
        {
            _blazeDeserializer = blazeDeserializer;
            _blazeTypeLookup = blazeTypeLookup;
            _mediator = mediator;
            _blazeSerializer = blazeSerializer;
        }

        //TODO: change to a connectionhub type way of handling notifications instead of returning a list
        public async Task<IList<BlazeMessage>> ProcessMessage(BlazeMessage requestMessage)
        {
            var requestMessageHeader = requestMessage.Header;
            if (_blazeTypeLookup.TryGetRequestType(requestMessageHeader.Component, requestMessageHeader.Command, out var requestType))
            {
                try
                {
                    var parsedRequest = _blazeDeserializer.Deserialize(requestMessage.Payload, requestType);

                    var response = (BlazeResponse) await _mediator.Send(parsedRequest);
                    var responseHeader = new BlazeHeader
                    {
                        Component = requestMessageHeader.Component,
                        Command = requestMessageHeader.Command,
                        ErrorCode = 0,
                        MessageType = BlazeMessageType.Reply,
                        MessageId = requestMessageHeader.MessageId
                    };

                    //TODO: remove stream
                    var output = new MemoryStream();
                    _blazeSerializer.Serialize(output, response);

                    var responseMessage = new BlazeMessage
                    {
                        Header = responseHeader,
                        Payload = new ReadOnlySequence<byte>(output.ToArray())
                    };

                    var responseMessages = new List<BlazeMessage>
                    {
                        responseMessage
                    };

                    //Send new notifications
                    //TODO: this is bad but works for now
                    if (response != null)
                    {
                        foreach (var note in response.Notifications)
                        {
                            //TODO: remove stream
                            var notificationOutput = new MemoryStream();
                            _blazeSerializer.Serialize(notificationOutput, note.Value);
                            var notification = new BlazeMessage
                            {
                                Header = note.Key,
                                Payload = new ReadOnlySequence<byte>(notificationOutput.ToArray())
                            };
                            responseMessages.Add(notification);
                        }
                    }
                    return responseMessages;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return null;
                }
            }

            Logger.Error($"Unknown component: {requestMessageHeader.Component} and command: {requestMessageHeader.Command}");
            return null;
        }
    }
}
