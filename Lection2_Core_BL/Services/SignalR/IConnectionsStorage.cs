using Lection2_Core.Core;

namespace Lection2_Core_API
{
    public interface IConnectionsStorage
    {
        string GetConnectionId(string nickname);
        PublicUserInfo? GetPublicUserInfo(string connectionId);
        string? GetUserNickname(string connectionId);
        void Remove(string connectionId);
        void SetPersonalColor(string connectionId, ConsoleColor consoleColor);
        bool TryAddOrUpdate(string connectionId, PublicUserInfo publicUserInfo);
    }
}