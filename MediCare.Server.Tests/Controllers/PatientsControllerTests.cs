using MediCare.Server.Controllers;
using MediCare.Server.Entities;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediCare.Server.Tests.Controllers
{
    public class PatientsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task GetPatients_ReturnOk()
        {
            var clinet = new SeededDbFactory().CreateClient();

            var response = await clinet.GetAsync("api/patients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();

            var patients = JsonSerializer.Deserialize<List<Patient>>(content,
               new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(patients);
            Assert.All(patients, p =>
            {
                Assert.False(string.IsNullOrEmpty(p.Name));
                Assert.False(string.IsNullOrEmpty(p.Surname));
                Assert.True(p.Birthday > DateTime.MinValue);
                Assert.False(string.IsNullOrEmpty(p.PESEL));
                Assert.False(string.IsNullOrEmpty(p.Email));
                Assert.False(string.IsNullOrEmpty(p.PhoneNumber));
            });
        }

        [Fact]
        public async Task GetPatients_Empty()
        {
            var client = new EmptyDbFactory().CreateClient();

            var response = await client.GetAsync("/api/patients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<Patient>>(content,
                 new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public async Task CreatePatient_ReturnsCreated()
        {

            var client = new EmptyDbFactory().CreateClient();

            var dto = new PatientRegisterDto
            {
                PESEL = "12345678900",
                Name = "John",
                Surname = "Smith",
                Birthday = DateTime.Now,
                Email = "john@email.com",
                PhoneNumber = "123457777",
                Password = "Test1234!"
            };

            var response = await client.PostAsJsonAsync("/api/patients/register", dto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<PatientRegisterDto>();

            Assert.NotNull(created);
            Assert.Equal("John", created!.Name);
            Assert.Equal("Smith", created.Surname);
        }

        [Fact]
        public async Task UpdatePatient_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<Patient>>("/api/patients");

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var existing = list.First();
            existing.Name = "New name";

            var response = await client.PutAsJsonAsync($"/api/patients/{existing.ID}", existing);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeletePatient_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<Patient>>("/api/patients");

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            int existingId = list.First().ID;

            var response = await client.DeleteAsync($"/api/patients/{existingId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}
