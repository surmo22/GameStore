namespace GameStore.Data.ViewModels
{
    public class ReccomendationViewModel
    {
        public required List<Game> Games { get; set; }
        public required List<Genre> Genres { get; set; }
        public int? SelectedGameId { get; set; }
        public IList<int>? SelectedGenreIds { get; set; }
        public IList<Game>? RecommendedGames { get; set; }
    }
}
