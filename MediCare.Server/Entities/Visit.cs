using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class Visit
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public DateOnly VisitDate {  get; set; }

        [Required]
        public TimeOnly VisitTime { get; set; }

        [Required]
        public int DoctorID { get; set; }
        public Doctor Doctor{ get; set; }

        [Required]
        public int PatientID { get; set; }
        public  Patient Patient { get; set; }

        [Required]
        public int StatusID { get; set; }
        public VisitStatus Status { get; set; }

        [Required]
        public VisitReason Reason { get; set; }
        public string? AdditionalNotes { get; set; }

        [Required]
        public int RoomID { get; set; }
        public Room Room { get; set; }
    }

    public enum VisitReason
    {
        Consultation = 1,
        FollowUp = 2,
        Prescription = 3,
        Checkup = 4
    }

    public class VisitStatus
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(255)]
        public required string Name { get; set; }
    }
}
