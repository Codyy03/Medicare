using MediCare.Server.Data;
using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static MediCare.Server.Controllers.DoctorsController;
using static MediCare.Server.Entities.Enums;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// Admin-only API controller for managing doctors in the MediCare system.
    /// Provides endpoints to create, update, and delete doctor records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDoctorsController : ControllerBase
    {
        readonly MediCareDbContext context;
        readonly JwtTokenHelper jwtHelper;
        public AdminDoctorsController(MediCareDbContext context, JwtTokenHelper jwtHelper)
        {
            this.context = context;
            this.jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Retrieves a specific doctor by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the doctor to retrieve.</param>
        /// <returns>
        /// A 200 OK response with the <see cref="DoctorDto"/> if found; 
        /// otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("doctorAdmin/{id}")]
        public async Task<ActionResult<DoctoAdminrDto>> GetDoctor(int id)
        {
            var doctor = await context.Doctors
                .Include(d => d.Specializations)
                .Where(d => d.ID == id)
                .Select(d => new DoctoAdminrDto
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
                        .Select(ds => ds.ID)
                        .ToList(),
                    Role = (int)d.Role
                })
                .FirstOrDefaultAsync();

            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        /// <summary>
        /// Updates an existing doctor record with new details.
        /// Only accessible by users with the Admin role.
        /// </summary>
        /// <param name="id">The ID of the doctor to update.</param>
        /// <param name="dto">The DTO containing updated doctor information.</param>
        /// <returns>
        /// 200 OK with the updated doctor entity if successful.  
        /// 404 Not Found if the doctor with the given ID does not exist.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> AdminUpdateDoctor(int id, AdminDoctorUpdateDto dto)
        {
            var existing = await context.Doctors
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (existing == null)
                return NotFound();

            existing.Name = dto.Name;
            existing.Surname = dto.Surname;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.StartHour = dto.StartHour;
            existing.EndHour = dto.EndHour;
            existing.Facility = dto.Facility;
            existing.DoctorDescription = dto.DoctorDescription;
            existing.Role = dto.Role;

            existing.Specializations.Clear();

            var specializations = await context.Specializations
                .Where(s => dto.SpecializationsIds.Contains(s.ID))
                .ToListAsync();

            foreach (var spec in specializations)
            {
                existing.Specializations.Add(spec);
            }

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Creates a new doctor account with all required details.
        /// Accessible by Admins to register doctors directly.
        /// </summary>
        /// <param name="dto">The DTO containing doctor registration details.</param>
        /// <returns>
        /// 204 No Content if the doctor was successfully created.  
        /// 400 Bad Request if validation fails (e.g., duplicate email, invalid password, missing specialization).
        /// </returns>
        [HttpPost("adminDoctorRegister")]
        public async Task<ActionResult> CreateDoctor(AdminDoctorRegisterDto dto)
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
                StartHour = dto.StartHour,
                EndHour = dto.EndHour,
                Role = dto.Role,
                Specializations = specializations,
                Facility = dto.Facility,
                DoctorDescription = dto.DoctorDescription,
                PasswordHash = hasher.HashPassword(null!, dto.Password)
            };

            context.Doctors.Add(doctor);
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
    }

    /// <summary>
    /// Data Transfer Object used for updating an existing doctor by an Admin.
    /// </summary>
    public class AdminDoctorUpdateDto
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public TimeOnly StartHour { get; set; }
        public TimeOnly EndHour { get; set; }
        public string Facility { get; set; } = string.Empty;
        public string DoctorDescription { get; set; } = string.Empty;
        public Role Role { get; set; }

        public List<int> SpecializationsIds { get; set; } = new();
    }

    /// <summary>
    /// Data Transfer Object used for registering a new doctor by an Admin.
    /// </summary>
    public class AdminDoctorRegisterDto
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

        [Required]
        public required TimeOnly StartHour { get; set; }

        [Required]
        public required TimeOnly EndHour { get; set; }

        [Required, StringLength(255)]
        public required string Facility { get; set; }

        [Required]
        public required string DoctorDescription { get; set; }

        [Required]
        public Role Role { get; set; } = Role.Doctor;
    }

    /// <summary>
    /// Data Transfer Object (DTO) for returning doctor information.
    /// Excludes sensitive fields such as password hash.
    /// </summary>
    public class DoctoAdminrDto
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
        public List<int> Specializations { get; set; } = new();
        public int Role { get; set; }
    }
}
