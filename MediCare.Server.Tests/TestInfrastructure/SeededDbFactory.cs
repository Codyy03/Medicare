using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediCare.Server.Tests.TestInfrastructure
{
    /// <summary>
    /// A custom <see cref="WebApplicationFactory{TEntryPoint}"/> that configures
    /// an in-memory database named "TestDb" and pre-populates it with sample
    /// specialization records for integration testing.
    /// The database is cleared before seeding to ensure a consistent state
    /// across tests.
    /// </summary>
    public class SeededDbFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MediCareDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<MediCareDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MediCareDbContext>();
                db.Database.EnsureCreated();

                db.Specializations.RemoveRange(db.Specializations);
                db.SaveChanges();

                db.Specializations.AddRange(
                    new Specialization
                    {
                        ID = 1,
                        SpecializationName = "Cardiologist",
                        SpecializationDescription = "Specialist in heart diseases",
                        SpecializationHighlight = "Protect your heart with expert cardiovascular care and diagnostics.",
                        Link = "#"
                    },
                    new Specialization
                    {
                        ID = 2,
                        SpecializationName = "Orthopedic Surgeon",
                        SpecializationDescription = "Specialist in musculoskeletal system injuries and disorders",
                        SpecializationHighlight = "Restore mobility and strength with advanced orthopedic solutions",
                        Link = "#"
                    },
                    new Specialization
                    {
                        ID = 3,
                        SpecializationName = "Dermatologist",
                        SpecializationDescription = "Specialist in skin conditions",
                        SpecializationHighlight = "Healthy skin starts here comprehensive dermatological treatments for all ages.",
                        Link = "#"
                    }
                );
                db.SaveChanges();
            });
        }
    }
}
