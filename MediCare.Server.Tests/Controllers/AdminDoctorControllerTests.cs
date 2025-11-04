using MediCare.Server.Controllers;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static MediCare.Server.Controllers.DoctorsController;

namespace MediCare.Server.Tests.Controllers
{
    public class AdminDoctorControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        /// <summary>
        /// Integration test that verifies updating an existing doctor by an Admin
        /// returns HTTP 200 OK and persists the updated data in the database.
        /// </summary>
        [Fact]
        public async Task UpdateAdminDoctor_ReturnOk()
        {
            var client = new SeededDbFactory().CreateClient();

            var list = await client.GetFromJsonAsync<List<DoctorDto>>("/api/doctors");

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var existing = list.First();
            var update = new AdminDoctorUpdateDto
            {
                Name = "Newname",
                Surname = existing.Surname,
                PhoneNumber = existing.PhoneNumber,
                Email = existing.Email,
                StartHour = existing.StartHour,
                EndHour = existing.EndHour,
                Facility = existing.Facility ?? "Room 203, MediCare Center",
                DoctorDescription = existing.DoctorDescription ?? "Experienced cardiologist with 15+ years of practice.",
            };

            var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Admin");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"/api/AdminDoctors/1", update);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Integration test that verifies creating a new doctor by an Admin
        /// returns HTTP 204 NoContent when the doctor is successfully registered.
        /// </summary>
        [Fact]
        public async Task CreateAdminDoctor_ReturnsCreated()
        {
            var client = new EmptyDbFactory().CreateClient();

            var dto = new AdminDoctorRegisterDto
            {
                Name = "John",
                Surname = "Smith",
                Email = "john@email.com",
                PhoneNumber = "123457777",
                Password = "Test1234!",
                StartHour = new TimeOnly(8, 0),
                EndHour = new TimeOnly(16, 0), 
                Facility = "sddfs",
                DoctorDescription = "suer",
                SpecializationIds = new List<int> { 1 }
            };

            var response = await client.PostAsJsonAsync("/api/AdminDoctors/adminDoctorRegister", dto);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
