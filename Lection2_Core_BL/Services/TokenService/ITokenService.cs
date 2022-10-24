using System.Security.Claims;

namespace Lection2_Core_BL.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateToken(string username, IEnumerable<string> userRoles);
        ClaimsPrincipal GetPrincipal(string token);
        string ValidateToken(string token);
    }
}