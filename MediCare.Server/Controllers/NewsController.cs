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
        /// Retrieves a list of news items from the database, optionally filtered by month and year,
        /// and sorted by date in ascending or descending order.
        /// </summary>
        /// <param name="sort">
        /// Sort order for the results:
        /// "asc" - oldest first,
        /// "desc" - newest first (default).
        /// </param>
        /// <param name="month">
        /// Optional month filter (1-12). If provided together with <paramref name="year"/>,
        /// only news from that month will be returned.
        /// </param>
        /// <param name="year">
        /// Optional year filter. Must be provided together with <paramref name="month"/> to apply filtering.
        /// </param>
        /// <returns>
        /// HTTP 200 OK with a list of <see cref="NewsItem"/> objects matching the filter and sort criteria.
        /// </returns>
        [HttpGet("by-date")]
        public ActionResult<List<NewsItem>> GetNewsByDate(
            [FromQuery] string sort = "desc",
            [FromQuery] int? month = null, // Optional month parameter
            [FromQuery] int? year = null) // Optional year parameter
        {
            // base query for the News Items table
            IQueryable<NewsItem> query = context.NewsItems;

            // if the user provided a month and year, we filter only those records
            if (month.HasValue && year.HasValue)
                query = query.Where(x => x.Date.Month== month.Value && x.Date.Year == year.Value);

            //Sorting depending on the 'sort' parameter
            if (sort.ToLower() =="asc")
            {
                query = query.OrderBy(x => x.Date);
            }
            else
            {
                query = query.OrderByDescending(x => x.Date);
            }

            List<NewsItem> result = query.ToList();

            return Ok(result);
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
