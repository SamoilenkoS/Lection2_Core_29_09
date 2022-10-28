using Microsoft.AspNetCore.SignalR;

namespace Lection2_Core_API
{
    public class AppUser : IUserIdProvider
    {
        private readonly ConnectionsStorage _connectionsStorage;

        public AppUser(ConnectionsStorage connectionsStorage)
        {
            _connectionsStorage = connectionsStorage;
        }

        public string? GetUserId(HubConnectionContext connection)
        {
            return _connectionsStorage.GetUserId(connection.ConnectionId)
                ?? connection.ConnectionId;
        }

    }
}
