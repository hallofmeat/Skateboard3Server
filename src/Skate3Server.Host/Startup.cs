using System.ServiceModel;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skate3Server.Api.Controllers;
using Skate3Server.Api.Services;
using Skate3Server.Blaze;
using Skate3Server.Data;
using SoapCore;

namespace Skate3Server.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BlazeConfig>(Configuration.GetSection("Blaze"));

            var assembly = typeof(ConfigController).Assembly;
            services.AddControllers()
                .AddApplicationPart(assembly);
            services.AddSoapCore();

            services.AddDbContext<BlazeContext>(options => options.UseSqlite("Data Source=skate3server.db"));
        }

        [UsedImplicitly]
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new BlazeRegistry());

            //EfCore
            builder
                .RegisterType<BlazeContext>()
                .InstancePerLifetimeScope();

            //Soap Services
            builder.RegisterType<SkateFeedService>().As<ISkateFeedService>().SingleInstance();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

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

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetRequiredService<BlazeContext>().Database.Migrate();
        }
    }
}
