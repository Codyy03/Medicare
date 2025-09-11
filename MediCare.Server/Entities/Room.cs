using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class Room
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int RoomNumber { get; set; }
      
        [Required, StringLength(255)]
        public string RoomType { get; set; } = string.Empty;

        [Required]
        public bool Availability { get; set; }
    }
}
