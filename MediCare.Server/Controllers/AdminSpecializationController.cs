using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace MediCare.Server.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SpecializationsController : ControllerBase
    {
        private readonly MediCareDbContext _context;
        public SpecializationsController(MediCareDbContext context) => _context = context;

        /// <summary>
        /// Retrieves all specializations from the system.
        /// </summary>
        /// <returns>A list of <see cref="SpecializationDto"/> objects.</returns>
        [HttpGet]
        public async Task<ActionResult<List<SpecializationDto>>> GetAll()
        {
            var list = await _context.Specializations
                .Select(s => new SpecializationDto
                {
                    ID = s.ID,
                    SpecializationName = s.SpecializationName,
                    SpecializationHighlight = s.SpecializationHighlight,
                    SpecializationDescription = s.SpecializationDescription
                }).ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Retrieves a specialization by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the specialization to retrieve.</param>
        /// <returns>A <see cref="SpecializationDto"/> if found, or 404 Not Found if not.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecializationDto>> GetById(int id)
        {
            var spec = await _context.Specializations
                .Where(s => s.ID == id)
                .Select(s => new SpecializationDto
                {
                    ID = s.ID,
                    SpecializationName = s.SpecializationName,
                    SpecializationHighlight = s.SpecializationHighlight,
                    SpecializationDescription = s.SpecializationDescription
                })
                .FirstOrDefaultAsync();

            if (spec == null) return NotFound();
            return Ok(spec);
        }

        /// <summary>
        /// Creates a new specialization record.
        /// </summary>
        /// <param name="dto">The DTO containing specialization details.</param>
        /// <returns>The created specialization as <see cref="SpecializationDto"/>.</returns>
        [HttpPost]
        public async Task<ActionResult<SpecializationDto>> Create([FromBody] SpecializationCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var normalized = dto.SpecializationName.Trim().ToLowerInvariant();
            var exists = await _context.Specializations
                .AnyAsync(s => s.SpecializationName.ToLower() == normalized);

            if (exists)
            {
                return BadRequest(new { error = "Specialization with the same name already exists" });
            }

            var entity = new Specialization
            {
                SpecializationName = dto.SpecializationName.Trim(),
                SpecializationHighlight = dto.SpecializationHighlight ?? string.Empty,
                SpecializationDescription = dto.SpecializationDescription ?? string.Empty
            };

            _context.Specializations.Add(entity);
            await _context.SaveChangesAsync();

            var result = new SpecializationDto
            {
                ID = entity.ID,
                SpecializationName = entity.SpecializationName,
                SpecializationHighlight = entity.SpecializationHighlight,
                SpecializationDescription = entity.SpecializationDescription
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.ID }, result);
        }

        /// <summary>
        /// Updates an existing specialization record.
        /// </summary>
        /// <param name="id">The ID of the specialization to update.</param>
        /// <param name="dto">The DTO containing updated specialization details.</param>
        /// <returns>204 No Content if successful, or 404 Not Found if not.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SpecializationUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.ID) return BadRequest("ID mismatch");

            var existing = await _context.Specializations.FindAsync(id);
            if (existing == null) return NotFound();

            existing.SpecializationName = dto.SpecializationName;
            existing.SpecializationHighlight = dto.SpecializationHighlight ?? string.Empty;
            existing.SpecializationDescription = dto.SpecializationDescription ?? string.Empty;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a specialization by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the specialization to delete.</param>
        /// <returns>204 No Content if successful, or 404 Not Found if not.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.Specializations.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Specializations.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    /// <summary>
    /// Data Transfer Object (DTO) representing a specialization.
    /// Used for returning specialization details to clients.
    /// </summary>
    public class SpecializationDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("specializationName")]
        public string? SpecializationName { get; set; }

        [JsonPropertyName("specializationHighlight")]
        public string? SpecializationHighlight { get; set; }

        [JsonPropertyName("specializationDescription")]
        public string? SpecializationDescription { get; set; }
    }

    /// <summary>
    /// DTO used for creating a new specialization.
    /// </summary>
    public class SpecializationCreateDto
    {
        [JsonPropertyName("specializationName")]
        public string SpecializationName { get; set; } = null!;

        [JsonPropertyName("specializationHighlight")]
        public string? SpecializationHighlight { get; set; }

        [JsonPropertyName("specializationDescription")]
        public string? SpecializationDescription { get; set; }
    }

    /// <summary>
    /// DTO used for updating an existing specialization.
    /// </summary>
    public class SpecializationUpdateDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("specializationName")]
        public string SpecializationName { get; set; } = null!;

        [JsonPropertyName("specializationHighlight")]
        public string? SpecializationHighlight { get; set; }

        [JsonPropertyName("specializationDescription")]
        public string? SpecializationDescription { get; set; }
    }
}
