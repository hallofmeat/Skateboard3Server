using System.Reflection;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NLog;
using Skateboard3Server.Blaze;
using Skateboard3Server.Data;
using Skateboard3Server.Web;
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
            var skateboardConfigSection = Configuration.GetSection("Skateboard");
            if (skateboardConfigSection != null)
            {
                services.Configure<BlazeConfig>(skateboardConfigSection.GetSection("Blaze"));
                services.Configure<WebConfig>(skateboardConfigSection.GetSection("Web"));
            }

            //Load controllers/views and custom xml output formatter from Skateboard3Server.Web
            var webAssembly = typeof(ConfigController).GetTypeInfo().Assembly;
            services.AddControllersWithViews(options => options.OutputFormatters.Add(new PoxOutputFormatter()))
                .AddApplicationPart(webAssembly)
                .AddRazorRuntimeCompilation();

            //Auth from sessions controller
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            //Allow loading of views from Skateboard3Server.Web
            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Add(
                    new EmbeddedFileProvider(webAssembly));
            });

            services.AddDbContext<Skateboard3Context>(options => options.UseSqlite(Configuration.GetConnectionString("SkateboardConnectionSqlite")));
        }

        [UsedImplicitly]
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new BlazeRegistry());
            builder.RegisterModule(new WebRegistry());

            //EfCore
            builder
                .RegisterType<Skateboard3Context>()
                .InstancePerLifetimeScope();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthentication();
            
            app.UseAuthorization();

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
            scope.ServiceProvider.GetRequiredService<Skateboard3Context>().Database.Migrate();
        }
    }


}