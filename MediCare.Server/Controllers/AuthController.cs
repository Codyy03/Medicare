using MediCare.Server.Data;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller responsible for authentication and token management in the MediCare system.
    /// Provides endpoints for refreshing JWT access tokens using valid refresh tokens.
    /// </summary>
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

        /// <summary>
        /// Issues a new access token and refresh token pair based on a valid refresh token.
        /// </summary>
        /// <param name="dto">
        /// The request DTO containing the refresh token to validate.
        /// </param>
        /// <returns>
        /// 200 OK with a new access token and refresh token if the provided refresh token is valid and not expired.  
        /// 401 Unauthorized if the token is invalid, expired, or does not belong to a recognized user.
        /// </returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var tokenEntity = await context.RefreshTokens
                .Include(rt => rt.Patient)
                .Include(rt => rt.Doctor)
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && !rt.IsRevoked);

            if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            string role, email, name;
            int id;

            if (tokenEntity.Patient != null)
            {
                role = tokenEntity.Patient.Role.ToString();
                email = tokenEntity.Patient.Email;
                name = tokenEntity.Patient.Name;
                id = tokenEntity.Patient.ID;
            }
            else if (tokenEntity.Doctor != null)
            {
                role = tokenEntity.Doctor.Role.ToString();
                email = tokenEntity.Doctor.Email;
                name = tokenEntity.Doctor.Name;
                id = tokenEntity.Doctor.ID;
            }
            else
            {
                return Unauthorized("Invalid token owner");
            }

            var accessToken = jwtHelper.GenerateJwtToken(id.ToString(), email, name, role);

            var newRefreshToken = jwtHelper.GenerateRefreshToken();
            tokenEntity.Token = newRefreshToken;
            tokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();

            return Ok(new { accessToken, refreshToken = newRefreshToken });
        }

        /// <summary>
        /// DTO used when requesting a new access token using a refresh token.
        /// </summary>
        public class RefreshRequestDto
        {
            public string RefreshToken { get; set; }
        }
    }
}
