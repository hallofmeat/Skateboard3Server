using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Teams.Messages
{
    [BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.Unknown640)]
    public class Unknown640Request : IRequest<Unknown640Response>, IBlazeRequest
    {
        [TdfField("CLID")]
        public uint ClientId { get; set; }

        [TdfField("INVT")]
        public int Invt { get; set; } //TODO

        [TdfField("NSOT")]
        public int Nsot { get; set; } //TODO

    }
}
