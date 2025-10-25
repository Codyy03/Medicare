using MediCare.Server.Entities;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using static MediCare.Server.Controllers.SpecializationsController;

namespace MediCare.Server.Tests.Controllers
{
    public class SpecializationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        /// <summary>
        /// Verifies that the /api/specializations endpoint returns a JSON list of specializations
        /// and that each specialization contains all required fields.
        /// </summary>
        [Fact]
        public async Task GetSpecializations_ReturnOk()
        {
            // arrage 
            var client = new SeededDbFactory().CreateClient();

            //act
            var response = await client.GetAsync("/api/specializations");

            //assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var specialization = JsonSerializer.Deserialize<List<Specialization>>(content,
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });

            Assert.NotNull(specialization);
            Assert.All(specialization, s =>
            {
                Assert.False(string.IsNullOrEmpty(s.SpecializationName));
                Assert.False(string.IsNullOrEmpty(s.SpecializationDescription));
                Assert.False(string.IsNullOrEmpty(s.Link));
                Assert.False(string.IsNullOrEmpty(s.SpecializationHighlight));
            });
        }

        /// <summary>
        /// Verifies that the endpoint <c>GET /api/specializations/specializationsNames</c>
        /// returns a JSON list of specialization IDs and names.
        /// Ensures the response is 200 OK, content type is JSON, 
        /// and that each specialization has a non-empty name and a valid positive ID.
        /// </summary>
        [Fact]
        public async Task GetSpecializationsNamesIDs_ReturnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var response = await client.GetAsync("/api/specializations/specializationsNames");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var specialization = JsonSerializer.Deserialize<List<SpecializationsNamesID>>(content,
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });

            Assert.NotNull(specialization);
            Assert.NotEmpty(specialization);
            Assert.All(specialization, s =>
            {
                Assert.False(string.IsNullOrEmpty(s.SpecializationName));
                Assert.True(s.ID > 0);
            });
        }

        /// <summary>
        /// Verifies that the /api/specializations/highlights endpoint returns a JSON list
        /// of highlighted specializations and that each highlight contains all required fields.
        /// </summary>
        [Fact]
        public async Task GetSpecialization_Empty()
        {
            // arrage
            var client = new EmptyDbFactory().CreateClient();

            //act
            var response = await client.GetAsync("api/specializations");

            //assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<Specialization>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        /// <summary>
        /// Verifies that requesting a specialization by a non-existent ID
        /// returns a 404 Not Found status and a ProblemDetails response.
        /// </summary>
        [Fact]
        public async Task GetSpecialization_ReturnsNotFound_WhenDoesNotExist()
        {
            var client = new EmptyDbFactory().CreateClient();

            var response = await client.GetAsync("/api/specializations/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var problem = JsonSerializer.Deserialize<ProblemDetails>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(problem);
            Assert.Equal(404, problem.Status);
        }

        /// <summary>
        /// Verifies that the /api/specializations/highlights endpoint returns a JSON list
        /// of highlighted specializations and that each highlight contains all required fields.
        /// </summary>
        [Fact]
        public async Task GetSpecializationsHightlights_ReturnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var response = await client.GetAsync("/api/specializations/highlights");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var conent = await response.Content.ReadAsStringAsync();
            var hightlights = JsonSerializer.Deserialize<List<SpecializationHighlightDto>>(conent,
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });

            Assert.NotNull(hightlights);
            Assert.All(hightlights, h =>
            {
                Assert.False(string.IsNullOrEmpty(h.SpecializationName));
                Assert.False(string.IsNullOrEmpty(h.Link));
                Assert.False(string.IsNullOrEmpty(h.SpecializationHighlight));
            });
        }

        /// <summary>
        /// Verifies that creating a new specialization via POST returns 201 Created.
        /// </summary>
        [Fact]
        public async Task CreateSpecialization_ReturnsCreated()
        {
            var client = new EmptyDbFactory().CreateClient();

            var newSpec = new Specialization
            {
                SpecializationName = "Neurologist",
                SpecializationDescription = "Specialist in nervous system disorders",
                SpecializationHighlight = "Expert care for brain and nerve health",
                Link = "#"
            };

            var response = await client.PostAsJsonAsync("/api/specializations", newSpec);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        /// <summary>
        /// Verifies that updating an existing specialization via PUT returns 204 NoContent
        /// when the specialization exists and the request is valid.
        /// </summary>
        [Fact]
        public async Task UpdateSpecialization_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<Specialization>>("/api/specializations");
            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var existing = list.First();
            existing.SpecializationName = "Updated Name";

            var response = await client.PutAsJsonAsync($"/api/specializations/{existing.ID}", existing);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Verifies that deleting an existing specialization via DELETE returns 204 NoContent
        /// when the specialization exists and is successfully removed.
        /// </summary>
        [Fact]
        public async Task DeleteSpecialization_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<Specialization>>("/api/specializations");
            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var existingId = list.First().ID;

            var response = await client.DeleteAsync($"/api/specializations/{existingId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}

