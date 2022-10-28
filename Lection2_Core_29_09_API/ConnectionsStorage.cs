using System.Collections.Concurrent;

namespace Lection2_Core_API
{
    public class ConnectionsStorage
    {
        private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        public string? GetUserId(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var userId) ? userId : null;
        }
        
        public bool Add(string connectionId, string userId)
        {
            return _connections.TryAdd(connectionId, userId);
        }

        public void Remove(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);
        }
        
        public string GetConnectionId(string nickname)
        {
            return _connections.FirstOrDefault(x => x.Value == nickname).Key;
        }
    }
}
