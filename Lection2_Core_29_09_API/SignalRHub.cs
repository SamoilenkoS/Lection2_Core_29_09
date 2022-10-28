using Lection2_Core.Core;
using Lection2_Core_API;
using Microsoft.AspNetCore.SignalR;

internal class SignalRHub : Hub<ISignalRClient>, ISignalRServer
{
    private readonly ConnectionsStorage _connectionsStorage;
    private readonly MessageStorage _messageStorage;

    public SignalRHub(
        ConnectionsStorage connectionsStorage,
        MessageStorage messageStorage)
    {
        _connectionsStorage = connectionsStorage;
        _messageStorage = messageStorage;
    }

    public async Task<bool> SetNickname(string nickname)
    {
        if (string.IsNullOrEmpty(nickname) || nickname.Length <= 3) return false;
        
        return _connectionsStorage.Add(Context.ConnectionId, nickname);
    }

    public async Task SendMessageToAll(string message)
    {
        if (_connectionsStorage.GetUserId(Context.ConnectionId) != null)
        {
            await Clients.Others.GetMessage(message);
            _messageStorage.Add(new MessageSnapshot
            {
                IsPersonal = false,
                Message = message,
                SenderId = Context.ConnectionId
            });
        }
        else
        {
            await Clients.Caller.GetMessage("You need to add nickname first!");
        }
    }

    public async Task SendPersonalMessage(string nickname, string message)
    {
        var connectionId = _connectionsStorage.GetConnectionId(nickname);
        if (!string.IsNullOrEmpty(connectionId))
        {
            await Clients.Client(connectionId).GetMessage(message);
            _messageStorage.Add(new MessageSnapshot
            {
                IsPersonal = true,
                Message = message,
                SenderId = Context.ConnectionId,
                ReceiverId = connectionId
            });
        }
    }

    public Task<IEnumerable<MessageSnapshot>> GetRecentMessages(int count = 50)
    {
        return Task.FromResult(_messageStorage.GetRecent(count));
    }
}