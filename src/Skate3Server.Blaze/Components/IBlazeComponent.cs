using System.IO;
using System.Threading.Tasks;

namespace SkateServer.Blaze.Components
{
    public interface IBlazeComponent
    {
        public void HandleRequest(BlazeRequest request, Stream output);
    }
}
