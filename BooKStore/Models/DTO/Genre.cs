using System.ComponentModel.DataAnnotations;

namespace BooKStore.Models.DTO
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
