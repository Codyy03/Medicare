using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        public PatientsController(MediCareDbContext context)
        {
            this.context = context;
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
                    Name = d.Name,
                    Surname = d.Surname,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    Birthday = d.Birthday,
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
                  Name = d.Name,
                  Surname = d.Surname,
                  Email = d.Email,
                  PhoneNumber = d.PhoneNumber,
                  Birthday = d.Birthday,
              })
              .FirstOrDefaultAsync();

            if (patient == null) 
                return NotFound();

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
                PasswordHash = hasher.HashPassword(null!, dto.Password)
            };

            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.ID }, new{
                patient.ID,
                patient.Name,
                patient.Surname,
                patient.Email
            });
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, PatientUpdateDto dto)
        {
            var existing = await context.Patients.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = dto.Name;
            existing.Surname = dto.Surname;
            existing.PESEL = dto.PESEL;
            existing.Birthday = dto.Birthday;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;

            await context.SaveChangesAsync();

            return Ok(new PatientDto
            {
                Name = existing.Name,
                Surname = existing.Surname,
                Email = existing.Email,
                PhoneNumber = existing.PhoneNumber,
                Birthday = existing.Birthday,
            });
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
    }

    /// <summary>
    /// Data Transfer Object (DTO) used when registering a new patient.
    /// Includes personal details, PESEL, contact information, and password.
    /// </summary>
    public class PatientRegisterDto
    {
        [Required]
        public string PESEL { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
    }

    /// <summary>
    /// Data Transfer Object (DTO) for returning patient information.
    /// Excludes sensitive fields such as password hash.
    /// </summary>
    public class PatientDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
    }

    /// <summary>
    /// Data Transfer Object (DTO) used when updating an existing patient’s details.
    /// </summary>
    public class PatientUpdateDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PESEL { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
