using Lection2_Core.Core;
using System.Collections.Concurrent;

namespace Lection2_Core_API
{
    public class MessageStorage : IMessageStorage
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

        public IEnumerable<MessageSnapshot> GetRecent(
            string callerNickname,
            int count = DefaultSize)
        {
            var arr = _messages.Where(x =>
                !x.IsPersonal ||//global messages
                x.ReceiverNickname == callerNickname ||//to me from someone
                x.SenderUserInfo.Nickname == callerNickname).ToArray();//my messages
            return arr.Take(count).Reverse();
        }
    }
}
