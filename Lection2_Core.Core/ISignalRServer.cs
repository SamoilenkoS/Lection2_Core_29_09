namespace Lection2_Core.Core
{
    public interface ISignalRServer
    {
        Task SendMessageToAll(string message);
        Task<bool> SetNickname(string nickname);
        Task SendPersonalMessage(string nickname, string message);
        Task<IEnumerable<MessageSnapshot>> GetRecentMessages(int count = 50);
    }
}