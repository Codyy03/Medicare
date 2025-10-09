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

}
