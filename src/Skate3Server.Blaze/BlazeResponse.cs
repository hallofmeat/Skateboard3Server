using System.Collections.Generic;

namespace Skate3Server.Blaze
{
    //TODO: I dont like this but its quick and dirty to get notifications out of request handlers
    public class BlazeResponse
    {
        public IDictionary<BlazeHeader, object> Notifications { get; }
            = new Dictionary<BlazeHeader, object>();
    }
}
