using Lection2_Core.Core;
using System.Collections.Concurrent;

namespace Lection2_Core_API
{
    public class MessageStorage
    {
        private const int DefaultSize = 50;
        private readonly ConcurrentStack<MessageSnapshot> _messages;

        public MessageStorage()
        {
            _messages = new ConcurrentStack<MessageSnapshot>();
        }

        public void Add(MessageSnapshot message)
        {
            _messages.Push(message);
        }

        public IEnumerable<MessageSnapshot> GetRecent(int count = DefaultSize)
        {
            var arr = _messages.ToArray();
            return arr.Skip(arr.Length - DefaultSize).Take(DefaultSize);
        }
    }
}
