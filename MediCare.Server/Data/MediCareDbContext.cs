using Microsoft.EntityFrameworkCore;
using MediCare.Server.Entities;

namespace MediCare.Server.Data
{
    public class MediCareDbContext : DbContext
    {
        public MediCareDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitStatus> VisitStatuses { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
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

            base.OnModelCreating(modelBuilder);
        }

    }
}
