using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager;

[BlazeNotification(BlazeComponent.GameManager, (ushort)GameManagerNotification.GameStateChange)]
public class GameStateChangeNotification : BlazeNotification
{
    [TdfField("GID")]
    public uint GameId { get; set; }

    [TdfField("GSTA")]
    public GameState GameState { get; set; }

}