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
                    Specializations = d.Specializations.Select(s => s.SpecializationName).ToList()
                })
                .ToListAsync();

            return Ok(doctors);
        }

        [HttpGet("by-filter")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsByFilter(
            [FromQuery] int? specializationID,
            [FromQuery] string? surname,
            [FromQuery] TimeOnly? availableFrom,
            [FromQuery] TimeOnly? availableUntil)
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
                    Specializations = d.Specializations
                        .Select(s => s.SpecializationName) 
                        .ToList()
                });

            if (!string.IsNullOrEmpty(surname))
                query = query.Where(d =>  d.Surname == surname);

            if (specializationID.HasValue)
            {
                query = query.Where(d => d.Specializations.Any(
                    s => s == context.Specializations
                        .Where(sp => sp.ID == specializationID.Value)
                        .Select(sp => sp.SpecializationName)
                        .FirstOrDefault()
                ));
            }

            if (availableFrom.HasValue)
                query = query.Where(d => d.StartHour <= availableFrom.Value);

            if (availableUntil.HasValue)
                query = query.Where(d => d.EndHour >= availableUntil.Value);

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
                    EndHour = d.EndHour
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

            var hasher = new PasswordHasher<Doctor>();

            var specializations = context.Specializations
                .Where(s => dto.SpecializationIds.Contains(s.ID))
                .ToList();

            Doctor doctor = new Doctor
            {
                Name = dto.Name,
                Email = dto.Email,
                Surname = dto.Surname,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = hasher.HashPassword(null!, dto.Password),
                StartHour = new TimeOnly(8, 0),
                EndHour = new TimeOnly(16, 0),
                Specializations = specializations
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, DoctorUpdateDto dto)
        {
            var existing = await context.Doctors.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = dto.Name;
            existing.Surname = dto.Surname;
            existing.Email = dto.Email;
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
            public List<string> Specializations { get; set; } = new();

        }

        /// <summary>
        /// Data Transfer Object (DTO) used when updating an existing doctor’s details.
        /// </summary>
        public class DoctorUpdateDto
        {
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string Email { get; set; }
            public required string PhoneNumber { get; set; }
            public TimeOnly StartHour { get; set; }
            public TimeOnly EndHour { get; set; }
        }
        public class LoginDto
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
    }
}
