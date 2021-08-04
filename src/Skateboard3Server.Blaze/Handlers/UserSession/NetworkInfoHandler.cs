using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.UserSession.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession
{
    public class NetworkInfoHandler : IRequestHandler<NetworkInfoRequest, NetworkInfoResponse>
    {
        private readonly ClientContext _clientContext;
        private readonly IUserSessionManager _userSessionManager;

        public NetworkInfoHandler(ClientContext clientContext, IUserSessionManager userSessionManager)
        {
            _clientContext = clientContext;
            _userSessionManager = userSessionManager;
        }

        public async Task<NetworkInfoResponse> Handle(NetworkInfoRequest request, CancellationToken cancellationToken)
        {
            if (_clientContext.UserSessionId == null)
            {
                throw new Exception("UserId/UserSessionId not on context");
            }
            var currentSessionId = _clientContext.UserSessionId.Value;

            var session = _userSessionManager.GetSession(currentSessionId);
            session.NetworkAddress = request.Address.Value;
            //TODO: rest of the values

            var response = new NetworkInfoResponse();
            return response;
        }
    }
}