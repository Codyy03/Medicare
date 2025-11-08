using MediCare.Server.Controllers.Admin;
using MediCare.Server.Entities;
using MediCare.Server.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MediCare.Server.Tests.Controllers
{
    public class AdminSpecializationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task GetSpecializations_ReturnOk()
        {
            // arrage 
            var client = new SeededDbFactory().CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Admin");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            //act
            var response = await client.GetAsync("/api/admin/specializations");

            //assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var specialization = JsonSerializer.Deserialize<List<SpecializationDto>>(content,
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });

            Assert.NotNull(specialization);
            Assert.All(specialization, s =>
            {
                Assert.False(string.IsNullOrEmpty(s.SpecializationName));
                Assert.False(string.IsNullOrEmpty(s.SpecializationDescription));
                Assert.False(string.IsNullOrEmpty(s.SpecializationHighlight));
            });
        }

        [Fact]
        public async Task GetSpecialization_ReturnOk()
        {
            // arrage 
            var client = new SeededDbFactory().CreateClient();

            var token = TestJwtTokenHelper.GenerateTestToken("1", "john.smith@medicare.com", "John", "Admin");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            //act
            var response = await client.GetAsync("/api/admin/specializations/1");

            //assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var specialization = JsonSerializer.Deserialize<SpecializationDto>(content,
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });

            Assert.NotNull(specialization);

            Assert.False(string.IsNullOrEmpty(specialization.SpecializationName));
            Assert.False(string.IsNullOrEmpty(specialization.SpecializationDescription));
            Assert.False(string.IsNullOrEmpty(specialization.SpecializationHighlight));
        }

    }
}
