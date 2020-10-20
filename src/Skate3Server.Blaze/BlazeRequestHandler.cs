using System.IO;
using System.Threading.Tasks;
using MediatR;
using NLog;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Blaze
{
    public interface IBlazeRequestHandler
    {
        Task ProcessRequest(BlazeHeader header, object request, Stream output);
    }

    public class BlazeRequestHandler : IBlazeRequestHandler
    {
        private readonly IMediator _mediator;
        private readonly IBlazeSerializer _blazeSerializer;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BlazeRequestHandler(IMediator mediator, IBlazeSerializer blazeSerializer)
        {
            _mediator = mediator;
            _blazeSerializer = blazeSerializer;
        }

        public async Task ProcessRequest(BlazeHeader header, object request, Stream output)
        {

            var response = (BlazeResponse) await _mediator.Send(request);
            //TODO: refine signature
            _blazeSerializer.Serialize(output, header, BlazeMessageType.Reply, response);

            //TODO: this is bad but works for now
            if (response != null)
            {
                foreach (var note in response.Notifications)
                {
                    _blazeSerializer.Serialize(output, note.Key, BlazeMessageType.Notification, note.Value);
                }
            }
        }
    }
}