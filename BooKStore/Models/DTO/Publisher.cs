using System.ComponentModel.DataAnnotations;

namespace BooKStore.Models.DTO
{
    public class Publisher
    {
        public int Id { get; set; }
        [Required]
        public string PublisherName { get; set; }
    }
}
