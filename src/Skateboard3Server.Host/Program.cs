using System;
using System.Text;
using Autofac.Extensions.DependencyInjection;
using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Skateboard3Server.Host.Blaze;

namespace Skateboard3Server.Host;

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
                        //gosredirector (Blaze) [TCP]
                        serverOptions.ListenAnyIP(42100,
                            options =>
                            {
                                options.UseConnectionLogging(loggingFormatter: HexLoggingFormatter)
                                    .UseConnectionHandler<BlazeConnectionHandler>();
                            });
                        //eadpgs-blapp001 (Blaze) [TCP]
                        //This normally is ssl but we would need to patch the game to accept our certificate since they use their own internal cert storage
                        serverOptions.ListenAnyIP(10744,
                            options =>
                            {
                                options.UseConnectionLogging(loggingFormatter: HexLoggingFormatter)
                                    .UseConnectionHandler<BlazeConnectionHandler>();

                            });
                        //gostelemetry [TCP]
                        serverOptions.ListenAnyIP(9946,
                            options =>
                            {
                                options.UseConnectionLogging(loggingFormatter: HexLoggingFormatter)
                                    .UseConnectionHandler<DummyConnectionHandler>();
                            });
                        //matchmaking host? //TODO: no idea what format this is in, udp?
                        //serverOptions.ListenAnyIP(9033,
                        //    options =>
                        //    {
                        //        options.UseConnectionLogging(loggingFormatter: HexLoggingFormatter)
                        //            .UseConnectionHandler<DummyConnectionHandler>();
                        //    });
                        //downloads.skate.online (HTTP) [TCP]
                        serverOptions.ListenAnyIP(80);
                    })
                    .UseStartup<Startup>();
            });
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

public static class ConnectionBuilderExtensions
{
    public static TBuilder UseBlazeServerSsl<TBuilder>(this TBuilder builder, BlazeTlsOptions options) where TBuilder : IConnectionBuilder
    {
        builder.Use(next =>
        {
            var middleware = new BlazeSslServerMiddleware(next, options);
            return middleware.OnConnectionAsync;
        });
        return builder;
    }
}