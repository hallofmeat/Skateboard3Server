using Autofac;
using Skateboard3Server.Web.Storage;

namespace Skateboard3Server.Host;

public class WebRegistry : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FilesystemBlobStorage>().As<IBlobStorage>();
    }
}