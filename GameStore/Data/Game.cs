using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GameStore.Data
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReleaseDate { get; set; }
        public string Publisher { get; set; }
        public  string Developer { get; set; }
        public  string Platform { get; set; }
        public  string CoverImageUrl { get; set; }
        [NotNull]
        public  string TrailerUrl { get; set; }
        public  List<string> GameImages { get; set; }
        public ICollection<Genre>? Genres { get; set; }
    }
}
