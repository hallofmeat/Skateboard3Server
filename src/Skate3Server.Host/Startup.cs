using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using Skate3Server.Api.Controllers;
using Skate3Server.Blaze;
using Skate3Server.Data;

namespace Skate3Server.Host
{
    public class Startup
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BlazeConfig>(Configuration.GetSection("Blaze"));

            var assembly = typeof(ConfigController).Assembly;
            services.AddMvc(options => options.OutputFormatters.Insert(0, new WcfXmlSerializerOutputFormatter()))
                .AddApplicationPart(assembly)
                .AddXmlSerializerFormatters();

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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            //TODO auth
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //Help find the endpoints we havent made yet
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 404)
                {
                    Logger.Warn($"TODO: 404 {context.HttpContext.Request.GetDisplayUrl()}");
                }
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetRequiredService<BlazeContext>().Database.Migrate();
        }
    }

    public class WcfXmlSerializerOutputFormatter : XmlSerializerOutputFormatter
    {
        public override XmlWriter CreateXmlWriter(TextWriter writer, XmlWriterSettings xmlWriterSettings)
        {
            xmlWriterSettings.OmitXmlDeclaration = false;
            xmlWriterSettings.CloseOutput = false;
            xmlWriterSettings.CheckCharacters = false;
            xmlWriterSettings.Indent = true;

            return base.CreateXmlWriter(writer, xmlWriterSettings);
        }

        protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(xmlWriter, value, namespaces);
        }
    }
}