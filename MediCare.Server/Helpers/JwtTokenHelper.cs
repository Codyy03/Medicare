using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediCare.Server.Helpers
{
    /// <summary>
    /// Provides functionality for generating JSON Web Tokens (JWT) 
    /// with user claims for authentication and authorization.
    /// </summary>
    public class JwtTokenHelper
    {
        private readonly IConfiguration configuration;

        public JwtTokenHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Generates a signed JWT token containing user claims.
        /// </summary>
        /// <param name="id">Unique identifier of the user.</param>
        /// <param name="email">Email address of the user.</param>
        /// <param name="name">Display name of the user.</param>
        /// <param name="role">Role of the user (e.g. "Patient", "Doctor").</param>
        /// <returns>A signed JWT token string.</returns>
        public string GenerateJwtToken(string id, string email, string name, string role)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim("name", name),
            new Claim("role", role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(60), //  DateTime.UtcNow.AddSeconds(60) // DateTime.UtcNow.AddHours(1)
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
