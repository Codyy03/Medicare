using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller for managing medical specializations in the MediCare system.
    /// Provides endpoints to retrieve, create, update, and delete specialization records.
    /// </summary>
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
        /// Retrieves all medical specializations available in the system.
        /// </summary>
        /// <returns>A list of all <see cref="Specialization"/> records.</returns>
        [HttpGet]
        public async Task<ActionResult<List<Specialization>>> GetSpecializations()
        {
            List<Specialization> specializations = await context.Specializations.ToListAsync();

            return Ok(specializations);
        }

        /// <summary>
        /// Retrieves a list of all available specializations with their IDs and names.
        /// </summary>
        /// <returns>
        /// A 200 OK response containing a list of <see cref="SpecializationsNamesID"/> objects.  
        /// Each object includes the specialization's unique identifier and display name.
        /// </returns>
        [HttpGet("specializationsNames")]
        public async Task<ActionResult<List<SpecializationsNamesID>>> GetSpecializationsNames()
        {
            List<SpecializationsNamesID> names = await context.Specializations.Select(
                 s => new SpecializationsNamesID
                 {
                     ID = s.ID,
                     SpecializationName = s.SpecializationName
                 }).ToListAsync();

            return Ok(names);
        }
        /// <summary>
        /// Retrieves a single specialization by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the specialization to retrieve.</param>
        /// <returns>
        /// Returns the <see cref="Specialization"/> if found; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialization>> GetSpecialization(int id)
        {
            Specialization? specialization = await context.Specializations.FindAsync(id);

            if (specialization == null)
                return NotFound();

            return Ok(specialization);
        }

        /// <summary>
        /// Retrieves a simplified list of specialization highlights,
        /// including the specialization name, highlight text, and an optional link.
        /// </summary>
        /// <returns>
        /// A list of <see cref="SpecializationHighlightDto"/> objects containing summary information.
        /// </returns>
        [HttpGet("highlights")]
        public async Task<ActionResult<List<SpecializationHighlightDto>>> GetSpecializationsHightlight()
        {
            List<SpecializationHighlightDto> specializationsHighlightDtos = await context.Specializations.Select
                (s => new SpecializationHighlightDto
                {
                    SpecializationName = s.SpecializationName,
                    SpecializationHighlight = s.SpecializationHighlight,
                    Link = s.Link
                }).ToListAsync();

            return Ok(specializationsHighlightDtos);
        }

        /// <summary>
        /// Creates a new specialization record in the system.
        /// </summary>
        /// <param name="specialization">The specialization entity to create.</param>
        /// <returns>
        /// A 201 Created response containing the newly created <see cref="Specialization"/> object.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<Specialization>> CreateSpecialization(Specialization specialization)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Specializations.Add(specialization);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSpecialization), new { id = specialization.ID }, specialization);
        }

        /// <summary>
        /// Updates the details of an existing specialization.
        /// </summary>
        /// <param name="id">The ID of the specialization to update.</param>
        /// <param name="specialization">The updated specialization entity.</param>
        /// <returns>
        /// A 204 No Content response if the update is successful; 
        /// 400 Bad Request if the IDs do not match; 
        /// or 404 Not Found if the specialization does not exist.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecialization(int id, Specialization specialization)
        {
            if (id != specialization.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Specialization? existing = await context.Specializations.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.SpecializationName = specialization.SpecializationName;
            existing.SpecializationHighlight = specialization.SpecializationHighlight;
            existing.SpecializationDescription = specialization.SpecializationDescription;
            existing.Link = specialization.Link;

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specialization by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the specialization to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; 
        /// or 404 Not Found if the specialization does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            Specialization? specialization = await context.Specializations.FindAsync(id);

            if (specialization == null)
                return NotFound();

            context.Specializations.Remove(specialization);
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Data Transfer Object (DTO) for returning specialization highlights.
        /// Contains only summary information such as name, highlight text, and link.
        /// </summary>
        public class SpecializationHighlightDto
        {
            public string? SpecializationName { get; set; }
            public string? Link { get; set; }
            public string? SpecializationHighlight { get; set; }
        }

        /// <summary>
        /// Represents a specialization with its ID and name.
        /// </summary>
        public class SpecializationsNamesID
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }

            [JsonPropertyName("specializationName")]
            public string? SpecializationName { get; set; }
        }
    }
}
