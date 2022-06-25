using System.Collections.Generic;
using Skateboard3Server.Blaze.Serializer.Attributes;

namespace Skateboard3Server.Blaze.Common;

public record StatReport
{
    [TdfField("RPRT")]
    public Dictionary<string, string> Stats { get; set; }
}