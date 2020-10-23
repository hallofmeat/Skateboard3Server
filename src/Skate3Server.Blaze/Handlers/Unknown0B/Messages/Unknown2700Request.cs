using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Unknown0B.Messages
{
    [BlazeRequest(BlazeComponent.Unknown0B, 0xA8C)]
    public class Unknown2700Request : IRequest<Unknown2700Response>
    {
        [TdfField("IDLT")]
        public List<uint> Idlt { get; set; } //TODO

    }
}
