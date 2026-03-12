using Clean.Application.Interfaces;
using Clean.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Clean.Persistence.SharedServices
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public string GenerateToken(
            string userId,
            string userName,
            string email,
            IList<string> roles,
            IEnumerable<Claim> additionalClaims)
        {
            if (string.IsNullOrWhiteSpace(_jwtSettings.Key))
                throw new InvalidOperationException("JWT signing key is not configured.");

            var roleClaims = roles?.Select(r => new Claim(ClaimTypes.Role, r)) ?? Enumerable.Empty<Claim>();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
                new Claim(ClaimTypes.Name, userName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Iat,
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64)
            };

            if (additionalClaims != null)
                claims.AddRange(additionalClaims);

            claims.AddRange(roleClaims);

            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var symmetricKey = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
