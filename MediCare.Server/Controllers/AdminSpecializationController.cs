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
                SpecializationHighlight = dto.SpecializationHighlight,
                SpecializationDescription = dto.SpecializationDescription
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


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SpecializationUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.ID) return BadRequest("ID mismatch");

            var existing = await _context.Specializations.FindAsync(id);
            if (existing == null) return NotFound();

            existing.SpecializationName = dto.SpecializationName;
            existing.SpecializationHighlight = dto.SpecializationHighlight;
            existing.SpecializationDescription = dto.SpecializationDescription;

            await _context.SaveChangesAsync();
            return NoContent();
        }

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

    public class SpecializationCreateDto
    {
        [JsonPropertyName("specializationName")]
        public string SpecializationName { get; set; } = null!;

        [JsonPropertyName("specializationHighlight")]
        public string? SpecializationHighlight { get; set; }

        [JsonPropertyName("specializationDescription")]
        public string? SpecializationDescription { get; set; }
    }

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
