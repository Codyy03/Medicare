using MediCare.Server.Data;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        readonly MediCareDbContext context;
        readonly JwtTokenHelper jwtHelper;
        public AuthController(MediCareDbContext context, JwtTokenHelper jwtHelper)
        {
            this.context = context;
            this.jwtHelper = jwtHelper;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var tokenEntity = await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && !rt.IsRevoked);

            if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            string role, email, name;
            int id;

            if (tokenEntity.Patient != null)
            {
                role = "Patient";
                email = tokenEntity.Patient.Email;
                name = tokenEntity.Patient.Name;
                id = tokenEntity.Patient.ID;
            }
            else if (tokenEntity.Doctor != null)
            {
                role = "Doctor";
                email = tokenEntity.Doctor.Email;
                name = tokenEntity.Doctor.Name;
                id = tokenEntity.Doctor.ID;
            }
            else
            {
                return Unauthorized("Invalid token owner");
            }

            var accessToken = jwtHelper.GenerateJwtToken(id.ToString(), email, name, role);

            // opcjonalnie: nowy refresh token
            var newRefreshToken = jwtHelper.GenerateRefreshToken();
            tokenEntity.Token = newRefreshToken;
            tokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();

            return Ok(new { accessToken, refreshToken = newRefreshToken });
        }
        public class RefreshRequestDto
        {
            public string RefreshToken { get; set; }
        }

    }
}
