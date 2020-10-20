using Autofac;
using MediatR;
using Skate3Server.Blaze;
using Skate3Server.Blaze.Handlers.Authentication;
using Skate3Server.Blaze.Handlers.Redirector;
using Skate3Server.Blaze.Handlers.Util;
using Skate3Server.Blaze.Serializer;
using Skate3Server.Common.Decoders;

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
            builder.RegisterType<BlazeTypeLookup>().As<IBlazeTypeLookup>();
            builder.RegisterType<Ps3TicketDecoder>().As<IPs3TicketDecoder>();

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

            builder.RegisterType<ServerInfoHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PreAuthHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PingHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<LoginHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PostAuthHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ClientMetricsHandler>().AsImplementedInterfaces().InstancePerDependency();
        }
    }
}