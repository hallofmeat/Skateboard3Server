using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Unknown0B.Messages
{
    [BlazeRequest(BlazeComponent.Unknown0B, 0x640)]
    public class Unknown1600Request : IRequest<Unknown1600Response>
    {
        [TdfField("CLID")]
        public uint ClientId { get; set; }

        [TdfField("INVT")]
        public int Invt { get; set; } //TODO

        [TdfField("NSOT")]
        public int Nsot { get; set; } //TODO

    }
}
