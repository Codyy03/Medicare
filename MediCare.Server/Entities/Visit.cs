using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class Visit
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public DateOnly VisitDate { get; set; }

        [Required]
        public TimeOnly VisitTime { get; set; }

        [Required]
        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public VisitStatus Status { get; set; }

        [Required]
        public VisitReason Reason { get; set; }
        public string? AdditionalNotes { get; set; }

        [Required]
        public int RoomID { get; set; }
        public Room Room { get; set; }

        // 🔥 Nowe pole:
        [Required]
        public int SpecializationID { get; set; }
        public Specialization Specialization { get; set; }
    }

    public enum VisitReason
    {
        Consultation = 1,
        FollowUp = 2,
        Prescription = 3,
        Checkup = 4
    }

    public enum VisitStatus
    {
        Scheduled = 1,
        Completed = 2,
        Cancelled = 3
    }
}
