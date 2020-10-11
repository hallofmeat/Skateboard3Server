using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Skate3Server.Blaze;

namespace Skate3Server.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Setup NLog
            LogManager.Setup().LoadConfigurationFromAppSettings();

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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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
                        //gosredirector (Blaze)
                        serverOptions.ListenLocalhost(42100, options =>
                        {
                            options.UseConnectionHandler<BlazeConnectionHandler>();
                        });
                        //eadpgs-blapp001 (Blaze)
                        serverOptions.ListenLocalhost(10744, options =>
                        {
                            options.UseConnectionHandler<BlazeConnectionHandler>();

                            //Debug Proxy setup
                            //var debugParser = new BlazeDebugParser();
                            //var handler = new BlazeProxyHandler(debugParser, "", 10744, true);
                            //options.Run(connection => handler.OnConnectedAsync(connection));
                        });
                        //qos servers [gosgvaprod-qos01, gosiadprod-qos01, gossjcprod-qos01] (HTTP)
                        serverOptions.ListenLocalhost(17502);
                    })
                    .UseStartup<Startup>();

                });
    }
}
