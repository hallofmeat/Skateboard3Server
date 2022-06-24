using System.Threading.Tasks;

namespace Skateboard3Server.Blaze.Server;

public interface IBlazeNotificationHandler
{
    /// <summary>
    /// Sends notification after a response is written
    /// </summary>
    /// <param name="personaId"></param>
    /// <param name="notification"></param>
    /// <returns></returns>
    Task EnqueueNotification(uint personaId, BlazeNotification notification);

    /// <summary>
    /// Sends notification after a response is written to the current persona/connection
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    Task EnqueueNotification(BlazeNotification notification);

    /// <summary>
    /// Sends a notification right now
    /// </summary>
    /// <param name="personaId"></param>
    /// <param name="notification"></param>
    /// <returns></returns>
    Task SendNotification(uint personaId, BlazeNotification notification);

    /// <summary>
    /// Sends a notification right now to the current persona/connection
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    Task SendNotification(BlazeNotification notification);
}