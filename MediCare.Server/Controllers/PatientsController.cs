using Microsoft.AspNetCore.Mvc;
using MediCare.Server.Data;
using MediCare.Server.Entities;

namespace MediCare.Server.Controllers
{    
     /// <summary>
     /// API controller for managing patients in the MediCare system.
     /// Provides endpoints to retrieve, create, update, and delete patient records.
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
        /// Retrieves all patients.
        /// </summary>
        /// <returns>A list of <see cref="Patient"/> objects.</returns>
        [HttpGet]
        public ActionResult<List<Patient>> GetPatients()
        {
            List<Patient> patients = context.Patients.ToList();
            return Ok(patients);
        }

        /// <summary>
        /// Retrieves a specific patient by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the patient.</param>
        /// <returns>
        /// The <see cref="Patient"/> object if found; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatient(int id)
        {
            Patient? patient = context.Patients.Find(id);

            if (patient == null) 
                return NotFound();

            return Ok(patient);
        }

        /// <summary>
        /// Creates a new patient record.
        /// </summary>
        /// <param name="patient">The patient object to create.</param>
        /// <returns>
        /// A 201 Created response containing the newly created patient object.
        /// </returns>
        [HttpPost]
        public ActionResult<Patient> CreatePatient(Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Patients.Add(patient);
            context.SaveChanges();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.ID }, patient);
        }

        /// <summary>
        /// Updates an existing patient record.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to update.</param>
        /// <param name="patient">The updated patient object.</param>
        /// <returns>
        /// A 204 No Content response if the update is successful; otherwise, an appropriate error response.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult UpdatePatient(int id, Patient patient)
        {
            if (id != patient.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Patient? existing = context.Patients.Find(id);

            if (existing == null)
                return NotFound();

            existing.Name = patient.Name;
            existing.Surname = patient.Surname;
            existing.PESEL = patient.PESEL;
            existing.Birthday = patient.Birthday;
            existing.Email = patient.Email;
            existing.PhoneNumber = patient.PhoneNumber;
            context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletes a patient record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the patient to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult DeletePatient(int id)
        {
            Patient? patient = context.Patients.Find(id);

            if (patient == null)
                return NotFound();

            context.Patients.Remove(patient);
            context.SaveChanges();

            return NoContent();
        }
    }
}
