
namespace Lection2_Core.Core
{
    public interface ISignalRClient
    {
        Task GetMessage(string message);
    }
}
