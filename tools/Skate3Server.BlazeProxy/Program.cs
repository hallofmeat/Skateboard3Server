using System;
using System.Threading.Tasks;
using NLog;

namespace Skate3Server.BlazeProxy
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            try
            {
                var proxy = new BlazeProxy();
                await proxy.Start("eadpgs-blapp001.ea.com", 10744, 10744);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to start {ex.Message}");
                throw;
            }
        }
    }
}
