using System.ServiceModel;
using Skate3Server.Api.Services.Models;

namespace Skate3Server.Api.Services
{
    [ServiceContract]
    public interface ISkateFeedService
    {
        [OperationContract]
        int PlayerSignedIntoEANation(PlayerSignedIntoEANation request);

    }

    public class SkateFeedService : ISkateFeedService
    {
        public int PlayerSignedIntoEANation(PlayerSignedIntoEANation request)
        {
            return 1; //TODO return 0
        }
    }
}