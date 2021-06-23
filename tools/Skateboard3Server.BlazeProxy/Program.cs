using System;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace Skateboard3Server.BlazeProxy
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                //Config
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory
                        .GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                //Services
                var serviceCollection = new ServiceCollection()
                    .AddLogging(loggingBuilder =>
                    {
                        // configure Logging with NLog
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        loggingBuilder.AddNLog(config);
                    });

                serviceCollection.Configure<BlazeProxySettings>(config.GetSection("BlazeProxy"));

                //Autofac
                var containerBuilder = new ContainerBuilder();
                containerBuilder.Populate(serviceCollection);
                containerBuilder.RegisterModule(new ProxyRegistry());

                var container = containerBuilder.Build();
                var serviceProvider = new AutofacServiceProvider(container);

                //Bedrock
                var proxySettings = config.GetSection("BlazeProxy").Get<BlazeProxySettings>();

                var server = new ServerBuilder(serviceProvider)
                    .UseSockets(sockets =>
                    {
                        sockets.ListenAnyIP(42100,
                            builder => builder.UseConnectionLogging(loggingFormatter: HexLoggingFormatter).UseConnectionHandler<BlazeProxyRedirectHandler>());
                        sockets.ListenAnyIP(proxySettings.LocalPort,
                            builder => builder.UseConnectionLogging(loggingFormatter: HexLoggingFormatter).UseConnectionHandler<BlazeProxyHandler>());
                    })
                    .Build();

                await server.StartAsync();

                foreach (var ep in server.EndPoints)
                {
                    logger.Info($"Listening on {ep}");
                }

                var tcs = new TaskCompletionSource<object>();
                Console.CancelKeyPress += (sender, e) => tcs.TrySetResult(null);
                await tcs.Task;

                await server.StopAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        private static void HexLoggingFormatter(Microsoft.Extensions.Logging.ILogger logger, string method, ReadOnlySpan<byte> buffer)
        {
            if (!logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
                return;

            var builder = new StringBuilder($"{method}[{buffer.Length}] ");

            // Write the hex
            foreach (var b in buffer)
            {
                builder.Append(b.ToString("X2"));
                builder.Append(" ");
            }

            logger.LogTrace(builder.ToString());
        }
    }
}
