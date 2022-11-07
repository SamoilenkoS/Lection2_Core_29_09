using Lection2_Core.Core;
using System.Collections.Concurrent;

namespace Lection2_Core_API
{
    public class ConnectionsStorage : IConnectionsStorage
    {
        private readonly ConcurrentDictionary<string, PublicUserInfo> _connections =
            new ConcurrentDictionary<string, PublicUserInfo>();

        //TODO Add nickname update
        public string? GetUserNickname(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var userInfo)
               ? userInfo.Nickname : null;
        }

        public PublicUserInfo? GetPublicUserInfo(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var userInfo)
                ? userInfo : null;
        }

        public bool TryAddOrUpdate(string connectionId, PublicUserInfo publicUserInfo)
        {
            if (!_connections.TryAdd(connectionId, publicUserInfo))
            {
                var value = _connections[connectionId];
                return _connections.TryUpdate(
                connectionId,
                new PublicUserInfo
                {
                    Color = value.Color,
                    Nickname = publicUserInfo.Nickname
                },
                _connections[connectionId]);
            }

            return true;
        }

        public void Remove(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);
        }

        public string GetConnectionId(string nickname)
        {
            return _connections.FirstOrDefault(x => x.Value.Nickname == nickname).Key;
        }

        public void SetPersonalColor(string connectionId, ConsoleColor consoleColor)
        {
            _connections[connectionId].Color = consoleColor;
        }
    }
}
