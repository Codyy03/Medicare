using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminNewsController : ControllerBase
    {
        private readonly MediCareDbContext context;

        public AdminNewsController(MediCareDbContext context)
        {
            this.context = context;
        }

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

        public class NewsDto 
        {
            public int Id { get; set; }
            public required string Title { get; set; }

            public required string Description { get; set; }

            public string ImageURL { get; set; }

            public DateTime Date { get; set; }
        }
    }
}
