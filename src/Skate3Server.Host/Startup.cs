using System.ServiceModel;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skate3Server.Api.Controllers;
using Skate3Server.Api.Services;
using SoapCore;

namespace Skate3Server.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(ConfigController).Assembly;
            services.AddControllers()
                .AddApplicationPart(assembly);
            services.AddSoapCore();
        }

        [UsedImplicitly]
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new BlazeRegistry());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //TODO auth

            app.UseSoapEndpoint<ISkateFeedService>("/skate3/ws/SkateFeed.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
