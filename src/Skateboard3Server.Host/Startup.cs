using System.Reflection;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NLog;
using Skateboard3Server.Blaze;
using Skateboard3Server.Data;
using Skateboard3Server.Web.Controllers;
using Skateboard3Server.Web.Formatter;

namespace Skateboard3Server.Host
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

            //Load controllers and custom xml output formatter from Skateboard3Server.Web
            var webAssembly = typeof(ConfigController).GetTypeInfo().Assembly;
            services.AddMvc(options => options.OutputFormatters.Add(new PoxOutputFormatter()))
                .AddApplicationPart(webAssembly);

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            //Allow loading of views from Skateboard3Server.Web
            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Add(
                    new EmbeddedFileProvider(webAssembly));
            });

            services.AddDbContext<BlazeContext>(options => options.UseSqlite("Data Source=skateboard3server.db"));
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


}