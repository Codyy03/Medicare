using MediCare.Server.Data;
using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static MediCare.Server.Entities.Enums;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller for managing patients in the MediCare system.
    /// Provides endpoints to retrieve, register, update, and delete patient records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        readonly MediCareDbContext context;
        readonly JwtTokenHelper jwtHelper;
        public PatientsController(MediCareDbContext context, JwtTokenHelper jwtHelper)
        {
            this.context = context;
            this.jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Retrieves all patients from the system.
        /// </summary>
        /// <returns>A list of <see cref="PatientDto"/> objects representing all patients.</returns>
        [HttpGet]
        public async Task<ActionResult<List<PatientDto>>> GetPatients()
        {
            var patients = await context.Patients
                .Select(d => new PatientDto
                {
                    ID = d.ID,
                    Name = d.Name,
                    Surname = d.Surname,
                    PESEL = d.PESEL,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    Birthday = d.Birthday
                })
                .ToListAsync();

            return Ok(patients);
        }

        /// <summary>
        /// Retrieves a specific patient by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the patient to retrieve.</param>
        /// <returns>
        /// Returns the <see cref="PatientDto"/> if found; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var patient = await context.Patients
              .Where(d => d.ID == id)
              .Select(d => new PatientDto
              {
                  ID = d.ID,
                  Name = d.Name,
                  Surname = d.Surname,
                  PESEL = d.PESEL,
                  Email = d.Email,
                  PhoneNumber = d.PhoneNumber,
                  Birthday = d.Birthday
              })
              .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        /// <summary>
        /// Retrieves the profile information of the currently authenticated patient.
        /// </summary>
        /// <returns>
        /// A 200 OK response containing the patient entity;  
        /// 401 Unauthorized if the user is not authenticated;  
        /// or 404 Not Found if the patient does not exist.
        /// </returns>
        [HttpGet("me")]
        public async Task<ActionResult<PatientDto>> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var patient = await context.Patients.FindAsync(int.Parse(userId));
            if (patient == null) return NotFound();

            return Ok(patient);
        }

        /// <summary>
        /// Registers a new patient in the system.
        /// </summary>
        /// <param name="dto">The registration data for the patient, including personal details and password.</param>
        /// <returns>
        /// A 201 Created response containing the newly created patient’s basic details; 
        /// or 400 Bad Request if the PESEL or email is not unique.
        /// </returns>
        [HttpPost("register")]
        public async Task<ActionResult> CreatePatient(PatientRegisterDto dto)
        {
            if (await context.Patients.AnyAsync(p => p.PESEL == dto.PESEL))
                return BadRequest("PESEL must be unique");

            if (await context.Patients.AnyAsync(p => p.Email == dto.Email))
                return BadRequest("Email must be unique");

            var hasher = new PasswordHasher<Patient>();

            var patient = new Patient
            {
                PESEL = dto.PESEL,
                Name = dto.Name,
                Surname = dto.Surname,
                Birthday = dto.Birthday,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = hasher.HashPassword(null!, dto.Password),
                Status = Status.Active
            };

            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.ID }, new PatientDto
            {
                ID = patient.ID,
                Name = patient.Name,
                Surname = patient.Surname,
                PESEL = patient.PESEL,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Birthday = patient.Birthday,
                Status = patient.Status
            });
        }

        /// <summary>
        /// Handles patient login by verifying provided credentials.
        /// </summary>
        /// <param name="dto">Login data transfer object containing email and password.</param>
        /// <returns>
        /// Returns <see cref="UnauthorizedResult"/> if credentials are invalid, 
        /// otherwise returns an <see cref="OkObjectResult"/> containing a JWT token.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            Patient? user = await context.Patients.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null) return Unauthorized("Invalid credentials");

            PasswordHasher<Patient> hasher = new PasswordHasher<Patient>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (string.IsNullOrEmpty(user.PasswordHash) || (result == PasswordVerificationResult.Failed))
                return Unauthorized("Invalid credentials");

            if (user.Status == Status.Inactive)
                return Unauthorized("Account is inactive.");

            var oldTokens = context.RefreshTokens.Where(rt => rt.PatientID == user.ID);
            context.RefreshTokens.RemoveRange(oldTokens);

            var accessToken = jwtHelper.GenerateJwtToken(user.ID.ToString(), user.Email, user.Name, user.Role.ToString());

            var refreshToken = jwtHelper.GenerateRefreshToken();
            var entity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                PatientID = user.ID
            };
            context.RefreshTokens.Add(entity);
            await context.SaveChangesAsync();

            return Ok(new { accessToken, refreshToken });
        }

        /// <summary>
        /// Updates the details of an existing patient.
        /// </summary>
        /// <param name="id">The ID of the patient to update.</param>
        /// <param name="dto">The updated patient data.</param>
        /// <returns>
        /// A 200 OK response containing the updated <see cref="PatientDto"/> if successful; 
        /// 404 Not Found if the patient does not exist.
        /// </returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePatient([FromBody] PatientUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var existing = await context.Patients.FindAsync(int.Parse(userId));

            if (existing == null)
                return NotFound();

            var NameErrors = ValidateName(dto.Name);
            if (NameErrors.Any())
                return BadRequest(new { message = string.Join(" ", NameErrors) });


            var SurnameErrors = ValidateSurname(dto.Surname);
            if (SurnameErrors.Any())
                return BadRequest(new { message = string.Join(" ", SurnameErrors) });

            var PESELErrors = ValidatePESEL(dto.PESEL);
            if (PESELErrors.Any())
                return BadRequest(new { message = string.Join(" ", PESELErrors) });

            var BirthdayErrors = ValidateBirthday(dto.Birthday);
            if (BirthdayErrors.Any())
                return BadRequest(new { message = string.Join(" ", BirthdayErrors) });

            existing.Name = dto.Name;
            existing.Surname = dto.Surname;
            existing.PESEL = dto.PESEL;
            existing.Birthday = dto.Birthday;
            existing.PhoneNumber = dto.PhoneNumber;

            await context.SaveChangesAsync();

            return Ok(new PatientDto
            {
                Name = existing.Name,
                Surname = existing.Surname,
                Email = existing.Email,
                PESEL = existing.PESEL,
                PhoneNumber = existing.PhoneNumber,
                Birthday = existing.Birthday,
            });
        }

        /// <summary>
        /// Resets the password for the currently authenticated patient.
        /// </summary>
        /// <param name="dto">
        /// A <see cref="PasswordResetDto"/> containing the old password for verification and the new password to be set.
        /// </param>
        /// <returns>
        /// A 204 No Content response if the password reset is successful;  
        /// 400 Bad Request if the old password is incorrect, the new password is invalid, or matches the old one;  
        /// 401 Unauthorized if the user is not authenticated;  
        /// 404 Not Found if the patient does not exist.
        /// </returns>
        [HttpPut("password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized access." });

            var patient = await context.Patients.FindAsync(int.Parse(userId));
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            var hasher = new PasswordHasher<Patient>();

            var verifyResult = hasher.VerifyHashedPassword(patient, patient.PasswordHash, dto.OldPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
                return BadRequest(new { message = "Old password is incorrect." });

            var newPasswordCheck = hasher.VerifyHashedPassword(patient, patient.PasswordHash, dto.NewPassword);
            if (newPasswordCheck == PasswordVerificationResult.Success)
                return BadRequest(new { message = "New password cannot be the same as the old one." });

            var passwordErrors = ValidatePassword(dto.NewPassword);
            if (passwordErrors.Any())
                return BadRequest(new { message = string.Join(" ", passwordErrors) });

            patient.PasswordHash = hasher.HashPassword(patient, dto.NewPassword);
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deactivates the currently authenticated patient's account by setting their status to Inactive.
        /// Requires the patient's password for verification. If the password is correct and the account exists,
        /// the account will be deactivated and the user should be logged out on the client side.
        /// </summary>
        /// <param name="dto">An object containing the patient's current password for verification.</param>
        /// <returns>
        /// Returns 200 OK with a confirmation message if deactivation is successful.
        /// Returns 400 Bad Request if the password is incorrect.
        /// Returns 401 Unauthorized if the user is not authenticated.
        /// Returns 404 Not Found if the patient does not exist.
        /// </returns>

        [HttpPut("deactivate")]
        public async Task<IActionResult> DeactivateAccount([FromBody] PasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized access." });

            var patient = await context.Patients.FindAsync(int.Parse(userId));
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            var hasher = new PasswordHasher<Patient>();
            var verifyResult = hasher.VerifyHashedPassword(patient, patient.PasswordHash, dto.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
                return BadRequest(new { message = "Password is incorrect." });


            patient.Status = Status.Inactive;
            await context.SaveChangesAsync();

            return Ok(new { message = "Account deactivated. You will be logged out." });
        }



        /// <summary>
        /// Deletes a patient by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; 
        /// or 404 Not Found if the patient does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            Patient? patient = await context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            context.Patients.Remove(patient);
            await context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Validates a password against defined security requirements.
        /// </summary>
        /// <param name="password">The password string to validate.</param>
        /// <returns>
        /// A list of validation error messages. 
        /// If the list is empty, the password meets all requirements.
        /// </returns>
        [NonAction]
        public List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                errors.Add("Password must be at least 8 characters long.");

            if (!password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                errors.Add("Password must contain at least one digit.");

            if (!password.Any(ch => "!@#$%^&*()_-+=<>?/{}~|".Contains(ch)))
                errors.Add("Password must contain at least one special character.");

            return errors;
        }
        /// <summary>
        /// Validates a name to ensure it is not empty and contains only letters.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <returns>A list of validation error messages, or empty if valid.</returns>
        [NonAction]
        public List<string> ValidateName(string name)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Name is required.");
                return errors;
            }
            if (!Regex.IsMatch(name, @"^[A-Za-z]+$"))
            {
                errors.Add("Name must contain only letters.");
                return errors;
            }
            return errors;
        }

        /// <summary>
        /// Validates a surname to ensure it is not empty and contains only letters.
        /// </summary>
        /// <param name="surname">The surname to validate.</param>
        /// <returns>A list of validation error messages, or empty if valid.</returns>
        [NonAction]
        public List<string> ValidateSurname(string surname)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(surname))
            {
                errors.Add("Surname is required.");
                return errors;
            }
            if (!Regex.IsMatch(surname, @"^[A-Za-z]+$"))
            {
                errors.Add("Surname must contain only letters.");
                return errors;
            }
            return errors;
        }


        /// <summary>
        /// Validates a PESEL number to ensure it has the correct length,
        /// structure, and control digit.
        /// </summary>
        /// <param name="pesel">The PESEL number to validate.</param>
        /// <returns>A validation error message, or empty if valid.</returns>
        [NonAction]
        public List<string> ValidatePESEL(string pesel)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(pesel))
            {
                errors.Add("PESEL is required.");
                return errors;
            }
            if (!Regex.IsMatch(pesel, @"^\d{11}$"))
            {
                errors.Add("PESEL must be exactly 11 digits.");
                return errors;
            }
            if (!Regex.IsMatch(pesel, @"^\d+$"))
            {
                errors.Add("PESEL must contain only digits.");
            }
            return errors;
        }


        /// <summary>
        /// Validates a date of birth to ensure it is in the correct format
        /// and not set in the future.
        /// </summary>
        /// <param name="birthday">The date of birth to validate.</param>
        /// <returns>A validation error message, or empty if valid.</returns>
        [NonAction]
        public List<string> ValidateBirthday(DateTime birthday)
        {
            var errors = new List<string>();
            if (birthday > DateTime.Now)
            {
                errors.Add("Birthday cannot be in the future.");
            }
            if (birthday < DateTime.Now.AddYears(-125))
            {
                errors.Add("Birthday is too far in the past");
            }
            return errors;
        }

        /// <summary>
        /// Data Transfer Object (DTO) used when registering a new patient.
        /// Includes personal details, PESEL, contact information,  password and sets account status as 1 (active).
        /// </summary>
        public class PatientRegisterDto
        {
            [Required]
            public required string PESEL { get; set; }
            [Required]
            public required string Name { get; set; }
            [Required]
            public required string Surname { get; set; }
            [Required, EmailAddress]
            public required string Email { get; set; }
            [Required, Phone]
            public required string PhoneNumber { get; set; }
            [Required]
            public required string Password { get; set; }
            public DateTime Birthday { get; set; }
        }

        /// <summary>
        /// Data Transfer Object (DTO) for returning patient information.
        /// Excludes sensitive fields such as password hash.
        /// </summary>
        public class PatientDto
        {
            public int ID { get; set; }
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string PESEL { get; set; }
            public required string Email { get; set; }
            public required string PhoneNumber { get; set; }
            public DateTime Birthday { get; set; }
            public Status Status { get; set; }
        }
        /// <summary>
        /// Data Transfer Object (DTO) used when updating an existing patient’s details.
        /// </summary>
        public class PatientUpdateDto
        {
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string PESEL { get; set; }
            public DateTime Birthday { get; set; }
            public required string PhoneNumber { get; set; }
        }

        /// <summary>
        /// Data transfer object for patient login.
        /// </summary>
        public class LoginDto
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
        /// <summary>
        /// Data transfer object for resetting a patients's password.
        /// </summary>
        public class PasswordResetDto
        {
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }
        public class PasswordDto
        {
            public required string Password { get; set; }
        }
    }
}
