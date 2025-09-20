using MediCare.Server.Entities;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
namespace MediCare.Server.Tests.Controllers
{
    public class NewsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        /// <summary>
        /// Verifies that the /api/news endpoint returns a JSON list of news items
        /// and that each item contains all required fields with valid values.
        /// </summary>
        [Fact]
        public async Task GetNews_ReturnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var response = await client.GetAsync("/api/news");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();

            var news = JsonSerializer.Deserialize<List<NewsItem>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(news);
            Assert.All(news, n =>
            {
                Assert.False(string.IsNullOrEmpty(n.Title));
                Assert.False(string.IsNullOrEmpty(n.Description));
                Assert.True(n.Date > DateTime.MinValue);
                Assert.False(string.IsNullOrEmpty(n.ImageURL));
            });
        }

        /// <summary>
        /// Verifies that the /api/news endpoint returns an empty list
        /// when the database contains no news items.
        /// </summary>
        [Fact]
        public async Task GetNews_Empty()
        {
            var client = new EmptyDbFactory().CreateClient();

            var response = await client.GetAsync("/api/news");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<NewsItem>>(content,
                 new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        /// <summary>
        /// Verifies that the /api/news/latest/{count} endpoint returns the specified number
        /// of most recent news items, sorted in descending order by date, and that each item
        /// contains all required fields.
        /// </summary>
        [Fact]
        public async Task GeLastedtNews_ReturnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var response = await client.GetAsync("/api/news/latest/3");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();

            var news = JsonSerializer.Deserialize<List<NewsItem>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(news);
            Assert.Equal(3, news.Count);
            Assert.True(news.SequenceEqual(news.OrderByDescending(n => n.Date)));

            Assert.All(news, n =>
            {
                Assert.False(string.IsNullOrEmpty(n.Title));
                Assert.False(string.IsNullOrEmpty(n.Description));
                Assert.True(n.Date > DateTime.MinValue);
                Assert.False(string.IsNullOrEmpty(n.ImageURL));
            });
        }

        /// <summary>
        /// Verifies that creating a new news item via POST to /api/news
        /// returns a 201 Created status when the request is valid.
        /// </summary>
        [Fact]
        public async void CreateNews_ReturnsCreated()
        {
            var client = new EmptyDbFactory().CreateClient();

            NewsItem newsItem = new NewsItem
            {
                Title = "Check your health with us!",
                Description = "My have new examinations for you check our description",
                Date = DateTime.Now,
                ImageURL = "#"
            };
            var response = await client.PostAsJsonAsync("/api/news", newsItem);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        /// <summary>
        /// Verifies that updating an existing news item via PUT to /api/news/{id}
        /// returns a 204 NoContent status when the item exists and the request is valid.
        /// </summary>
        [Fact]
        public async void UpdateNews_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<NewsItem>>("/api/news");

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var existing = list.First();
            existing.Title = "New title";

            var response = await client.PutAsJsonAsync($"/api/news/{existing.ID}", existing);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Verifies that deleting an existing news item via DELETE to /api/news/{id}
        /// returns a 204 NoContent status when the item exists and is successfully removed.
        /// </summary>
        [Fact]
        public async Task DeleteSpecialization_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<NewsItem>>("/api/news");

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            int existingId = list.First().ID;

            var response = await client.DeleteAsync($"/api/news/{existingId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
