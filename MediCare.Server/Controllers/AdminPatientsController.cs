using MediCare.Server.Data;
using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static MediCare.Server.Entities.Enums;

namespace MediCare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminPatientsController : ControllerBase
    {
        private readonly MediCareDbContext context;

        public AdminPatientsController(MediCareDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves all patients from the system.
        /// Accessible by administrators.
        /// </summary>
        /// <returns>List of Patient entities.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Patient>>> GetAll()
        {
            var patients = await context.Patients.ToListAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Retrieves a specific patient by ID.
        /// </summary>
        /// <param name="id">Patient ID.</param>
        /// <returns>Patient entity if found.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetById(int id)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        /// <summary>
        /// Creates a new patient account.
        /// </summary>
        /// <param name="model">Patient registration model.</param>
        /// <returns>201 Created if successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create(AdminPatientRegisterModel model)
        {
            if (await context.Patients.AnyAsync(p => p.PESEL == model.PESEL))
                return BadRequest("PESEL must be unique");

            if (await context.Patients.AnyAsync(p => p.Email == model.Email))
                return BadRequest("Email must be unique");

            var passwordErrors = ValidatePassword(model.Password);
            if (passwordErrors.Any())
                return BadRequest(new { message = string.Join(" ", passwordErrors) });

            var hasher = new PasswordHasher<Patient>();

            var patient = new Patient
            {
                PESEL = model.PESEL,
                Name = model.Name,
                Surname = model.Surname,
                Birthday = model.Birthday,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = hasher.HashPassword(null!, model.Password),
                Status = Status.Active
            };

            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Updates an existing patient.
        /// </summary>
        /// <param name="id">Patient ID.</param>
        /// <param name="model">Updated patient data.</param>
        /// <returns>200 OK if successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AdminPatientUpdateModel model)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            patient.Name = model.Name;
            patient.Surname = model.Surname;
            patient.PESEL = model.PESEL;
            patient.Birthday = model.Birthday;
            patient.Email = model.Email;
            patient.PhoneNumber = model.PhoneNumber;
            patient.Status = model.Status;

            await context.SaveChangesAsync();
            return Ok(patient);
        }

        /// <summary>
        /// Deletes a patient from the system.
        /// </summary>
        /// <param name="id">Patient ID.</param>
        /// <returns>204 No Content if successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            context.Patients.Remove(patient);
            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Validates password strength.
        /// </summary>
        private List<string> ValidatePassword(string password)
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
    /// Model used for registering a new patient by an admin.
    /// </summary>
    public class AdminPatientRegisterModel
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
    /// Model used for updating an existing patient by an admin.
    /// </summary>
    public class AdminPatientUpdateModel
    {
        [Required]
        public string PESEL { get; set; }

        [Required]
        public string Name { get; set; } 

        [Required]
        public string Surname { get; set; } 

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public Status Status { get; set; }
    }
}
