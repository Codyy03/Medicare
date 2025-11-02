using System.ComponentModel.DataAnnotations;
using static MediCare.Server.Entities.Enums;

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

        [Required]
        public int SpecializationID { get; set; }
        public Specialization Specialization { get; set; }

        public string? VisitNotes { get; set; }
        public string? PrescriptionText { get; set; }
    }
}
