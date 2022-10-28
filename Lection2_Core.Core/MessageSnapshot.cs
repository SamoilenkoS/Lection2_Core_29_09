namespace Lection2_Core.Core
{
    public class MessageSnapshot
    {
        public string SenderId { get; set; }
        public string Message { get; set; }
        public bool IsPersonal { get; set; }
        public string? ReceiverId { get; set; }
    }
}