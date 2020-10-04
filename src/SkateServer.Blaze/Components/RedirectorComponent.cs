using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using NLog;

namespace SkateServer.Blaze.Components
{
    public class RedirectorComponent : IBlazeComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void HandleRequest(BlazeRequest request, Stream output)
        {
            switch (request.Command)
            {
                case 1:
                    SendServerInstance(request, output);
                    break;
                default:
                    Logger.Error($"Unknown Redirector command: {request.Command}");
                    break;

            }
            
        }

        public void SendServerInstance(BlazeRequest request, Stream output)
        {
            //We dont care about the request for now just reply with what we know




        }
    }
}
