using System.Threading.Tasks;

namespace Skateboard3Server.Blaze.Server
{
    public interface IBlazeNotificationHandler
    {
        /// <summary>
        /// Sends notification after a response is written
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notification"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        Task EnqueueNotification(uint userId, BlazeNotification notification);

        /// <summary>
        /// Sends a notification right now
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notification"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        Task SendNotification(uint userId, BlazeNotification notification);
    }
}