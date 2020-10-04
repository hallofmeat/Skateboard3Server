using System.IO;
using System.Threading.Tasks;
using NLog;
using SkateServer.Blaze.Components;

namespace SkateServer.Blaze
{
    public interface IBlazeRequestHandler
    {
        Task ProcessRequest(BlazeRequest request, Stream output);
    }

    public class BlazeRequestHandler : IBlazeRequestHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Task ProcessRequest(BlazeRequest request, Stream output)
        {
            //TODO: do better async
            switch (request.Component)
            {
                //TODO: use DI
                case BlazeComponent.Redirector:
                    var redirector = new RedirectorComponent();
                    redirector.HandleRequest(request, output);
                    break;
                default:
                    Logger.Error($"Unknown Component {request.Component}");
                    break;
                    
            }
            return Task.CompletedTask;
        }
    }
}