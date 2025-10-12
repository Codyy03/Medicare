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

    [Fact]
    public async Task UpdateDoctor_ReturnsOkWithUpdatedDoctor()
    {
        var client = new SeededDbFactory().CreateClient();

        var list = await client.GetFromJsonAsync<List<DoctorDto>>("/api/doctors");

        Assert.NotNull(list);
        Assert.NotEmpty(list);

        var existing = list.First();
        existing.Name = "New name";

        var response = await client.PutAsJsonAsync($"/api/doctors/{existing.ID}", existing);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<DoctorDto>();
        Assert.NotNull(updated);
        Assert.Equal("New name", updated!.Name);
    }

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

    [Fact]
    public async Task GetMe_Unauthorized_WithoutToken()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

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

    [Fact]
    public async Task GetDoctors_FilterByAvailableFrom_ReturnsOnlyMatching()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/by-filter?availableFrom=9:00");

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

            Assert.True(d.StartHour <= new TimeOnly(9,0));
        });
    }

    [Fact]
    public async Task GetDoctors_FilterByAvailableUntil_ReturnsOnlyMatching()
    {
        var clinet = new SeededDbFactory().CreateClient();

        var response = await clinet.GetAsync("/api/doctors/by-filter?availableUntil=17:00");

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

            Assert.True(d.EndHour >= new TimeOnly(17, 0));
        });
    }

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
