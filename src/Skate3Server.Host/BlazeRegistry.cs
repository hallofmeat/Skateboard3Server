using Autofac;
using MediatR;
using Skate3Server.Blaze;
using Skate3Server.Blaze.Handlers.Authentication;
using Skate3Server.Blaze.Handlers.Redirector;
using Skate3Server.Blaze.Serializer;

namespace Skate3Server.Host
{
    public class BlazeRegistry : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlazeRequestHandler>().As<IBlazeRequestHandler>();
            builder.RegisterType<BlazeRequestParser>().As<IBlazeRequestParser>();
            builder.RegisterType<BlazeSerializer>().As<IBlazeSerializer>();
            builder.RegisterType<BlazeDebugParser>().As<IBlazeDebugParser>();

            //Mediator
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency()
            //builder.RegisterAssemblyTypes(typeof(MyType).GetTypeInfo().Assembly).AsImplementedInterfaces(); // via assembly scan
            builder.RegisterType<ServerInfoHandler>().AsImplementedInterfaces().InstancePerDependency();          // or individually
            builder.RegisterType<PreAuthHandler>().AsImplementedInterfaces().InstancePerDependency();          // or individually
        }
    }
}