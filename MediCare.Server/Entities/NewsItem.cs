using System.ComponentModel.DataAnnotations;

namespace MediCare.Server.Entities
{
    public class NewsItem
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(255)]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        public string ImageURL { get; set; }

        [Required]
        public DateTime Date { get; set; }


    }
}
