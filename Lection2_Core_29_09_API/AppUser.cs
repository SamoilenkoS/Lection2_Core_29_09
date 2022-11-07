using Microsoft.AspNetCore.SignalR;

namespace Lection2_Core_API
{
    public class AppUser : IUserIdProvider
    {
        private readonly IConnectionsStorage _connectionsStorage;

        public AppUser(IConnectionsStorage connectionsStorage)
        {
            _connectionsStorage = connectionsStorage;
        }

        public string? GetUserId(HubConnectionContext connection)
        {
            return _connectionsStorage.GetUserNickname(connection.ConnectionId)
                ?? connection.ConnectionId;
        }

    }
}
