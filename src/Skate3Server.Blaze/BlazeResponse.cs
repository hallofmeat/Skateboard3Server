using System.Collections.Generic;

namespace Skate3Server.Blaze
{
    public abstract class BlazeResponse
    {
        //TODO not super happy with this but DI is being a pain https://jimmybogard.com/sharing-context-in-mediatr-pipelines/
        public IDictionary<BlazeHeader, object> Notifications { get; }
            = new Dictionary<BlazeHeader, object>();
    }
}