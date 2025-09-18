using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Server.Controllers
{
    /// <summary>
    /// API controller for managing news items in the MediCare system.
    /// Provides endpoints to retrieve, create, update, and delete news records.
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
        /// Retrieves all news items.
        /// </summary>
        /// <returns>A list of <see cref="NewsItem"/> objects.</returns>
        [HttpGet]
        public ActionResult<List<NewsItem>> GetNewsItems()
        {
            List<NewsItem> newsItems = context.NewsItems.ToList();

            return Ok(newsItems);
        }

        /// <summary>
        /// Retrieves the latest news items, ordered by date in descending order.
        /// </summary>
        /// <param name="count">
        /// The maximum number of news items to return.
        /// </param>
        /// <returns>
        /// A list of the most recent <see cref="NewsItem"/> objects, up to the specified count.
        /// </returns>
        [HttpGet("latest/{count:int}")]
        public ActionResult<List<NewsItem>> GetLatestNews(int count)
        {
            List<NewsItem> latestNews = context.NewsItems
                .OrderByDescending(n => n.Date)
                .Take(count)
                .ToList();

            return Ok(latestNews);
        }

        /// <summary>
        /// Retrieves a specific news item by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the news item.</param>
        /// <returns>
        /// The <see cref="NewsItem"/> object if found; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{id:int}")]
        public ActionResult<NewsItem> GetNewsItem(int id) 
        {
            NewsItem? newsItem = context.NewsItems.Find(id);

            if (newsItem == null)
                return NotFound();

            return Ok(newsItem);
        }

        /// <summary>
        /// Creates a new news item.
        /// </summary>
        /// <param name="newsItem">The news item object to create.</param>
        /// <returns>
        /// A 201 Created response containing the newly created news item.
        /// </returns>
        [HttpPost]
        public ActionResult CreateNews(NewsItem newsItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.NewsItems.Add(newsItem);
            context.SaveChanges();

            return CreatedAtAction(nameof(GetNewsItem), new { id = newsItem.ID }, newsItem);
        }

        /// <summary>
        /// Updates an existing news item.
        /// </summary>
        /// <param name="id">The unique identifier of the news item to update.</param>
        /// <param name="newsItem">The updated news item object.</param>
        /// <returns>
        /// A 204 No Content response if the update is successful; otherwise, an appropriate error response.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult UpdateNews(int id, NewsItem newsItem)
        {
            if (id != newsItem.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            NewsItem? existing = context.NewsItems.Find(id);

            if (existing == null)
                return NotFound();

            existing.Title = newsItem.Title;
            existing.Description = newsItem.Description;
            existing.Date = newsItem.Date;
            existing.ImageURL = newsItem.ImageURL;

            context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletes a news item by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the news item to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteNews(int id)
        {
            NewsItem? newsItem = context.NewsItems.Find(id);

            if (newsItem == null)
                return NotFound();

            context.NewsItems.Remove(newsItem);
            context.SaveChanges();

            return NoContent();
        }
    }
}
