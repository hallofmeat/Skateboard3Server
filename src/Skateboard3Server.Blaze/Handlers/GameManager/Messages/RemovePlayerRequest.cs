using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.RemovePlayer)]
    public class RemovePlayerRequest : BlazeRequest, IRequest<RemovePlayerResponse>
    {
        [TdfField("BTPL")]
        public ulong Btpl { get; set; } //TODO

        [TdfField("CNTX")]
        public ushort Cntx { get; set; } //TODO context?

        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("PID")]
        public uint PersonaId { get; set; }

        [TdfField("REAS")]
        public PlayerRemoveReason Reason { get; set; }

    }
}
