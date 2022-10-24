namespace Lection2_Core_BL.Services.HashService
{
    public interface IHashService
    {
        string GetHash(string password);
        bool VerifySameHash(string password, string hash);
    }
}