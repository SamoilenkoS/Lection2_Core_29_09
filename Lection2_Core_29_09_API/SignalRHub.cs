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

        return _connectionsStorage.TryAddOrUpdate(
            Context.ConnectionId,
            new PublicUserInfo
            {
                Nickname = nickname
            });
    }

    public async Task SendMessageToAll(string message)
    {
        if (await CheckIsUserNameExist())
        {
            var senderUserInfo = _connectionsStorage.GetPublicUserInfo(Context.ConnectionId);
            var messageSnapshot = new MessageSnapshot
            {
                IsPersonal = false,
                Message = message,
                SenderUserInfo = senderUserInfo!
            };
            await Clients.Others.GetMessage(messageSnapshot);
            
            _messageStorage.Add(messageSnapshot);
        }
    }

    public async Task SendPersonalMessage(string nickname, string message)
    {
        var targetConnectionId = _connectionsStorage.GetConnectionId(nickname);
        if (!string.IsNullOrEmpty(targetConnectionId))
        {
            if (await CheckIsUserNameExist())
            {
                var senderPublicInfo = _connectionsStorage.GetPublicUserInfo(Context.ConnectionId);
                var messageSnapshot = new MessageSnapshot
                {
                    IsPersonal = true,
                    Message = message,
                    SenderUserInfo = senderPublicInfo!,
                    ReceiverNickname = nickname
                };
                await Clients.Client(targetConnectionId).GetMessage(messageSnapshot);
                _messageStorage.Add(messageSnapshot);
            }
        }
    }

    private async Task<bool> CheckIsUserNameExist()
    {
        if (_connectionsStorage.GetUserNickname(Context.ConnectionId) == null)
        {
            var messageSnapshot = new MessageSnapshot
            {
                IsPersonal = true,
                Message = "You must set nickname before sending messages",
                SenderUserInfo = new PublicUserInfo
                {
                    Color = ConsoleColor.Red,
                    Nickname = "Server"
                }
            };
            await Clients.Caller.GetMessage(messageSnapshot);
            return false;
        }
        return true;
    }

    public Task<IEnumerable<MessageSnapshot>> GetRecentMessages(int count = 50)
    {
        var connectionNickName = _connectionsStorage.GetUserNickname(Context.ConnectionId);
        return Task.FromResult(_messageStorage.GetRecent(connectionNickName!, count));
    }

    public Task SetPersonalColor(ConsoleColor consoleColor)
    {
        _connectionsStorage.SetPersonalColor(Context.ConnectionId, consoleColor);
        return Task.CompletedTask;
    }
}