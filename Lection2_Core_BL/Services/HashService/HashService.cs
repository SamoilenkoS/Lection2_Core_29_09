using Lection2_Core_BL.Options;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace Lection2_Core_BL.Services.HashService
{
    public class HashService : IHashService
    {
        private readonly HashingOptions _hashingOptions;

        public HashService(IOptions<HashingOptions> hashingOptions)
        {
            _hashingOptions = hashingOptions.Value;
        }

        public string GetHash(string password)
            => Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: Convert.FromBase64String(_hashingOptions.Salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _hashingOptions.IterationCount,
                numBytesRequested: _hashingOptions.NumBytesRequested));

        public bool VerifySameHash(string password, string hash)
            => string.Compare(GetHash(password), hash) == 0;
    }
}
