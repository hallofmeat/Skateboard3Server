using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.SetGameSettings)]
    public class SetGameSettingsRequest : IRequest<SetGameSettingsResponse>, IBlazeRequest
    {
        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("GSET")]
        public uint GameSettings { get; set; }
    }

}
