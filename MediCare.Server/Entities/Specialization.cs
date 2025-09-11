using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class Specialization
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(255)]
        public required string SpecializationName { get; set; }
        
        [Required]
        public required string SpecializationDescription { get; set; }
    
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
