using MediCare.Server.Data;
using MediCare.Server.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;

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
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.ChangeTracker.Clear();

                if (db.Specializations.Any())
                    db.Specializations.RemoveRange(db.Specializations);

                if (db.NewsItems.Any())
                    db.NewsItems.RemoveRange(db.NewsItems);

                if (db.Patients.Any())
                    db.Patients.RemoveRange(db.Patients);

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
                db.NewsItems.AddRange(
                    new NewsItem
                    {
                        ID = 1,
                        Title = "Free Blood Pressure Screening",
                        Description = "Join us for a free blood pressure check and consultation with our cardiology team.",
                        ImageURL = "https://i.ibb.co/k2hBfcpL/blood-pressure.jpg",
                        Date = new DateTime(2025, 10, 5, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new NewsItem
                    {
                        ID = 2,
                        Title = "Flu Vaccination Campaign",
                        Description = "Get your flu shot before the season starts. No appointment needed.",
                        ImageURL = "https://i.ibb.co/BHxNtvLj/vaccination.jpg",
                        Date = new DateTime(2025, 11, 12, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new NewsItem
                    {
                        ID = 3,
                        Title = "Healthy Eating Workshop",
                        Description = "Learn how to prepare balanced meals with our nutritionist. Free entry.",
                        ImageURL = "https://i.ibb.co/HTVch19N/healthy-eating.jpg",
                        Date = new DateTime(2025, 9, 25, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new NewsItem
                    {
                        ID = 4,
                        Title = "World Diabetes Day Awareness",
                        Description = "Educational lectures and free glucose testing for all visitors.",
                        ImageURL = "https://i.ibb.co/1VTGg5c/diabetes.jpg",
                        Date = new DateTime(2025, 11, 14, 0, 0, 0, DateTimeKind.Utc)
                    }
                    );

                db.Patients.AddRange(
                 new Patient
                   {
                       ID = 1,
                       PESEL = "90010112345",
                       Name = "Michael",
                       Surname = "Brown",
                       Birthday = new DateTime(1990, 1, 1),
                       Email = "michael.brown@example.com",
                       PhoneNumber = "555111222",
                       PasswordHash = "AQAAAAIAAYagAAAAELg5sSlJ9Z8pG1rKefpy1Pbfql3D+S2J7bXiM77uUOfyfdvsBcOdh75oC42ktQ8h3w==" // doctor1
                   },
                   new Patient
                   {
                       ID = 2,
                       PESEL = "85050567890",
                       Name = "Sarah",
                       Surname = "Williams",
                       Birthday = new DateTime(1985, 5, 5),
                       Email = "sarah.williams@example.com",
                       PhoneNumber = "555333444",
                       PasswordHash = "AQAAAAIAAYagAAAAELo4ftbkWmWkCzg4YbZhATzJFIRg6s7HBY322tgk4mquh9bHdQy3NraDhJLvnhjJEQ==" // doctor1
                   }
                );

                db.SaveChanges();
            });
        }
    }
}
