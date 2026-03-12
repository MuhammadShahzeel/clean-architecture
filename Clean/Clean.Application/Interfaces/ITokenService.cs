using System.Security.Claims;

namespace Clean.Application.Interfaces
{
    public interface ITokenService
    {

        string GenerateToken(
            string userId,
            string userName,
            string email,
            IList<string> roles,
            IEnumerable<Claim> additionalClaims);
    }
}
