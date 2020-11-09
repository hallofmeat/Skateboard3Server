using Autofac;
using Skate3Server.Blaze;

namespace Skate3Server.BlazeProxy
{
    public class ProxyRegistry : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlazeDebugParser>().As<IBlazeDebugParser>();
        }
    }
}