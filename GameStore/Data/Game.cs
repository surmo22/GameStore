using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GameStore.Data
{
    public class Game
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public string? Publisher { get; set; }
        [Required]
        public string? Developer { get; set; }
        [Required]
        public string? Platform { get; set; }
        [Required]
        public string? CoverImageUrl { get; set; }
        [Required]
        public string? TrailerUrl { get; set; }
        [Required]
        public List<string>? GameImages { get; set; }
        public ICollection<Genre>? Genres { get; set; }
    }
}
