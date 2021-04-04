using Autofac;
using Skateboard3Server.Blaze;

namespace Skateboard3Server.BlazeProxy
{
    public class ProxyRegistry : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlazeDebugParser>().As<IBlazeDebugParser>();
        }
    }
}