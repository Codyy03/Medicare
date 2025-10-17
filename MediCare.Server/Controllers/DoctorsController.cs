using MediCare.Server.Data;
using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller for managing doctors in the MediCare system.
    /// Provides endpoints to retrieve, register, update, and delete doctor records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        readonly MediCareDbContext context;
        readonly JwtTokenHelper jwtHelper;
        public DoctorsController(MediCareDbContext context, JwtTokenHelper jwtHelper)
        {
            this.context = context;
            this.jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Retrieves all doctors from the system.
        /// </summary>
        /// <returns>A list of <see cref="DoctorDto"/> objects representing all doctors.</returns>
        [HttpGet]
        public async Task<ActionResult<List<DoctorDto>>> GetDoctors()
        {
            var doctors = await context.Doctors
                .Select(d => new DoctorDto
                {
                    ID = d.ID,
                    Name = d.Name,
                    Surname = d.Surname,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    StartHour = d.StartHour,
                    EndHour = d.EndHour,
                    Facility = d.Facility,
                    DoctorDescription = d.DoctorDescription,
                    Specializations = d.Specializations.Select(s => s.SpecializationName).ToList()
                })
                .ToListAsync();

            return Ok(doctors);
        }

        /// <summary>
        /// Retrieves a list of doctors filtered by specialization, surname, and availability hours.
        /// </summary>
        /// <param name="specializationID">
        /// Optional specialization ID to filter doctors by their specialization.
        /// </param>
        /// <param name="surname">
        /// Optional surname (case-insensitive, partial match) to filter doctors.
        /// </param>
        /// <param name="availableFrom">
        /// Optional start time; returns doctors whose working hours start before or at this time.
        /// </param>
        /// <param name="availableUntil">
        /// Optional end time; returns doctors whose working hours end after or at this time.
        /// </param>
        /// <returns>
        /// A 200 OK response containing a filtered list of <see cref="DoctorDto"/> objects.
        /// </returns>
        [HttpGet("by-filter")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsByFilter(
            [FromQuery] int? specializationID,
            [FromQuery] string? surname,
            [FromQuery] TimeOnly? availableAt)

        {
            IQueryable<DoctorDto> query = context.Doctors.Select(d =>
                new DoctorDto
                {
                    ID = d.ID,
                    Name = d.Name,
                    Surname = d.Surname,
                    Email = d.Email,
                    StartHour = d.StartHour,
                    EndHour = d.EndHour,
                    PhoneNumber = d.PhoneNumber,
                    Facility = d.Facility,
                    DoctorDescription = d.DoctorDescription,
                    Specializations = d.Specializations
                        .Select(s => s.SpecializationName) 
                        .ToList()
                });

            if (!string.IsNullOrEmpty(surname))
                query = query.Where(d => d.Surname.ToLower().Contains(surname.ToLower()));

            if (specializationID.HasValue)
            {
                query = query.Where(d => d.Specializations.Any(
                    s => s == context.Specializations
                        .Where(sp => sp.ID == specializationID.Value)
                        .Select(sp => sp.SpecializationName)
                        .FirstOrDefault()
                ));
            }

            if (availableAt.HasValue)
                query = query.Where(d => d.StartHour <= availableAt.Value && d.EndHour >= availableAt.Value);

            List<DoctorDto> result = await query.ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific doctor by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the doctor to retrieve.</param>
        /// <returns>
        /// A 200 OK response with the <see cref="DoctorDto"/> if found; 
        /// otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            var doctor = await context.Doctors
                .Where(d => d.ID == id)
                .Select(d => new DoctorDto
                {
                    ID = d.ID,
                    Name = d.Name,
                    Surname = d.Surname,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    StartHour = d.StartHour,
                    EndHour = d.EndHour,
                    Facility = d.Facility,
                    DoctorDescription = d.DoctorDescription,
                    Specializations = d.Specializations
                        .Select(ds => ds.SpecializationName)
                        .ToList()

                })
                .FirstOrDefaultAsync();

            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        /// <summary>
        /// Registers a new doctor in the system.
        /// </summary>
        /// <param name="dto">
        /// The registration data for the doctor, including personal details, password, and specialization IDs.
        /// </param>
        /// <returns>
        /// A 201 Created response containing the newly created <see cref="DoctorDto"/>; 
        /// or 400 Bad Request if the email is not unique.
        /// </returns>
        [HttpPost("register")]
        public async Task<ActionResult> CreateDoctor(DoctorRegisterDto dto)
        {
            if (await context.Doctors.AnyAsync(d => d.Email == dto.Email))
                return BadRequest("Email must be unique");

            if (dto.SpecializationIds == null || !dto.SpecializationIds.Any())
                return BadRequest("At least one specialization must be selected");

            var passwordErrors = ValidatePassword(dto.Password);
            if (passwordErrors.Any())
                return BadRequest(new { message = string.Join(" ", passwordErrors) });

            var hasher = new PasswordHasher<Doctor>();

            var specializations = context.Specializations
                .Where(s => dto.SpecializationIds.Contains(s.ID))
                .ToList();

            var doctor = new Doctor
            {
                Name = dto.Name,
                Email = dto.Email,
                Surname = dto.Surname,
                PhoneNumber = dto.PhoneNumber,
                StartHour = new TimeOnly(8, 0),
                EndHour = new TimeOnly(16, 0),
                Specializations = specializations,
                Facility = "-",
                DoctorDescription = "-",
                PasswordHash = hasher.HashPassword(null!, dto.Password)
            };


            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.ID }, new DoctorDto
            {
                ID = doctor.ID,
                Name = doctor.Name,
                Surname = doctor.Surname,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber
            });
        }

        /// <summary>
        /// Retrieves the profile information of the currently authenticated doctor.
        /// </summary>
        /// <returns>
        /// A 200 OK response containing the <see cref="DoctorDto"/> with the doctor's details and specializations;  
        /// 401 Unauthorized if the user is not authenticated;  
        /// or 404 Not Found if the doctor does not exist.
        [HttpGet("me")]
        public async Task<ActionResult<DoctorDto>> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var doctor = await context.Doctors.Include(d => d.Specializations).FirstOrDefaultAsync(d => d.ID == int.Parse(userId));
            if (doctor == null) return NotFound();

            var dto = new DoctorDto
            {
                ID = doctor.ID,
                Name = doctor.Name,
                Surname = doctor.Surname,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber,
                EndHour = doctor.EndHour,
                StartHour = doctor.StartHour,
                Specializations = doctor.Specializations.Select(s => s.SpecializationName).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Handles doctor login by verifying provided credentials.
        /// </summary>
        /// <param name="dto">Login data transfer object containing email and password.</param>
        /// <returns>
        /// Returns <see cref="UnauthorizedResult"/> if credentials are invalid, 
        /// otherwise returns an <see cref="OkObjectResult"/> containing a JWT token.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Find doctor
            Doctor? doctor = await context.Doctors.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (doctor == null) return Unauthorized("Invalid credentials");

            // Check password
            PasswordHasher<Doctor> hasher = new PasswordHasher<Doctor>();
            var result = hasher.VerifyHashedPassword(doctor, doctor.PasswordHash, dto.Password);

            if (string.IsNullOrEmpty(doctor.PasswordHash) || (result == PasswordVerificationResult.Failed))
                return Unauthorized("Invalid credentials");

            var token = jwtHelper.GenerateJwtToken(doctor.ID.ToString(), doctor.Email, doctor.Name, "Doctor");
            return Ok(new { token });
        }

        /// <summary>
        /// Updates the details of an existing doctor.
        /// </summary>
        /// <param name="id">The ID of the doctor to update.</param>
        /// <param name="dto">The updated doctor data.</param>
        /// <returns>
        /// A 200 OK response containing the updated <see cref="DoctorDto"/> if successful; 
        /// or 404 Not Found if the doctor does not exist.
        /// </returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDoctor(DoctorUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) 
                return Unauthorized();


            var existing = await context.Doctors.FindAsync(int.Parse(userId));
            if (existing == null)
                return NotFound();


            var phoneNumberErrors = ValidatePhoneNumber(dto.PhoneNumber);
            if (phoneNumberErrors.Any())
                return BadRequest(new { message = string.Join(" ", phoneNumberErrors) });


            var NameErrors = ValidateName(dto.Name);
            if (NameErrors.Any())
                return BadRequest(new { message = string.Join(" ", NameErrors) });


            var SurnameErrors = ValidateSurname(dto.Surname);
            if (SurnameErrors.Any())
                return BadRequest(new { message = string.Join(" ", SurnameErrors) });


            var WorkHoursErrors = ValidateWorkHours(dto.StartHour, dto.EndHour);
            if (WorkHoursErrors.Any())
                return BadRequest(new { message = string.Join(" ", WorkHoursErrors) });

            existing.Name = dto.Name;
            existing.Surname = dto.Surname;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.StartHour = dto.StartHour;
            existing.EndHour = dto.EndHour;

            await context.SaveChangesAsync();

            return Ok(new DoctorDto
            {
                ID = existing.ID,
                Name = existing.Name,
                Surname = existing.Surname,
                Email = existing.Email,
                PhoneNumber = existing.PhoneNumber
            });
        }

        /// <summary>
        /// Resets the password for the currently authenticated doctor.
        /// </summary>
        /// <param name="dto">
        /// A <see cref="PasswordResetDto"/> containing the old password for verification and the new password to be set.
        /// </param>
        /// <returns>
        /// A 204 No Content response if the password reset is successful;  
        /// 400 Bad Request if the old password is incorrect, the new password is invalid, or matches the old one;  
        /// 401 Unauthorized if the user is not authenticated;  
        /// 404 Not Found if the doctor does not exist.
        /// </returns>
        [HttpPut("password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Unauthorized access." });

            var doctor = await context.Doctors.FindAsync(int.Parse(userId));
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var hasher = new PasswordHasher<Doctor>();

            var verifyResult = hasher.VerifyHashedPassword(doctor, doctor.PasswordHash, dto.OldPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
                return BadRequest(new { message = "Old password is incorrect." });

            var newPasswordCheck = hasher.VerifyHashedPassword(doctor, doctor.PasswordHash, dto.NewPassword);
            if (newPasswordCheck == PasswordVerificationResult.Success)
                return BadRequest(new { message = "New password cannot be the same as the old one." });

            var passwordErrors = ValidatePassword(dto.NewPassword);
            if (passwordErrors.Any())
                return BadRequest(new { message = string.Join(" ", passwordErrors) });

            doctor.PasswordHash = hasher.HashPassword(doctor, dto.NewPassword);
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
        List<string> ValidatePassword(string password)
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
        /// Validates a phone number to ensure it is not empty, contains only digits,
        /// has exactly 9 digits, and does not start with zero.
        /// </summary>
        /// <param name="phoneNumber">The phone number to validate.</param>
        /// <returns>A list of validation error messages, or empty if valid.</returns>
        List<string> ValidatePhoneNumber(string phoneNumber)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add("Phone number is required.");
                return errors;
            }
            
            if (!Regex.IsMatch(phoneNumber, @"^\d+$"))
            {
                errors.Add("Phone number must contain only digits.");
            }
            if (!Regex.IsMatch(phoneNumber, @"^\d{9}$"))
            {
                errors.Add("Phone number must be 9 digits long.");
            }
            if (phoneNumber.StartsWith("0"))
            {
                errors.Add("Phone number cannot start with zero.");
            }

            return errors;
        }

        /// <summary>
        /// Validates a name to ensure it is not empty and contains only letters.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <returns>A list of validation error messages, or empty if valid.</returns>
        List<string> ValidateName(string name)
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
        List<string> ValidateSurname(string surname)
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
        /// Validates working hours to ensure both start and end times are provided
        /// and that the start time is earlier than the end time.
        /// </summary>
        /// <param name="startHour">The start hour of work.</param>
        /// <param name="endHour">The end hour of work.</param>
        /// <returns>A list of validation error messages, or empty if valid.</returns>
        List<string> ValidateWorkHours(TimeOnly? startHour, TimeOnly? endHour)
        {
            var errors = new List<string>();
            if (startHour == null || endHour == null)
            {
                errors.Add("Start hour and end hour are required.");
                return errors;
            }

            if (startHour >= endHour)
            {
                errors.Add("Start hour must be earlier than end hour.");
            }
            return errors;
        }

        /// <summary>
        /// Deletes a doctor by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the doctor to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; 
        /// or 404 Not Found if the doctor does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            Doctor? doctor = await context.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound();

            context.Doctors.Remove(doctor);
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Data Transfer Object (DTO) used when registering a new doctor.
        /// Includes personal details, password, and specialization IDs.
        /// </summary>
        public class DoctorRegisterDto
        {
            [Required]
            public required string Name { get; set; }
            [Required]
            public required string Surname { get; set; }
            [Required, EmailAddress]
            public required string Email { get; set; }
            [Phone]
            public required string PhoneNumber { get; set; }
            [Required]
            public required string Password { get; set; }

            [MinLength(1, ErrorMessage = "At least one specialization is required")]
            public List<int> SpecializationIds { get; set; } = new();
        }

        /// <summary>
        /// Data Transfer Object (DTO) for returning doctor information.
        /// Excludes sensitive fields such as password hash.
        /// </summary>
        public class DoctorDto
        {
            public int ID { get; set; }
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string Email { get; set; }
            public required string PhoneNumber { get; set; }
            public TimeOnly StartHour { get; set; }
            public TimeOnly EndHour { get; set; }
            public string? Facility { get; set; }
            public string? DoctorDescription { get; set; }
            public List<string> Specializations { get; set; } = new();
        }

        /// <summary>
        /// Data Transfer Object (DTO) used when updating an existing doctor’s details.
        /// </summary>
        public class DoctorUpdateDto
        {
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string PhoneNumber { get; set; }
            public TimeOnly StartHour { get; set; }
            public TimeOnly EndHour { get; set; }
        }

        /// <summary>
        /// Data transfer object for doctor login.
        /// </summary>
        public class LoginDto
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        /// <summary>
        /// Data transfer object for resetting a doctor's password.
        /// </summary>
        public class PasswordResetDto
        {
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }
    }
}
