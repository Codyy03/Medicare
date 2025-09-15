using Microsoft.AspNetCore.Mvc;
using MediCare.Server.Data;
using MediCare.Server.Entities;

namespace MediCare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        readonly MediCareDbContext context;

        public PatientsController(MediCareDbContext context)
        {
            this.context = context;
        }

        // GET: api/patients
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetPatients()
        {
            List<Patient> patients = context.Patients.ToList();
            return Ok(patients);
        }

        // GET: api/patients/5
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatient(int id)
        {
            Patient? patient = context.Patients.Find(id);

            if (patient == null) 
                return NotFound();

            return Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        public ActionResult<Patient> CreatePatient(Patient patient)
        {
            context.Patients.Add(patient);
            context.SaveChanges();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.ID }, patient);
        }

        // PUT: api/patients/5
        [HttpPut("id")]
        public IActionResult UpdatePatient(int id, Patient patient)
        {
            if (id != patient.ID)
                return BadRequest();

            context.Entry(patient).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/patients/5
        [HttpDelete("id")]
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
