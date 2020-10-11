using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Skate3Server.Blaze;

namespace Skate3Server.BlazeProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Setup NLog
            LogManager.Setup().LoadConfigurationFromAppSettings();

            //Handle arguments


            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.ListenLocalhost(10744, options =>
                        {
                            //Debug Proxy setup
                            var debugParser = new BlazeDebugParser();
                            var handler = new BlazeProxyHandler(debugParser, "eadpgs-blapp001.ea.com", 10744, true);
                            options.Run(connection => handler.OnConnectedAsync(connection));
                        });
                    }).UseStartup<Startup>(); 
                });
        }
    }
}
