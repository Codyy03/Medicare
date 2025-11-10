using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller for managing news items in the MediCare system.
    /// Provides endpoints to retrieve, filter, create, update, and delete news records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        readonly MediCareDbContext context;

        public NewsController(MediCareDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves all news items from the system.
        /// </summary>
        /// <returns>A list of all <see cref="NewsItem"/> objects.</returns>
        [HttpGet]
        public async Task<ActionResult<List<NewsItem>>> GetNewsItems()
        {
            List<NewsItem> newsItems = await context.NewsItems.ToListAsync();

            return Ok(newsItems);
        }

        /// <summary>
        /// Retrieves news items filtered by month and year (if provided),
        /// and sorted by date in ascending or descending order.
        /// </summary>
        /// <param name="sort">
        /// Sort order for the results:
        /// "asc" - oldest first,
        /// "desc" - newest first (default).
        /// </param>
        /// <param name="month">
        /// Optional month filter (1–12). Must be provided together with <paramref name="year"/>.
        /// </param>
        /// <param name="year">
        /// Optional year filter. Must be provided together with <paramref name="month"/>.
        /// </param>
        /// <returns>
        /// A 200 OK response containing a list of <see cref="NewsItem"/> objects
        /// matching the filter and sort criteria.
        /// </returns>
        [HttpGet("by-date")]
        public async Task<ActionResult<List<NewsItem>>> GetNewsByDate(
            [FromQuery] string sort = "desc",
            [FromQuery] int? month = null, // Optional month parameter
            [FromQuery] int? year = null) // Optional year parameter
        {
            // base query for the News Items table
            IQueryable<NewsItem> query = context.NewsItems;

            // if the user provided a month and year, we filter only those records
            if (month.HasValue && year.HasValue)
                query = query.Where(x => x.Date.Month == month.Value && x.Date.Year == year.Value);

            //Sorting depending on the 'sort' parameter
            if (Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(x => x.Date);
            }
            else
            {
                query = query.OrderByDescending(x => x.Date);
            }

            List<NewsItem> result = await query.ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the latest news items, ordered by date in descending order.
        /// </summary>
        /// <param name="count">The maximum number of news items to return.</param>
        /// <returns>
        /// A 200 OK response containing up to <paramref name="count"/> of the most recent <see cref="NewsItem"/> objects.
        /// </returns>
        [HttpGet("latest/{count:int}")]
        public async Task<ActionResult<List<NewsItem>>> GetLatestNews(int count)
        {
            List<NewsItem> latestNews = await context.NewsItems
                .OrderByDescending(n => n.Date)
                .Take(count)
                .ToListAsync();

            return Ok(latestNews);
        }

        /// <summary>
        /// Retrieves a specific news item by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the news item to retrieve.</param>
        /// <returns>
        /// A 200 OK response with the <see cref="NewsItem"/> if found; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<NewsItem>> GetNewsItem(int id)
        {
            NewsItem? newsItem = await context.NewsItems.FindAsync(id);

            if (newsItem == null)
                return NotFound();

            return Ok(newsItem);
        }

        /// <summary>
        /// Creates a new news item in the system.
        /// </summary>
        /// <param name="newsItem">The news item entity to create.</param>
        /// <returns>
        /// A 201 Created response containing the newly created <see cref="NewsItem"/>; 
        /// or 400 Bad Request if the model state is invalid.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> CreateNews(NewsItem newsItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.NewsItems.Add(newsItem);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNewsItem), new { id = newsItem.ID }, newsItem);
        }

        /// <summary>
        /// Updates the details of an existing news item.
        /// </summary>
        /// <param name="id">The ID of the news item to update.</param>
        /// <param name="newsItem">The updated news item entity.</param>
        /// <returns>
        /// A 204 No Content response if the update is successful; 
        /// 400 Bad Request if the IDs do not match or the model state is invalid; 
        /// or 404 Not Found if the news item does not exist.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, NewsItem newsItem)
        {
            if (id != newsItem.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            NewsItem? existing = await context.NewsItems.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.Title = newsItem.Title;
            existing.Description = newsItem.Description;
            existing.Date = newsItem.Date;
            existing.ImageURL = newsItem.ImageURL;

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a news item by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the news item to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; 
        /// or 404 Not Found if the news item does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            NewsItem? newsItem = await context.NewsItems.FindAsync(id);

            if (newsItem == null)
                return NotFound();

            context.NewsItems.Remove(newsItem);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
