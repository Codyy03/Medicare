using MediCare.Server.Controllers;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MediCare.Server.Entities;
using MediCare.Server.Helpers;
using Xunit;
using static MediCare.Server.Entities.Enums;

namespace MediCare.Server.Tests.Controllers
{
    public class AdminPatientsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        /// <summary>
        /// Integration test that verifies retrieving all patients by an Admin
        /// returns HTTP 200 OK and a non-empty list of patients.
        /// </summary>
        [Fact]
        public async Task GetAllAdminPatients_ReturnsOk()
        {
            var client = new SeededDbFactory().CreateClient();
            var token = TestJwtTokenHelper.GenerateTestToken("1", "admin@medicare.com", "Admin", "Admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/AdminPatients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var patients = await response.Content.ReadFromJsonAsync<List<Patient>>();
            Assert.NotNull(patients);
            Assert.NotEmpty(patients);
        }

        /// <summary>
        /// Integration test that verifies retrieving a specific patient by ID
        /// returns HTTP 200 OK and the correct patient data.
        /// </summary>
        [Fact]
        public async Task GetAdminPatientById_ReturnsOk()
        {
            var client = new SeededDbFactory().CreateClient();
            var token = TestJwtTokenHelper.GenerateTestToken("1", "admin@medicare.com", "Admin", "Admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/AdminPatients/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var patient = await response.Content.ReadFromJsonAsync<Patient>();
            Assert.NotNull(patient);
            Assert.Equal(1, patient.ID);
        }

        /// <summary>
        /// Integration test that verifies creating a new patient by an Admin
        /// returns HTTP 201 Created when the patient is successfully registered.
        /// </summary>
        [Fact]
        public async Task CreateAdminPatient_ReturnsCreated()
        {
            var client = new EmptyDbFactory().CreateClient();
            var token = TestJwtTokenHelper.GenerateTestToken("1", "admin@medicare.com", "Admin", "Admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dto = new AdminPatientRegisterModel
            {
                PESEL = "12345678901",
                Name = "Anna",
                Surname = "Kowalska",
                Email = "anna.kowalska@example.com",
                PhoneNumber = "123456789",
                Password = "StrongP@ss1",
                Birthday = DateTime.Today
            };

            var response = await client.PostAsJsonAsync("/api/AdminPatients", dto);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Integration test that verifies updating an existing patient by an Admin
        /// returns HTTP 200 OK and persists the updated data in the database.
        /// </summary>
        [Fact]
        public async Task UpdateAdminPatient_ReturnsOk()
        {
            var client = new SeededDbFactory().CreateClient();
            var token = TestJwtTokenHelper.GenerateTestToken("1", "admin@medicare.com", "Admin", "Admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var patients = await client.GetFromJsonAsync<List<Patient>>("/api/AdminPatients");
            Assert.NotNull(patients);
            Assert.NotEmpty(patients);

            var existing = patients.First();

            var update = new AdminPatientUpdateModel
            {
                PESEL = existing.PESEL,
                Name = "UpdatedName",
                Surname = "UpdatedSurname",
                Email = "updated@example.com",
                PhoneNumber = "987654321",
                Birthday = DateTime.Today,
                Status = Status.Active
            };

            var response = await client.PutAsJsonAsync($"/api/AdminPatients/{existing.ID}", update);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Integration test that verifies deleting an existing patient by an Admin
        /// returns HTTP 204 NoContent and removes the patient from the database.
        /// </summary>
        [Fact]
        public async Task DeleteAdminPatient_ReturnsNoContent()
        {
            var client = new SeededDbFactory().CreateClient();
            var token = TestJwtTokenHelper.GenerateTestToken("1", "admin@medicare.com", "Admin", "Admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync("/api/AdminPatients/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
