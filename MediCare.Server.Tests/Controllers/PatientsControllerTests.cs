using MediCare.Server.Controllers;
using MediCare.Server.Entities;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static MediCare.Server.Controllers.PatientsController;

namespace MediCare.Server.Tests.Controllers
{
    public class PatientsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        /// <summary>
        /// Verifies that <c>GET /api/patients</c> returns a list of patients with valid data.
        /// Ensures response is 200 OK and each patient has required fields populated.
        /// </summary>
        [Fact]
        public async Task GetPatients_ReturnOk()
        {
            var clinet = new SeededDbFactory().CreateClient();

            var response = await clinet.GetAsync("api/patients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();

            var patients = JsonSerializer.Deserialize<List<PatientDto>>(content,
               new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(patients);
            Assert.All(patients, p =>
            {
                Assert.False(string.IsNullOrEmpty(p.Name));
                Assert.False(string.IsNullOrEmpty(p.Surname));
                Assert.True(p.Birthday > DateTime.MinValue);
                Assert.False(string.IsNullOrEmpty(p.Email));
                Assert.False(string.IsNullOrEmpty(p.PhoneNumber));
            });
        }

        /// <summary>
        /// Verifies that <c>GET /api/patients</c> returns an empty list when no patients exist.
        /// Ensures response is 200 OK and the list is empty.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>POST /api/patients/register</c> creates a new patient.
        /// Ensures response is 201 Created and returned patient matches input data.
        /// </summary>
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

            var created = await response.Content.ReadFromJsonAsync<PatientDto>();

            Assert.NotNull(created);
            Assert.Equal("John", created!.Name);
            Assert.Equal("Smith", created.Surname);
        }

        /// <summary>
        /// Verifies that <c>PUT /api/patients/update</c> updates an existing patient.
        /// Ensures response is 200 OK and returned patient reflects updated values.
        /// </summary>
        [Fact]
        public async Task UpdatePatient_ReturnsOkWithUpdatedPatient()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<PatientDto>>("/api/patients");

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var existing = list.First();

            var updateDto = new PatientUpdateDto
            {
                Name = "NewName",
                Surname = existing.Surname,
                PESEL = "12345678901",
                Birthday = existing.Birthday,
                PhoneNumber = existing.PhoneNumber
            };

            var token = TestJwtTokenHelper.GenerateTestToken("1", "michael.brown@example.com", "Michael", "Patient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"/api/patients/update", updateDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updated = await response.Content.ReadFromJsonAsync<PatientDto>();
            Assert.NotNull(updated);
            Assert.Equal("NewName", updated!.Name);
        }

        /// <summary>
        /// Verifies that <c>DELETE /api/patients/{id}</c> removes a patient.
        /// Ensures response is 204 NoContent and subsequent GET returns 404 NotFound.
        /// </summary>
        [Fact]
        public async Task DeletePatient_ReturnsNoContent()
        {
            // Arrange
            var factory = new SeededDbFactory();
            var client = factory.CreateClient();

            // get patients list
            var list = await client.GetFromJsonAsync<List<PatientDto>>("/api/patients");

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            int existingId = list.First().ID;

            // Act – delete patient
            var response = await client.DeleteAsync($"/api/patients/{existingId}");

            // Assert – DELETE reurn 204 NoContent
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // try to get delete patient
            var getResponse = await client.GetAsync($"/api/patients/{existingId}");

            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        /// <summary>
        /// Verifies that <c>GET /api/patients/me</c> without a token returns 401 Unauthorized.
        /// </summary>
        [Fact]
        public async Task GetMe_Unauthorized_WithoutToken()
        {
            var clinet = new SeededDbFactory().CreateClient();

            var response = await clinet.GetAsync("/api/patients/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        /// <summary>
        /// Verifies that <c>GET /api/patients/me</c> with a valid token returns the authenticated patient.
        /// Ensures response is 200 OK and patient details match the token identity.
        /// </summary>
        [Fact]
        public async Task GetMe_ReturnsDoctor_WhenAuthorized()
        {
            // Arrange
            var factory = new SeededDbFactory();
            var client = factory.CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "michael.brown@example.com", "Michael", "Patient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync("/api/patients/me");

            // Assert
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<PatientDto>();

            Assert.NotNull(dto);
            Assert.Equal(1, dto!.ID);
            Assert.Equal("Michael", dto.Name);
            Assert.Equal("Brown", dto.Surname);
            Assert.Equal("michael.brown@example.com", dto.Email);
            Assert.False(string.IsNullOrEmpty(dto.PhoneNumber));
        }

        /// <summary>
        /// Verifies that <c>PUT /api/patients/password-reset</c> succeeds with correct old password.
        /// Ensures response is 204 NoContent and login works only with the new password.
        /// </summary>
        [Fact]
        public async Task ReestPassword_ReturnsNoContent_WhenValid()
        {
            var factory = new SeededDbFactory();
            var client = factory.CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "michael.brown@example.com", "Michael", "Patient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PasswordResetDto passwordResetDto = new PasswordResetDto
            {
                OldPassword = "doctor1",
                NewPassword = "NewPass123!"
            };

            var response = await client.PutAsJsonAsync("/api/patients/password-reset", passwordResetDto);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var loginOld = await client.PostAsJsonAsync("/api/patients/login", new LoginDto
            {
                Email = "michael.brown@example.com",
                Password = "NewPass123!!"
            });
            Assert.Equal(HttpStatusCode.Unauthorized, loginOld.StatusCode);

            var loginNew = await client.PostAsJsonAsync("/api/patients/login", new LoginDto
            {
                Email = "michael.brown@example.com",
                Password = "NewPass123!"
            });
            Assert.Equal(HttpStatusCode.OK, loginNew.StatusCode);
        }

        /// <summary>
        /// Verifies that <c>PUT /api/patients/password-reset</c> fails with incorrect old password.
        /// Ensures response is 400 BadRequest and error message indicates invalid old password.
        /// </summary>
        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenOldPasswordIncorrect()
        {
            var client = new SeededDbFactory().CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "michael.brown@example.com", "Michael", "Patient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dto = new PasswordResetDto
            {
                OldPassword = "WrongPass",
                NewPassword = "NewPass123"
            };

            var response = await client.PutAsJsonAsync("/api/patients/password-reset", dto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Old password is incorrect", content);
        }

    }
}
