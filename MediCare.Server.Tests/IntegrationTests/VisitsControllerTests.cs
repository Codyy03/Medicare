using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static MediCare.Server.Controllers.VisitsController;
using static MediCare.Server.Entities.Enums;

namespace MediCare.Server.Tests.Controllers
{
    public class VisitsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        /// <summary>
        /// Verifies that retrieving all visits returns HTTP 200 OK and valid data.
        /// </summary>
        /// <remarks>
        /// The test calls the /api/visits endpoint, ensures the response is successful,
        /// and deserializes the JSON into a list of VisitResponseDto objects.
        /// It asserts that the list is not null and that each visit has valid values for
        /// ID, date, time, doctor, patient, room, status, and reason.
        /// </remarks>
        [Fact]
        public async Task GetVisitsReturn_Ok()
        {
            var client = new SeededDbFactory().CreateClient();

            var date = new DateOnly(2025, 10, 22);
            var response = await client.GetAsync("api/visits");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();

            var visits = JsonSerializer.Deserialize<List<VisitResponseDto>>(content,
               new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(visits);
            Assert.All(visits, v =>
            {
                Assert.True(v.ID > 0);
                Assert.InRange(v.VisitDate, DateOnly.MinValue, DateOnly.MaxValue);
                Assert.InRange(v.VisitTime, TimeOnly.MinValue, TimeOnly.MaxValue);
                Assert.False(string.IsNullOrEmpty(v.DoctorName));
                Assert.False(string.IsNullOrEmpty(v.Specialization));
                Assert.False(string.IsNullOrEmpty(v.PatientName));
                Assert.False(string.IsNullOrEmpty(v.Room));
                Assert.False(string.IsNullOrEmpty(v.Reason));
                Assert.False(string.IsNullOrEmpty(v.Status));
            });
        }

        /// <summary>
        /// Verifies that the endpoint <c>GET api/visits/visitsTime</c> 
        /// returns a JSON list of visit times for a given doctor and date.
        /// Ensures the response is 200 OK, content type is JSON, 
        /// and that the returned visit matches the seeded data.
        /// </summary>
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

        /// <summary>
        /// Verifies that the endpoint <c>GET api/visits/visitsToday</c>
        /// returns today's visits for the authenticated doctor.
        /// Ensures the response is 200 OK, content type is JSON,
        /// and validates that each visit (if any) contains the expected fields.
        /// If no visits exist for today, asserts that the list is empty.
        /// </summary>
        [Fact]
        public async Task GetVisitsToday_ReurnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Doctor");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/visits/visitsToday");

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TodayVisitsResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result.Visits.Any())
            {
                Assert.All(result.Visits, v =>
                {
                    Assert.False(string.IsNullOrEmpty(v.PatientName));
                    Assert.False(string.IsNullOrEmpty(v.Specialization));
                    Assert.False(string.IsNullOrEmpty(v.Reason));
                    Assert.False(string.IsNullOrEmpty(v.Room));
                    Assert.True(v.ID > 0);
                    Assert.InRange(v.VisitTime, TimeOnly.MinValue, TimeOnly.MaxValue);
                });
            }
            else
            {
                Assert.Empty(result.Visits);
            }
        }

        /// <summary>
        /// Verifies that the endpoint <c>GET api/visits/patient</c>
        /// returns the list of visits for the authenticated patient.
        /// Ensures the response is 200 OK, content type is JSON,
        /// and validates that each visit (if any) contains the expected fields.
        /// If the patient has no visits, asserts that the list is empty.
        /// </summary>
        [Fact]
        public async Task GetPatientVisit_ReurnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "michael.brown@example.com", "Michael", "Patient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/visits/patient");

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<VisitResponseDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result.Any())
            {
                Assert.All(result, v =>
                {
                    Assert.False(string.IsNullOrEmpty(v.Specialization));
                    Assert.False(string.IsNullOrEmpty(v.Reason));
                    Assert.False(string.IsNullOrEmpty(v.Status));
                    Assert.True(v.VisitDate > DateOnly.MinValue);
                    Assert.False(string.IsNullOrEmpty(v.Room));
                    Assert.True(v.ID > 0);
                    Assert.InRange(v.VisitTime, TimeOnly.MinValue, TimeOnly.MaxValue);
                });
            }
            else
            {
                Assert.Empty(result);
            }
        }

        /// <summary>
        /// Verifies that retrieving visits for a doctor returns HTTP 200 OK and valid data.
        /// </summary>
        /// <remarks>
        /// The test generates a JWT token for a doctor user, sets it in the request headers,
        /// and calls the /api/visits/doctor endpoint. It asserts that the response is OK and
        /// that the returned list of visits contains valid values for all required fields.
        /// If no visits are returned, it asserts that the result is an empty list.
        /// </remarks>
        [Fact]
        public async Task GetDoctorVisit_ReurnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Doctor");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/visits/doctor");

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<DoctorVisitsDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result.Any())
            {
                Assert.All(result, v =>
                {
                    Assert.True(v.ID > 0);
                    Assert.InRange(v.VisitDate, DateOnly.MinValue, DateOnly.MaxValue);
                    Assert.InRange(v.VisitTime, TimeOnly.MinValue, TimeOnly.MaxValue);
                    Assert.False(string.IsNullOrEmpty(v.DoctorName));
                    Assert.False(string.IsNullOrEmpty(v.Specialization));
                    Assert.False(string.IsNullOrEmpty(v.PatientName));
                    Assert.False(string.IsNullOrEmpty(v.Room));
                    Assert.True(v.RoomNumber > 0);
                    Assert.False(string.IsNullOrEmpty(v.Status));
                    Assert.False(string.IsNullOrEmpty(v.Reason));
                });
            }
            else
            {
                Assert.Empty(result);
            }
        }

        /// <summary>
        /// Integration test verifying that cancelling a scheduled visit
        /// returns 200 OK and updates the visit status to "Cancelled".
        /// </summary>
        [Fact]
        public async Task CancelVisitReturnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "michael.brown@example.com", "Michael", "Patient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync("api/visits/canceledVisit/1", null);

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var visitResponse = await client.GetAsync("api/visits/1");
            var content = await visitResponse.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<VisitResponseDto>(content,
             new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(VisitStatus.Cancelled.ToString(), result.Status);
        }

        /// <summary>
        /// Verifies that updating an existing visit returns HTTP 204 NoContent.
        /// </summary>
        /// <remarks>
        /// The test retrieves an existing visit, modifies several fields (PrescriptionText, VisitNotes,
        /// AdditionalNotes, Reason, Status), and sends a PUT request to the update endpoint.
        /// It asserts that the response status code is NoContent, indicating a successful update.
        /// </remarks>
        [Fact]
        public async Task UpdateVisit_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();

            var existing = await client.GetFromJsonAsync<VisitResponseDto>("/api/visits/1");
            Assert.NotNull(existing);

            existing.PrescriptionText = "New PrescriptionText";
            existing.VisitNotes = "vist node";
            existing.AdditionalNotes = "Additional Notes";
            existing.Reason = "Checkup";
            existing.Status = "Completed";

            var response = await client.PutAsJsonAsync($"/api/visits/update/1", existing);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Verifies that the endpoint <c>POST api/visits</c> 
        /// successfully creates a new visit when valid data is provided.
        /// Ensures the response is 201 Created, content type is JSON, 
        /// and that the returned visit details match the request payload.
        /// </summary>
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

        /// <summary>
        /// Verifies that the endpoint <c>POST api/visits</c> 
        /// returns 400 BadRequest when attempting to create a visit 
        /// with today's date (which is not allowed by business rules).
        /// </summary>
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
