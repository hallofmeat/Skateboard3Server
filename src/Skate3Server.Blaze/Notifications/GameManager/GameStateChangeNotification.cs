using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Notifications.GameManager
{
    [BlazeNotification(BlazeComponent.GameManager, (ushort)GameManagerNotification.GameStateChange)]
    public class GameStateChangeNotification : IBlazeNotification
    {
        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("GSTA")]
        public GameState State { get; set; }

    }
}
