using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public DoctorsController(MediCareDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves all doctors.
        /// </summary>
        /// <returns>A list of <see cref="DoctorDto"/> objects.</returns>
        [HttpGet]
        public ActionResult<List<DoctorDto>> GetDoctors()
        {
            var doctors = context.Doctors
                .Select(d => new DoctorDto
                {
                    ID = d.ID,
                    Name = d.Name,
                    Surname = d.Surname,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    StartHour = d.StartHour,
                    EndHour = d.EndHour,
                })
                .ToList();

            return Ok(doctors);
        }

        /// <summary>
        /// Retrieves a specific doctor by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the doctor.</param>
        /// <returns>
        /// The <see cref="DoctorDto"/> object if found; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult<DoctorDto> GetDoctor(int id)
        {
            var doctor = context.Doctors
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
                .FirstOrDefault();

            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        /// <summary>
        /// Registers a new doctor in the system.
        /// </summary>
        /// <param name="dto">The registration data for the doctor, including personal details, password, and specialization IDs.</param>
        /// <returns>
        /// A 201 Created response containing the newly created <see cref="DoctorDto"/> object.
        /// </returns>
        [HttpPost("register")]
        public async Task<ActionResult> CreateDoctor(DoctorRegisterDto dto)
        {
            if (context.Doctors.Any(d => d.Email == dto.Email))
                return BadRequest("Email have to unique");

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
                StartHour = new TimeOnly(8),
                EndHour = new TimeOnly(16),
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
        /// Updates an existing doctor record.
        /// </summary>
        /// <param name="id">The unique identifier of the doctor to update.</param>
        /// <param name="doctor">The updated doctor object.</param>
        /// <returns>
        /// The updated <see cref="DoctorDto"/> if successful; otherwise, an appropriate error response.
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
        /// Deletes a doctor record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the doctor to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            Doctor? doctor = context.Doctors.Find(id);

            if (doctor == null)
                return NotFound();

            context.Doctors.Remove(doctor);
            context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// DTO used for doctor registration.
        /// </summary>
        public class DoctorRegisterDto
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
            public List<int> SpecializationIds { get; set; }
        }
        
        /// <summary>
         /// DTO used for returning doctor data (without sensitive fields).
         /// </summary>
        public class DoctorDto
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public TimeOnly StartHour { get; set; }
            public TimeOnly EndHour { get; set; }
        }

        public class DoctorUpdateDto
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public TimeOnly StartHour { get; set; }
            public TimeOnly EndHour { get; set; }
        }

    }
}
