using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
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
                            //TODO: add connection logging middleware

                            //gosredirector (Blaze)
                            serverOptions.ListenAnyIP(42100,
                                options => { options.UseConnectionLogging().UseConnectionHandler<BlazeConnectionHandler>(); });
                            //eadpgs-blapp001 (Blaze)
                            serverOptions.ListenAnyIP(10744,
                                options => { options.UseConnectionLogging().UseConnectionHandler<BlazeConnectionHandler>(); });
                            //gostelemetry //TODO: no idea when this gets called
                            //serverOptions.ListenAnyIP(9946,
                            //    options => { options.UseConnectionHandler<BlazeConnectionHandler>(); });
                            //qos servers [gosgvaprod-qos01, gosiadprod-qos01, gossjcprod-qos01] (HTTP)
                            serverOptions.ListenAnyIP(17502);
                            //TODO qos UDP 17499
                            //downloads.skate.online (HTTP)
                            serverOptions.ListenAnyIP(80);
                        })
                        .UseStartup<Startup>();
                });
        }
    }
}
