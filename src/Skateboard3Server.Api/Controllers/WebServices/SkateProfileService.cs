using System.IO;
using CoreWCF;
using Skateboard3Server.Api.Controllers.WebServices.SkateProfile;

namespace Skateboard3Server.Api.Controllers.WebServices
{

    [ServiceContract]
    public interface ISkateProfileService
    {
        [OperationContract]
        StartLoginProcessResponse StartLoginProcess(StartLoginProcess data);

        [OperationContract]
        bool SetUserDlc(Stream data);

        [OperationContract]
        int SetUserAchievements(SetUserAchievements data);
    }

    public class SkateProfileService : ISkateProfileService
    {
        public StartLoginProcessResponse StartLoginProcess(Stream data)
        {
            return new StartLoginProcessResponse
            {
                PrivacyFlagContainer = string.Empty, //TODO: forces a self-closing tag
                AwardedBoardSales = 0,
                TeamInfo = new TeamInfo //TODO
                {
                    TeamId = 0,
                    NumMembers = 0,
                }
            };
        }

        public StartLoginProcessResponse StartLoginProcess(StartLoginProcess data)
        {
            throw new System.NotImplementedException();
        }

        public bool SetUserDlc(Stream data)
        {
            return true;
        }

        public int SetUserAchievements(SetUserAchievements data)
        {
            return 0;
        }
    }
}
