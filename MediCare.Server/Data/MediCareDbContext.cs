using MediCare.Server.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace MediCare.Server.Data
{
    public class MediCareDbContext : DbContext
    {
        public MediCareDbContext(DbContextOptions<MediCareDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<SpecializationRoom> SpecializationRooms { get; set; }
        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            HandleOneToOne(modelBuilder);
            HandleOneToMany(modelBuilder);
            HandleManyToMany(modelBuilder);
            HandleUniqueIndexes(modelBuilder);
            HandleTimeZone(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // tables seeds
            SpecializationsSeeds(modelBuilder);
            RoomsSeeds(modelBuilder);
            DoctorsSeeds(modelBuilder);
            PatientsSeeds(modelBuilder);
            VisitsSeeds(modelBuilder);
            NewsSeeds(modelBuilder);
            SpecializationRoomSeeds(modelBuilder);
            SpecializationRelationships(modelBuilder);
        }

        #region Relationships configuration
        /// <summary>
        /// Configures one-to-many relationships between entities in the model.
        /// </summary>
        void HandleOneToMany(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Doctor)
                .WithMany(d => d.Visits)
                .HasForeignKey(v => v.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Room)
                .WithMany()
                .HasForeignKey(v => v.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Specialization)
                .WithMany()
                .HasForeignKey(v => v.SpecializationID)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Configures one-to-one relationships between entities in the model.
        /// </summary>
        void HandleOneToOne(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Patient)
                .WithOne(p => p.RefreshToken) 
                .HasForeignKey<RefreshToken>(rt => rt.PatientID);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Doctor)
                .WithOne(d => d.RefreshToken)
                .HasForeignKey<RefreshToken>(rt => rt.DoctorID);
        }

        /// <summary>
        /// Configures many-to-many relationships between entities in the model.
        /// </summary>
        void HandleManyToMany(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
            .HasMany(d => d.Specializations)
            .WithMany(s => s.Doctors)
                .UsingEntity<Dictionary<string, object>>(
            "DoctorSpecialization",
            j => j
                .HasOne<Specialization>()
                .WithMany()
                .HasForeignKey("SpecializationID")
                .OnDelete(DeleteBehavior.Cascade),
            j => j
                .HasOne<Doctor>().
                WithMany()
                .HasForeignKey("DoctorID")
                .OnDelete(DeleteBehavior.Cascade),
            j =>
            {
                j.HasKey("DoctorID", "SpecializationID");
            });

            modelBuilder.Entity<SpecializationRoom>()
                .HasKey(sr => new { sr.SpecializationID, sr.RoomID });
        }
        #endregion

        #region Indexes and TimeZone
        /// <summary>
        /// Configures unique indexes for selected entity properties.
        /// </summary>
        void HandleUniqueIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.PESEL)
                .IsUnique();

            modelBuilder.Entity<Patient>()
                .HasIndex(d => d.Email)
                .IsUnique();

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();
        }
        void HandleTimeZone(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
             .Property(p => p.Birthday)
             .HasColumnType("timestamp without time zone");
        }
        #endregion

        #region Seeds
        /// <summary>
        /// Seeds initial data for the Specialization table.
        /// </summary>
        void SpecializationsSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Specialization>().HasData(
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
        }

        /// <summary>
        /// Seeds initial data for the Room table.
        /// </summary>
        void RoomsSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().HasData(
                new Room { ID = 1, RoomNumber = 101, RoomType = "Cardiology Consultation Room" },
                new Room { ID = 2, RoomNumber = 102, RoomType = "Orthopedic Consultation Room" },
                new Room { ID = 3, RoomNumber = 103, RoomType = "Dermatology Consultation Room" },
                new Room { ID = 5, RoomNumber = 104, RoomType = "Cardiology Consultation Room" },
                new Room { ID = 6, RoomNumber = 106, RoomType = "Orthopedic Consultation Room" },
                new Room { ID = 8, RoomNumber = 107, RoomType = "Dermatology Consultation Room" }
            );
        }

        /// <summary>
        /// Seeds initial data for the Doctor table, including hashed passwords.
        /// </summary>
        void DoctorsSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>().HasData(
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
                },

                new Doctor
                {
                    ID = 3,
                    Name = "Michael",
                    Surname = "Anderson",
                    Email = "michael.anderson@medicare.com",
                    PhoneNumber = "555111222",
                    PasswordHash = "AQAAAAIAAYagAAAAEP8zA5dHE7nyVqBLoZwbn3FPUcUJDN4lB3E4uIv9H1sF5xW0+2Fi7vsmzJYgPNh8lA==", // pass123
                    StartHour = new TimeOnly(8, 0),
                    EndHour = new TimeOnly(14, 0),
                    Facility = "Room 104, MediCare Center",
                    DoctorDescription = "Dr. Michael Anderson is a skilled cardiologist specializing in arrhythmia management and cardiac imaging diagnostics."
                },
                new Doctor
                {
                    ID = 4,
                    Name = "Sophia",
                    Surname = "Martinez",
                    Email = "sophia.martinez@medicare.com",
                    PhoneNumber = "555333444",
                    PasswordHash = "AQAAAAIAAYagAAAAEPxZkz/F7fYxzUVEXn9xIQKo5Tk1DbiRdnXx7SC7pPjUJW1xVdYzqk5GfYjxyK6K+w==", // heart123
                    StartHour = new TimeOnly(10, 0),
                    EndHour = new TimeOnly(18, 0),
                    Facility = "Room 105, MediCare Center",
                    DoctorDescription = "Dr. Sophia Martinez has extensive experience in non-invasive cardiology and preventive heart health."
                },
                new Doctor
                {
                    ID = 5,
                    Name = "David",
                    Surname = "Kowalski",
                    Email = "david.kowalski@medicare.com",
                    PhoneNumber = "555666777",
                    PasswordHash = "AQAAAAIAAYagAAAAEKh2N3ZC1HkP7HhKJ7blQ7tb4jHY0mM3PwOydZx5Lq3yW6SkmtFfw4tq1LZzW9zENg==", // cardio1
                    StartHour = new TimeOnly(7, 30),
                    EndHour = new TimeOnly(15, 30),
                    Facility = "Room 101, MediCare Center",
                    DoctorDescription = "Dr. David Kowalski is a cardiologist focused on hypertension treatment, cardiac stress testing, and patient education."
                }
            );
        }


        /// <summary>
        /// Seeds initial data for the Patient table, including hashed passwords.
        /// </summary>
        void PatientsSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasData(
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
                },
                new Patient
                {
                    ID = 3,
                    PESEL = "80010112345",
                    Name = "Anna",
                    Surname = "Kowalska",
                    Birthday = new DateTime(1980, 1, 1),
                    Email = "anna.kowalska@example.com",
                    PhoneNumber = "555777888",
                    PasswordHash = "..."
                },
                new Patient
                {
                    ID = 4,
                    PESEL = "75050567890",
                    Name = "Piotr",
                    Surname = "Nowak",
                    Birthday = new DateTime(1975, 5, 5),
                    Email = "piotr.nowak@example.com",
                    PhoneNumber = "555999000",
                    PasswordHash = "..."
                }
            );
        }

        /// <summary>
        /// Seeds initial data for the NewsItem table, including sample announcements and events.
        /// </summary>
        void NewsSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewsItem>().HasData(
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
        }

        void SpecializationRoomSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpecializationRoom>().HasData(
                new SpecializationRoom { SpecializationID = 1, RoomID = 1 },
                new SpecializationRoom { SpecializationID = 1, RoomID = 5 },

                new SpecializationRoom { SpecializationID = 2, RoomID = 2 },
                new SpecializationRoom { SpecializationID = 2, RoomID = 6 },

                new SpecializationRoom { SpecializationID = 3, RoomID = 3 },
                new SpecializationRoom { SpecializationID = 3, RoomID = 8 }
            );
        }

        /// <summary>
        /// Seeds initial data for the Visit table, linking doctors, patients, statuses, and rooms with specific dates and times.
        /// </summary>
        void VisitsSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Visit>().HasData(
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
                    SpecializationID = 1,
                    VisitNotes = null,
                    PrescriptionText = null
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
                    SpecializationID = 2,
                    VisitNotes = null,
                    PrescriptionText = null
                },
                new Visit
                {
                    ID = 3,
                    VisitDate = new DateOnly(2025, 10, 22),
                    VisitTime = new TimeOnly(9, 30),
                    DoctorID = 1,
                    PatientID = 2,
                    Status = VisitStatus.Completed,
                    RoomID = 1,
                    Reason = VisitReason.FollowUp,
                    AdditionalNotes = "Checkup after previous visit.",
                    SpecializationID = 1,
                    VisitNotes = "Patient recovering well, continue current treatment.",
                    PrescriptionText = "Vitamin D 2000 IU daily for 3 months"
                },
                new Visit
                {
                    ID = 4,
                    VisitDate = new DateOnly(2025, 10, 25),
                    VisitTime = new TimeOnly(11, 00),
                    DoctorID = 3,
                    PatientID = 3,
                    Status = VisitStatus.Completed,
                    RoomID = 3,
                    Reason = VisitReason.Checkup,
                    AdditionalNotes = "Routine annual checkup.",
                    SpecializationID = 3,
                    VisitNotes = "All vitals normal, recommend regular exercise.",
                    PrescriptionText = null
                },
                new Visit
                {
                    ID = 5,
                    VisitDate = new DateOnly(2025, 10, 27),
                    VisitTime = new TimeOnly(15, 00),
                    DoctorID = 2,
                    PatientID = 4,
                    Status = VisitStatus.Scheduled,
                    RoomID = 2,
                    Reason = VisitReason.Consultation,
                    AdditionalNotes = "Patient complains of recurring headaches.",
                    SpecializationID = 2,
                    VisitNotes = null,
                    PrescriptionText = null
                }
            );
        }

        /// <summary>
        /// Seeds initial data for the DoctorSpecialization join table (many-to-many relationship).
        /// </summary>
        /// <param name="modelBuilder"></param>
        void SpecializationRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity("DoctorSpecialization").HasData(
                new { DoctorID = 1, SpecializationID = 1 }, // John Smith - Cardiologist
                new { DoctorID = 1, SpecializationID = 3 }, // John Smith - Dermatologist
                new { DoctorID = 2, SpecializationID = 2 }, // Emily Johnson - Orthopedic Surgeon

                new { DoctorID = 3, SpecializationID = 1 }, // Michael Anderson
                new { DoctorID = 4, SpecializationID = 1 }, // Sophia Martinez
                new { DoctorID = 5, SpecializationID = 1 }  // David Kowalski
            );
        }

        #endregion
    }
}
