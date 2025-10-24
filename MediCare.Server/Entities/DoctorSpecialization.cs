using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediCare.Server.Entities
{
    public class DoctorSpecialization
    {
        [Key, Column(Order = 0)]
        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int SpecializationID { get; set; }
        public Specialization Specialization { get; set; } = null!;
    }
}