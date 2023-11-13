using System.ComponentModel.DataAnnotations;

namespace BooKStore.Models.DTO
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public string AuthorName { get; set; }
    }
}
