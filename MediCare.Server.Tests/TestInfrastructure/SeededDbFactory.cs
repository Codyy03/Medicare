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
    /// data for integration testing.
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

                db.Visits.RemoveRange(db.Visits);
                db.SpecializationRooms.RemoveRange(db.SpecializationRooms);
                db.Rooms.RemoveRange(db.Rooms);
                db.Doctors.RemoveRange(db.Doctors);
                db.Patients.RemoveRange(db.Patients);
                db.Specializations.RemoveRange(db.Specializations);
                db.NewsItems.RemoveRange(db.NewsItems);

                db.SaveChanges();

                db.Specializations.AddRange(
                    new Specialization
                    {
                        ID = 1,
                        SpecializationName = "Cardiologist",
                        SpecializationDescription = "Specialist in heart diseases",
                        SpecializationHighlight = "Protect your heart with expert cardiovascular care and diagnostics.",
                    },
                    new Specialization
                    {
                        ID = 2,
                        SpecializationName = "Orthopedic Surgeon",
                        SpecializationDescription = "Specialist in musculoskeletal system injuries and disorders",
                        SpecializationHighlight = "Restore mobility and strength with advanced orthopedic solutions",
                    },
                    new Specialization
                    {
                        ID = 3,
                        SpecializationName = "Dermatologist",
                        SpecializationDescription = "Specialist in skin conditions",
                        SpecializationHighlight = "Healthy skin starts here comprehensive dermatological treatments for all ages.",
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

                db.Doctors.AddRange(
                  new Doctor
                  {
                      ID = 1,
                      Name = "John",
                      Surname = "Smith",
                      Email = "john.smith@medicare.com",
                      PhoneNumber = "123456789",
                      PasswordHash = "AQAAAAIAAYagAAAAENK5qXUBaMBuUFBpttYV0aR626yy171wqlX3Fr6lZ3A63GhTGmRFWptH6uZm1Eu9Og==", //1234
                      StartHour = new TimeOnly(8, 0),
                      EndHour = new TimeOnly(16, 0),
                      Facility = "Room 203, MediCare Center",
                      DoctorDescription = "Dr. John Smith is an experienced cardiologist and surgeon with over 15 years of practice. He specializes in preventive cardiology, minimally invasive surgery, and patient-centered care."
                  },
                  new Doctor
                  {
                      ID = 2,
                      Name = "Emily",
                      Surname = "Johnson",
                      Email = "emily.johnson@medicare.com",
                      PhoneNumber = "987654321",
                      PasswordHash = "AQAAAAIAAYagAAAAEK2wr62/vPT1IadjOSNuOLLQ9ECj5CKYZbod4yvHThIexqGnCcp5Yry6PpFG9WRYYw==", //password1
                      StartHour = new TimeOnly(9, 0),
                      EndHour = new TimeOnly(17, 0),
                      Facility = "Building A, Floor 2, MediCare Center",
                      DoctorDescription = "Dr. Emily Johnson is a dedicated neurologist with over 10 years of experience. She focuses on patient-centered care, neurological diagnostics, and innovative treatment methods."
                  }
                  );
                db.Rooms.AddRange(
                    new Room { ID = 1, RoomNumber = 101, RoomType = "Cardiology Consultation Room" },
                    new Room { ID = 2, RoomNumber = 102, RoomType = "Orthopedic Consultation Room" },
                    new Room { ID = 3, RoomNumber = 103, RoomType = "Dermatology Consultation Room" },
                    new Room { ID = 5, RoomNumber = 104, RoomType = "Cardiology Consultation Room" },
                    new Room { ID = 6, RoomNumber = 105, RoomType = "Cardiology Consultation Room" },
                    new Room { ID = 7, RoomNumber = 106, RoomType = "Orthopedic Consultation Room" },
                    new Room { ID = 8, RoomNumber = 107, RoomType = "Dermatology Consultation Room" }
                );

                db.SpecializationRooms.AddRange(
                    new SpecializationRoom { SpecializationID = 1, RoomID = 1 },
                    new SpecializationRoom { SpecializationID = 1, RoomID = 5 },
                    new SpecializationRoom { SpecializationID = 1, RoomID = 6 },
                    new SpecializationRoom { SpecializationID = 2, RoomID = 2 },
                    new SpecializationRoom { SpecializationID = 2, RoomID = 7 },
                    new SpecializationRoom { SpecializationID = 3, RoomID = 3 },
                    new SpecializationRoom { SpecializationID = 3, RoomID = 8 }
                );

                db.Visits.AddRange(
                    new Visit
                    {
                        ID = 1,
                        VisitDate = new DateOnly(2025, 10, 20),
                        VisitTime = new TimeOnly(10, 30),
                        DoctorID = 1,
                        PatientID = 1,
                        Status = VisitStatus.Scheduled,
                        RoomID = 1,
                        Reason = VisitReason.Consultation,
                        AdditionalNotes = "Please discuss the test results in advance.",
                        SpecializationID = 1
                    },
                    new Visit
                    {
                        ID = 2,
                        VisitDate = new DateOnly(2025, 10, 21),
                        VisitTime = new TimeOnly(13, 00),
                        DoctorID = 2,
                        PatientID = 2,
                        Status = VisitStatus.Scheduled,
                        RoomID = 2,
                        Reason = VisitReason.Prescription,
                        AdditionalNotes = null,
                        SpecializationID = 2
                    },
                    new Visit
                    {
                        ID = 3,
                        VisitDate = new DateOnly(2025, 10, 22),
                        VisitTime = new TimeOnly(9, 30),
                        DoctorID = 1,
                        PatientID = 1,
                        Status = VisitStatus.Scheduled,
                        RoomID = 1,
                        Reason = VisitReason.Consultation,
                        SpecializationID = 1
                    },
                    new Visit
                    {
                        ID = 4,
                        VisitDate = new DateOnly(2025, 10, 27),
                        VisitTime = new TimeOnly(9, 30),
                        DoctorID = 1,
                        PatientID = 1,
                        Status = VisitStatus.Scheduled,
                        RoomID = 1,
                        Reason = VisitReason.Consultation,
                        SpecializationID = 1
                    }

                );

                db.SaveChanges();
            });
        }
    }
}
