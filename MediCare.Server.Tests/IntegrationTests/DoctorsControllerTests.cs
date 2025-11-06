using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static MediCare.Server.Controllers.DoctorsController;

namespace MediCare.Server.Tests.Controllers;

public class DoctorsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// Verifies that <c>GET /api/doctors</c> returns a list of doctors with valid data.
    /// Ensures response is 200 OK and each doctor has required fields populated.
    /// </summary>
    [Fact]
    public async Task GetDoctors_ReturnOk()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("api/doctors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var content = await response.Content.ReadAsStringAsync();

        var doctors = JsonSerializer.Deserialize<List<DoctorDto>>(content,
           new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(doctors);
        Assert.All(doctors, d =>
        {
            Assert.False(string.IsNullOrEmpty(d.Name));
            Assert.False(string.IsNullOrEmpty(d.Surname));
            Assert.True(d.StartHour > TimeOnly.MinValue);
            Assert.True(d.EndHour > TimeOnly.MinValue);
            Assert.False(string.IsNullOrEmpty(d.Email));
            Assert.False(string.IsNullOrEmpty(d.PhoneNumber));
        });
    }

    /// <summary>
    /// Verifies that <c>GET /api/doctors</c> returns an empty list when no doctors exist.
    /// Ensures response is 200 OK and the list is empty.
    /// </summary>
    [Fact]
    public async Task GetDoctors_Empty()
    {
        var client = new EmptyDbFactory().CreateClient();

        var response = await client.GetAsync("/api/doctors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List<DoctorDto>>(content,
             new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(list);
        Assert.Empty(list);
    }

    /// <summary>
    /// Verifies that <c>POST /api/doctors/register</c> creates a new doctor.
    /// Ensures response is 201 Created and returned doctor matches input data.
    /// </summary>
    [Fact]
    public async Task CreateDoctor_ReturnsCreated()
    {
        var client = new EmptyDbFactory().CreateClient();

        var dto = new DoctorRegisterDto
        {
            Name = "John",
            Surname = "Smith",
            Email = "john@email.com",
            PhoneNumber = "123457777",
            Password = "Test1234!",
            SpecializationIds = new List<int> { 1 }
        };

        var response = await client.PostAsJsonAsync("/api/doctors/register", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<DoctorDto>();

        Assert.NotNull(created);
        Assert.Equal("John", created!.Name);
        Assert.Equal("Smith", created.Surname);
    }

    /// <summary>
    /// Verifies that <c>PUT /api/doctors/update</c> updates an existing doctor.
    /// Ensures response is 200 OK and returned doctor reflects updated values.
    /// </summary>
    [Fact]
    public async Task UpdateDoctor_ReturnsOkWithUpdatedDoctor()
    {
        var client = new SeededDbFactory().CreateClient();

        var list = await client.GetFromJsonAsync<List<DoctorDto>>("/api/doctors");

        Assert.NotNull(list);
        Assert.NotEmpty(list);

        var existing = list.First();
        var update = new DoctorUpdateDto
        {
            Name = "Newname",
            Surname = existing.Surname,
            PhoneNumber = existing.PhoneNumber,
            StartHour = existing.StartHour,
            EndHour = existing.EndHour,
            Facility = existing.Facility ?? "Room 203, MediCare Center",
            DoctorDescription = existing.DoctorDescription ?? "Experienced cardiologist with 15+ years of practice."
        };

        var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Doctor");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync($"/api/doctors/update", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedData = await response.Content.ReadFromJsonAsync<DoctorDto>();
        Assert.NotNull(updatedData);
        Assert.Equal("Newname", updatedData!.Name);
    }

    /// <summary>
    /// Verifies that <c>DELETE /api/doctors/{id}</c> removes a doctor.
    /// Ensures response is 204 NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteDoctor_ReturnsNoContent()
    {
        var client = new SeededDbFactory().CreateClient();

        var list = await client.GetFromJsonAsync<List<DoctorDto>>("/api/doctors");

        Assert.NotNull(list);
        Assert.NotEmpty(list);

        int existingId = list.First().ID;

        var response = await client.DeleteAsync($"/api/doctors/{existingId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    /// <summary>
    /// Verifies that <c>GET /api/doctors/me</c> without a token returns 401 Unauthorized.
    /// </summary>
    [Fact]
    public async Task GetMe_Unauthorized_WithoutToken()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Verifies that <c>GET /api/doctors/me</c> with a valid token returns the authenticated doctor.
    /// Ensures response is 200 OK and doctor details match the token identity.
    /// </summary>
    [Fact]
    public async Task GetMe_ReturnsDoctor_WhenAuthorized()
    {
        // Arrange
        var factory = new SeededDbFactory();
        var client = factory.CreateClient();

        var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Doctor");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/doctors/me");

        // Assert
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<DoctorDto>();

        Assert.NotNull(dto);
        Assert.Equal(1, dto!.ID);                
        Assert.Equal("John", dto.Name);
        Assert.Equal("Smith", dto.Surname);
        Assert.Equal("john.smith@medicare.com", dto.Email);
        Assert.False(string.IsNullOrEmpty(dto.PhoneNumber));
        Assert.NotEmpty(dto.Specializations);
    }

    /// <summary>
    /// Verifies that <c>PUT /api/doctors/password-reset</c> succeeds with correct old password.
    /// Ensures response is 204 NoContent and login works only with the new password.
    /// </summary>
    [Fact]
    public async Task ReestPassword_ReturnsNoContent_WhenValid()
    {
        var factory = new SeededDbFactory();
        var client = factory.CreateClient();

        var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Doctor");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        PasswordResetDto passwordResetDto = new PasswordResetDto
        {
            OldPassword = "1234",
            NewPassword = "NewPass123!"
        };

        var response = await client.PutAsJsonAsync("/api/doctors/password-reset", passwordResetDto);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var loginOld = await client.PostAsJsonAsync("/api/doctors/login", new LoginDto
        {
            Email = "john.smith@medicare.com",
            Password = "Test1234!"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, loginOld.StatusCode);

        var loginNew = await client.PostAsJsonAsync("/api/doctors/login", new LoginDto
        {
            Email = "john.smith@medicare.com",
            Password = "NewPass123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginNew.StatusCode);
    }

    /// <summary>
    /// Verifies that <c>PUT /api/doctors/password-reset</c> fails with incorrect old password.
    /// Ensures response is 400 BadRequest and error message indicates invalid old password.
    /// </summary>
    [Fact]
    public async Task ResetPassword_ReturnsBadRequest_WhenOldPasswordIncorrect()
    {
        var client = new SeededDbFactory().CreateClient();

        var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Doctor");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new PasswordResetDto
        {
            OldPassword = "WrongPass",
            NewPassword = "NewPass123"
        };

        var response = await client.PutAsJsonAsync("/api/doctors/password-reset", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Old password is incorrect", content);
    }

    /// <summary>
    /// Verifies that <c>GET /api/doctors/by-filter?surname=Smith</c> returns only doctors with matching surname.
    /// Ensures response is 200 OK and all returned doctors contain "Smith" in surname.
    /// </summary>
    [Fact]
    public async Task GetDoctors_ReturnSortedBySurnname_ReturnOk()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/by-filter?surname=Smith");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var content = await response.Content.ReadAsStringAsync();

        var doctors = JsonSerializer.Deserialize<List<DoctorDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(doctors);
        Assert.NotEmpty(doctors);

        Assert.All(doctors, d =>
        {
            Assert.False(string.IsNullOrEmpty(d.Name));
            Assert.False(string.IsNullOrEmpty(d.Surname));
            Assert.False(string.IsNullOrEmpty(d.Email));
            Assert.False(string.IsNullOrEmpty(d.PhoneNumber));
            Assert.True(d.ID > 0);
            Assert.True(d.StartHour > TimeOnly.MinValue);
            Assert.True(d.EndHour > TimeOnly.MinValue);

            Assert.Contains("smith", d.Surname.ToLower());
        });
    }

    /// <summary>
    /// Verifies that <c>GET /api/doctors/by-filter?availableAt=HH:mm</c> returns only doctors available at the given time.
    /// Ensures response is 200 OK and all returned doctors have working hours covering the requested time.
    /// </summary>
    [Fact]
    public async Task GetDoctors_FilterByAvailableAt_ReturnsOnlyMatching()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/by-filter?availableAt=9:00");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var content = await response.Content.ReadAsStringAsync();

        var doctors = JsonSerializer.Deserialize<List<DoctorDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(doctors);
        Assert.NotEmpty(doctors);

        Assert.All(doctors, d =>
        {
            Assert.False(string.IsNullOrEmpty(d.Name));
            Assert.False(string.IsNullOrEmpty(d.Surname));
            Assert.False(string.IsNullOrEmpty(d.Email));
            Assert.False(string.IsNullOrEmpty(d.PhoneNumber));
            Assert.True(d.ID > 0);
            Assert.True(d.StartHour <= new TimeOnly(9, 0) && d.EndHour >= new TimeOnly(9, 0));
        });
    }

    /// <summary>
    /// Verifies that <c>GET /api/doctors/by-filter?specializationID=1</c> returns only doctors with the given specialization.
    /// Ensures response is 200 OK and all returned doctors include the specialization.
    /// </summary>
    [Fact]
    public async Task GetDoctors_FilterBySpecialization_ReturnsOnlyMatching()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/by-filter?specializationID=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var content = await response.Content.ReadAsStringAsync();

        var doctors = JsonSerializer.Deserialize<List<DoctorDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(doctors);
        Assert.NotEmpty(doctors);

        Assert.All(doctors, d =>
        {
            Assert.False(string.IsNullOrEmpty(d.Name));
            Assert.False(string.IsNullOrEmpty(d.Surname));
            Assert.False(string.IsNullOrEmpty(d.Email));
            Assert.False(string.IsNullOrEmpty(d.PhoneNumber));
            Assert.True(d.ID > 0);
            Assert.True(d.StartHour > TimeOnly.MinValue);
            Assert.True(d.EndHour > TimeOnly.MinValue);

            Assert.Contains("Cardiologist", d.Specializations);
        });
    }

    /// <summary>
    /// Verifies that <c>GET /api/doctors/by-filter?specializationID=1&amp;surname=Smith</c> 
    /// returns only doctors matching both filters.
    /// Ensures response is 200 OK and all returned doctors have the specialization and surname.
    /// </summary>
    [Fact]
    public async Task GetDoctors_FilterBySurnameAndSpecialization_ReturnsOnlyMatching()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/by-filter?specializationID=1&&surname=Smith");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var content = await response.Content.ReadAsStringAsync();

        var doctors = JsonSerializer.Deserialize<List<DoctorDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(doctors);
        Assert.NotEmpty(doctors);

        Assert.All(doctors, d =>
        {
            Assert.False(string.IsNullOrEmpty(d.Name));
            Assert.False(string.IsNullOrEmpty(d.Surname));
            Assert.False(string.IsNullOrEmpty(d.Email));
            Assert.False(string.IsNullOrEmpty(d.PhoneNumber));
            Assert.True(d.ID > 0);
            Assert.True(d.StartHour > TimeOnly.MinValue);
            Assert.True(d.EndHour > TimeOnly.MinValue);

            Assert.Contains("Cardiologist", d.Specializations);
            Assert.Contains("smith", d.Surname.ToLower());
        });
    }
}
