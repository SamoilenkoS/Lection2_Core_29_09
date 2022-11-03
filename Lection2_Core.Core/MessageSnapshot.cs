namespace Lection2_Core.Core
{
    public class MessageSnapshot
    {
        public PublicUserInfo SenderUserInfo { get; set; }
        public string Message { get; set; }
        public bool IsPersonal { get; set; }
        public string? ReceiverNickname { get; set; }
    }
}