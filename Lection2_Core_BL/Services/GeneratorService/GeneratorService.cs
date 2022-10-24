using System.Text;

namespace Lection2_Core_BL.Services.GeneratorService
{
    public class GeneratorService : IGeneratorService
    {
        public string GetRandomString()
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLower();
            var length = random.Next(10, 20);
            var result = new StringBuilder(string.Empty, length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
