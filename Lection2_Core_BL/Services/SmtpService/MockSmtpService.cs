using System.Diagnostics;

namespace Lection2_Core_BL.Services.SmtpService
{
    public class MockSmtpService : ISmtpService
    {
        public Task SendEmailAsync(string email, string subject, string messageText)
        {
            Debug.WriteLine(messageText);
            return Task.CompletedTask;
        }
    }
}
