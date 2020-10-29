using Autofac;
using MediatR;
using Skate3Server.Api.Services;
using Skate3Server.Blaze;
using Skate3Server.Blaze.Handlers.Authentication;
using Skate3Server.Blaze.Handlers.Redirector;
using Skate3Server.Blaze.Handlers.Social;
using Skate3Server.Blaze.Handlers.Stats;
using Skate3Server.Blaze.Handlers.Unknown0B;
using Skate3Server.Blaze.Handlers.UserSession;
using Skate3Server.Blaze.Handlers.Util;
using Skate3Server.Blaze.Serializer;
using Skate3Server.Blaze.Server;
using Skate3Server.Common.Decoders;

namespace Skate3Server.Host
{
    public class BlazeRegistry : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlazeMessageHandler>().As<IBlazeMessageHandler>();
            builder.RegisterType<BlazeSerializer>().As<IBlazeSerializer>();
            builder.RegisterType<BlazeDeserializer>().As<IBlazeDeserializer>();
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

            //Blaze Message Handlers
            builder.RegisterType<ServerInfoHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PreAuthHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PingHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<LoginHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PostAuthHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ClientMetricsHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SessionDataHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<HardwareFlagsHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<FriendsListHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<NetworkInfoHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SkateStatsHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<DlcHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<Unknown2700Handler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<Unknown1600Handler>().AsImplementedInterfaces().InstancePerDependency();

            //Soap Services
            builder.RegisterType<SkateFeedService>().As<ISkateFeedService>().SingleInstance();

        }
    }
}