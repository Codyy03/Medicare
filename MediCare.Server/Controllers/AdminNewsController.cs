using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// Admin-only API controller for managing news items in the MediCare system.
    /// Provides endpoints to create and update news records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminNewsController : ControllerBase
    {
        private readonly MediCareDbContext context;

        public AdminNewsController(MediCareDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Creates a new news item in the system.
        /// Accepts a <see cref="NewsDto"/> object in the request body and saves it to the database.
        /// Returns 204 No Content if successful.
        /// </summary>
        /// <param name="dto">The data transfer object containing news details.</param>
        [HttpPost("createNews")]
        public async Task<IActionResult> CreateNews([FromBody] NewsDto dto)
        {
            var news = new NewsItem
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageURL = dto.ImageURL,
                Date = dto.Date,
            };

            await context.NewsItems.AddAsync(news);

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Updates an existing news item by its ID.
        /// Accepts a <see cref="NewsDto"/> object in the request body and applies changes to the database.
        /// Returns 200 OK with the updated news data if successful, or 404 Not Found if the item does not exist.
        /// </summary>
        /// <param name="id">The unique identifier of the news item to update.</param>
        /// <param name="dto">The data transfer object containing updated news details.</param>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] NewsDto dto)
        {
            var news = await context.NewsItems.FindAsync(id);

            if (news == null)
                return NotFound();

            news.Title = dto.Title;
            news.Description = dto.Description;
            news.ImageURL = dto.ImageURL;
            news.Date = dto.Date;

            await context.SaveChangesAsync();

            return Ok(new NewsDto
            {
                Title = news.Title,
                Description = news.Description,
                ImageURL = news.ImageURL,
                Date = news.Date
            });

        }

        /// <summary>
        /// Data Transfer Object (DTO) representing a news item.
        /// Used for creating and updating news records without exposing entity internals.
        /// </summary>
        public class NewsDto
        {
            public int Id { get; set; }
            public required string Title { get; set; }

            public required string Description { get; set; }

            public string? ImageURL { get; set; }

            public DateTime Date { get; set; }
        }
    }
}
