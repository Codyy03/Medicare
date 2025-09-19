using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller for managing medical specializations in the MediCare system.
    /// Provides endpoints to retrieve, create, update, and delete specialization records.
    [ApiController]
    [Route("api/[controller]")]
    public class SpecializationsController : ControllerBase
    {
        readonly MediCareDbContext context;

        public SpecializationsController(MediCareDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves all specializations.
        /// </summary>
        /// <returns>A list of <see cref="Specialization"/> objects.</returns>
        [HttpGet]
        public ActionResult<List<Specialization>> GetSpecializations()
        {
            List<Specialization> specializations = context.Specializations.ToList();

            return Ok(specializations);
        }

        /// <summary>
        /// Retrieves a specific specialization by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the specialization.</param>
        /// <returns>
        /// The <see cref="Specialization"/> object if found; otherwise, a 404 Not Found response.
        /// </returns>
        //Get api/Specializations/4
        [HttpGet("{id}")]
        public ActionResult<Specialization> GetSpecialization(int id)
        {
            Specialization? specialization = context.Specializations.Find(id);

            if (specialization == null)
                return NotFound();

            return Ok(specialization);
        }

        /// <summary>
        /// Retrieves a list of specialization highlights, including name, highlight text, and link.
        /// </summary>
        /// <returns>
        /// A list of <see cref="SpecializationHighlightDto"/> objects containing summary information.
        /// </returns>
        [HttpGet("highlights")]
        public ActionResult<List<SpecializationHighlightDto>> GetSpecializationsHightlight()
        {
            List<SpecializationHighlightDto> specializationsHighlightDtos = context.Specializations.Select
                (s => new SpecializationHighlightDto
                {
                    SpecializationName = s.SpecializationName,
                    SpecializationHighlight = s.SpecializationHighlight,
                    Link = s.Link
                }).ToList();

            return Ok(specializationsHighlightDtos);
        }

        /// <summary>
        /// Creates a new specialization record.
        /// </summary>
        /// <param name="specialization">The specialization object to create.</param>
        /// <returns>
        /// A 201 Created response containing the newly created specialization object.
        /// </returns>
        [HttpPost]
        public ActionResult<Specialization> CreateSpecialization(Specialization specialization)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Specializations.Add(specialization);
            context.SaveChanges();

            return CreatedAtAction(nameof(GetSpecialization), new { id = specialization.ID }, specialization);
        }

        /// <summary>
        /// Updates an existing specialization record.
        /// </summary>
        /// <param name="id">The unique identifier of the specialization to update.</param>
        /// <param name="specialization">The updated specialization object.</param>
        /// <returns>
        /// A 204 No Content response if the update is successful; otherwise, an appropriate error response.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult UpdateSpecialization(int id, Specialization specialization)
        {
            if (id != specialization.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Specialization? existing = context.Specializations.Find(id);

            if (existing == null)
                return NotFound();

            existing.SpecializationName = specialization.SpecializationName;
            existing.SpecializationHighlight = specialization.SpecializationHighlight;
            existing.SpecializationDescription = specialization.SpecializationDescription;
            existing.Link = specialization.Link;

            context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specialization record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the specialization to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteSpecialization(int id)
        {
            Specialization? specialization = context.Specializations.Find(id);

            if (specialization == null)
                return NotFound();

            context.Specializations.Remove(specialization);
            context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// DTO for returning specialization highlights.
        /// </summary>
        public class SpecializationHighlightDto
        {
            public string? SpecializationName { get; set; }
            public string? Link { get; set; }
            public string? SpecializationHighlight { get; set; }
        }
    }
}
