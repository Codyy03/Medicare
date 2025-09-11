using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class Doctor
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(50)]
        public required string Name { get; set; }

        [Required, StringLength(50)]
        public required string Surname { get; set; }

        [Required, StringLength(255)]
        public required string Email { get; set; }

        [Required, StringLength(255)]
        public required string PhoneNumber { get; set; }

        [Required, StringLength(255)]
        public required string PasswordHash { get; set; }

        [Required]
        public required TimeOnly StartHour { get; set; }
      
        [Required]
        public required TimeOnly EndHour { get; set; }
    
        public ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

    }
}
