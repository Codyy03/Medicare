using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class RefreshToken
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public required string Token { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public bool IsRevoked { get; set; }
        public int? PatientID { get; set; }
        public Patient? Patient { get; set; }
        public int? DoctorID { get; set; }
        public Doctor? Doctor { get; set; }

    }
}
