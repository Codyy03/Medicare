using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class Patient
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(11)]
        public required string PESEL { get; set; }
        
        [Required, StringLength(50)]
        public required string Name { get; set; }
       
        [Required, StringLength(50)]
        public required string Surname { get; set; }
    
        public required DateTime Birthday { get; set; }

        [Required, StringLength(255)]
        public required string Email { get; set; }

        [Required, StringLength(255)]
        public required string PhoneNumber { get; set; }

        [Required, StringLength(255)]
        public required string PasswordHash { get; set; }

        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

    }
}
