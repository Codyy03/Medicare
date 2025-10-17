using MediCare.Server.Entities;
using Microsoft.EntityFrameworkCore;
namespace MediCare.Server.Data
{
    public class MediCareDbContext : DbContext
    {
        public MediCareDbContext(DbContextOptions<MediCareDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitStatus> VisitStatuses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<NewsItem> NewsItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            HandleOneToMany(modelBuilder);
            HandleManyToMany(modelBuilder);
            HandleUniqueIndexes(modelBuilder);
            HandleTimeZone(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // tables seeds
            VisitStatusesSeeds(modelBuilder);
            SpecializationsSeeds(modelBuilder);
            RoomsSeeds(modelBuilder);
            DoctorsSeeds(modelBuilder);
            PatientsSeeds(modelBuilder);
            NewsSeeds(modelBuilder);

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
              .HasOne(v => v.Status)
              .WithMany()
              .HasForeignKey(v => v.StatusID)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
              .HasOne(v => v.Room)
              .WithMany()
              .HasForeignKey(v => v.RoomID)
              .OnDelete(DeleteBehavior.Restrict);
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
        }

        /// <summary>
        /// Seeds initial data for the VisitStatus table.
        /// </summary>
        void VisitStatusesSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VisitStatus>().HasData(
                new VisitStatus { ID = 1, Name = "Scheduled" },
                new VisitStatus { ID = 2, Name = "Completed" },
                new VisitStatus { ID = 3, Name = "Cancelled" }
            );
        }

        /// <summary>
        /// Seeds initial data for the Room table.
        /// </summary>
        void RoomsSeeds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().HasData(
                new Room { ID = 1, RoomNumber = 101, RoomType = "Consultation Room", Availability = true },
                new Room { ID = 2, RoomNumber = 102, RoomType = "Consultation Room", Availability = true },
                new Room { ID = 3, RoomNumber = 201, RoomType = "Operating Room", Availability = false }
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
                }
            );
        }

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
        /// <summary>
        /// Seeds initial data for the DoctorSpecialization join table (many-to-many relationship).
        /// </summary>
        /// <param name="modelBuilder"></param>
        void SpecializationRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity("DoctorSpecialization").HasData(
                new { DoctorID = 1, SpecializationID = 1 }, // John Smith - Cardiologist
                new { DoctorID = 1, SpecializationID = 3 }, // John Smith - Dermatologist
                new { DoctorID = 2, SpecializationID = 2 }  // Emily Johnson - Orthopedic Surgeon
            );
        }
        #endregion
    }
}
