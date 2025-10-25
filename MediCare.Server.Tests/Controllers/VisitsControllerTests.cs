using MediCare.Server.Entities;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using static MediCare.Server.Controllers.VisitsController;

namespace MediCare.Server.Tests.Controllers
{
    public class VisitsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task GetVisitsTimeReturn_Ok()
        {
            var client = new SeededDbFactory().CreateClient();

            var date = new DateOnly(2025, 10, 22);
            var response = await client.GetAsync("api/visits/visitsTime?id=1&date=2025-10-22");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();

            var visits = JsonSerializer.Deserialize<List<VisitTimeDto>>(content,
               new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(visits);
            Assert.Single(visits);
            Assert.Equal(new TimeOnly(9, 30), visits[0].VisitTime);
            Assert.Equal("Cardiology Consultation Room", visits[0].Room);
        }

        [Fact]
        public async Task CreateVisit_ReturnsCreatedVisit_WhenValid()
        {
            // Arrange
            var client = new SeededDbFactory().CreateClient();

            var dto = new
            {
                VisitDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                VisitTime = new TimeOnly(10, 0),
                DoctorID = 1,
                PatientID = 1,
                SpecializationID = 1, 
                RoomID = 1,          
                Reason = 1,           
                AdditionalNotes = "Test visit"
            };

            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/visits", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var body = await response.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<VisitResponseDto>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(created);
            Assert.Equal(dto.DoctorID, created!.DoctorName.Contains("John") ? 1 : 0);
            Assert.Equal("Consultation", created.Reason);
            Assert.Equal("Cardiology Consultation Room", created.Room);
        }

        [Fact]
        public async Task CreateVisit_ReturnsBadRequest_WhenDateIsToday()
        {
            var client = new SeededDbFactory().CreateClient();

            var dto = new
            {
                VisitDate = DateOnly.FromDateTime(DateTime.Today),
                VisitTime = new TimeOnly(10, 0),
                DoctorID = 1,
                PatientID = 1,
                SpecializationID = 1,
                RoomID = 1,
                Reason = 1
            };

            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/visits", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
