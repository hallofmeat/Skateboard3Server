using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Notifications.GameManager
{
    [BlazeNotification(BlazeComponent.GameManager, (ushort)GameManagerNotification.MatchmakingFinished)]
    public class MatchmakingFinishedNotification : IBlazeNotification
    {
        [TdfField("FIT")]
        public uint Fit { get; set; } //TODO

        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("MAXF")]
        public uint Maxf { get; set; } //TODO

        [TdfField("MSID")]
        public uint Msid { get; set; } //TODO

        [TdfField("RSLT")]
        public MatchmakingResult Result { get; set; }

        [TdfField("USID")]
        public uint Usid { get; set; } //TODO
    }
}
