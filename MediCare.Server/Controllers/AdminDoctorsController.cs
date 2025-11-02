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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> AdminUpdateDoctor(int id, AdminDoctorUpdateDto dto)
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
            existing.Facility = dto.Facility;
            existing.DoctorDescription = dto.DoctorDescription;
            existing.Role = dto.Role;

            await context.SaveChangesAsync();

            return Ok(existing);
        }

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
    }
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
}
