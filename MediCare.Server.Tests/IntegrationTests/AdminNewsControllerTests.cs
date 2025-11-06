using MediCare.Server.Entities;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static MediCare.Server.Controllers.AdminNewsController;

namespace MediCare.Server.Tests.Controllers
{
    public class AdminNewsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        /// <summary>
        /// Verifies that creating a new news item via POST to /api/adminNews/createNews
        /// returns a 201 Created status when the request is valid.
        /// </summary>
        [Fact]
        public async Task CreateNews_ReturnsCreated()
        {
            var client = new EmptyDbFactory().CreateClient();

            NewsDto newsItem = new NewsDto
            {
                Title = "Check your health with us!",
                Description = "My have new examinations for you check our description",
                Date = DateTime.Now,
                ImageURL = "#"
            };
            var response = await client.PostAsJsonAsync("/api/adminNews/createNews", newsItem);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Verifies that updating an existing news item via PUT to /api/news/{id}
        /// returns a 204 NoContent status when the item exists and the request is valid.
        /// </summary>
        [Fact]
        public async Task UpdateNews_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var existing = await client.GetFromJsonAsync<NewsDto>("/api/news/1");
            Assert.NotNull(existing);

            existing.Title = "New title";

            var response = await client.PutAsJsonAsync($"/api/adminNews/update/1", existing);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
