using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Teams.Messages
{
    [BlazeResponse(BlazeComponent.Teams, (ushort)TeamsCommand.Unknown640)]
    public class Unknown640Response : BlazeResponse
    {
        //Empty
    }
}
