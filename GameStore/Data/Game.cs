using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GameStore.Data
{
    public class Game
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReleaseDate { get; set; }
        public required string Publisher { get; set; }
        public required string Developer { get; set; }
        public required string Platform { get; set; }
        public required string CoverImageUrl { get; set; }
        [NotNull]
        public required string TrailerUrl { get; set; }
        public required List<string> GameImages { get; set; }
        public ICollection<Genre>? Genres { get; set; }
    }
}
