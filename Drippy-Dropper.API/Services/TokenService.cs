using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Drippy_Dropper.API.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Drippy_Dropper.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username),
            };

            var keyString = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new ArgumentNullException(nameof(keyString), "JwtSettings:Key nie może być nullem.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var expiresString = _configuration["JwtSettings:Expires"];
            if (string.IsNullOrEmpty(expiresString))
            {
                throw new ArgumentNullException(nameof(expiresString), "JwtSettings:Expires nie może być nullem.");
            }

            if (!double.TryParse(expiresString, out double expiresMinutes))
            {
                throw new ArgumentException("JwtSettings:Expires musi być liczbą");
            }

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
