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
        public int DoctorID { get; set; }
        public required Doctor Doctor{ get; set; }

        [Required]
        public int PatientID { get; set; }
        public required Patient Patient { get; set; }

        [Required]
        public int StatusID { get; set; }
        public required VisitStatus Status { get; set; }

        [Required]
        public int RoomID { get; set; }
        public required Room Room { get; set; }
    }

    public class VisitStatus
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(255)]
        public required string Name { get; set; }
    }
}
