using Autofac;
using SkateServer.Blaze;

namespace SkateServer.Host
{
    public class BlazeRegistry : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlazeRequestHandler>().As<IBlazeRequestHandler>();
            builder.RegisterType<BlazeRequestParser>().As<IBlazeRequestParser>();
        }
    }
}