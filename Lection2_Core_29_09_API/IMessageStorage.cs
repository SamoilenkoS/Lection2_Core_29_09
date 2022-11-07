using Lection2_Core.Core;

namespace Lection2_Core_API
{
    public interface IMessageStorage
    {
        void Add(MessageSnapshot message);
        IEnumerable<MessageSnapshot> GetRecent(string callerNickname, int count = 50);
    }
}