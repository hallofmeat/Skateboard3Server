using System.Collections.Generic;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Common
{
    public class StatReport
    {
        [TdfField("RPRT")]
        public Dictionary<string, string> Stats { get; set; }
    }
}