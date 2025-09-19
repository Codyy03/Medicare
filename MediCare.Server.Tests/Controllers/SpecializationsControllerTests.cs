using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using static MediCare.Server.Controllers.SpecializationsController;

namespace MediCare.Server.Tests.Controllers
{
    public class SpecializationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        readonly WebApplicationFactory<Program> factory;
        readonly WebApplicationFactory<Program> EmptyTestDb;

        public SpecializationsControllerTests(WebApplicationFactory<Program> factory)
        {
            this.factory = new SeededDbFactory();
            this.EmptyTestDb = new EmptyDbFactory();
        }

        /// <summary>
        /// Verifies that the /api/specializations endpoint returns a JSON list of specializations
        /// and that each specialization contains all required fields.
        /// </summary>
        [Fact]
        public async Task GetSpecializations_ReturnOk()
        {
            // arrage 
            var client = factory.CreateClient();

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
        /// Verifies that the /api/specializations/highlights endpoint returns a JSON list
        /// of highlighted specializations and that each highlight contains all required fields.
        /// </summary>
        [Fact]
        public async Task GetSpecialization_Empty()
        {
            // arrage
            var client = EmptyTestDb.CreateClient();

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
            var client = factory.CreateClient();

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
            var factory = new EmptyDbFactory();
            var client = factory.CreateClient();

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
            var factory = new SeededDbFactory();
            var client = factory.CreateClient();

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
            var factory = new SeededDbFactory();
            var client = factory.CreateClient();

            var list = await client.GetFromJsonAsync<List<Specialization>>("/api/specializations");
            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var existingId = list.First().ID;

            var response = await client.DeleteAsync($"/api/specializations/{existingId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    public class EmptyDbFactory : WebApplicationFactory<Program>
    {
        /// <summary>
        /// Configures a test web host with an empty in-memory database.
        /// A unique database name is used for each run to ensure data isolation.
        /// </summary>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MediCareDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<MediCareDbContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });
            });
        }
    }

    public class SeededDbFactory : WebApplicationFactory<Program>
    {
        /// A custom <see cref="WebApplicationFactory{TEntryPoint}"/> that configures
        /// an in-memory database named "TestDb" and pre-populates it with sample
        /// specialization records for integration testing.
        /// The database is cleared before seeding to ensure a consistent state
        /// across tests.
        /// </summary>
        protected override void ConfigureWebHost(IWebHostBuilder builder)

        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MediCareDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<MediCareDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MediCareDbContext>();
                db.Database.EnsureCreated();

                db.Specializations.RemoveRange(db.Specializations);
                db.SaveChanges();

                db.Specializations.AddRange(
                    new Specialization
                    {
                        ID = 1,
                        SpecializationName = "Cardiologist",
                        SpecializationDescription = "Specialist in heart diseases",
                        SpecializationHighlight = "Protect your heart with expert cardiovascular care and diagnostics.",
                        Link = "#"
                    },
                    new Specialization
                    {
                        ID = 2,
                        SpecializationName = "Orthopedic Surgeon",
                        SpecializationDescription = "Specialist in musculoskeletal system injuries and disorders",
                        SpecializationHighlight = "Restore mobility and strength with advanced orthopedic solutions",
                        Link = "#"
                    },
                    new Specialization
                    {
                        ID = 3,
                        SpecializationName = "Dermatologist",
                        SpecializationDescription = "Specialist in skin conditions",
                        SpecializationHighlight = "Healthy skin starts here comprehensive dermatological treatments for all ages.",
                        Link = "#"
                    }
                );
                db.SaveChanges();
            });
        }
    }
}

