using Newtonsoft.Json.Linq;

namespace GameStore.Data
{
    public class GameService
    {
        private readonly ApplicationDbContext _context;
        private readonly IgdbService _igdbService;

        public GameService(ApplicationDbContext context, IgdbService igdbService)
        {
            _context = context;
            _igdbService = igdbService;
        }

        public async Task FetchAndSaveGamesAsync()
        {
            var igdbGames = await _igdbService.GetGamesAsync();
            foreach (var igdbGame in igdbGames)
            {
                var game = MapIgdbGameToGame((JObject)igdbGame);
                await _context.Games.AddAsync(game);
            }
            await _context.SaveChangesAsync();
        }

        private Game MapIgdbGameToGame(JObject igdbGame)
        {
            return new Game
            {
                Title = igdbGame["name"]?.ToString() ?? string.Empty,
                Description = igdbGame["summary"]?.ToString() ?? string.Empty,
                Price = 0, // Set manually or from another source
                ReleaseDate = DateTime.Parse(igdbGame["release_dates"]?[0]?["human"]?.ToString() ?? DateTime.MinValue.ToString()),
                Publisher = igdbGame["involved_companies"]?[0]?["company"]?["name"]?.ToString() ?? string.Empty,
                Developer = igdbGame["involved_companies"]?[1]?["company"]?["name"]?.ToString() ?? string.Empty,
                Platform = string.Join(", ", igdbGame["platforms"].Select(p => p["name"]?.ToString() ?? string.Empty)),
                CoverImageUrl = igdbGame["cover"]?["url"]?.ToString() ?? string.Empty,
                TrailerUrl = igdbGame["videos"]?[0]?["video_id"]?.ToString() ?? string.Empty,
                GameImages = igdbGame["screenshots"].Select(s => s["url"]?.ToString() ?? string.Empty).ToList(),
                Genres = GetOrCreateGenres(igdbGame["genres"])
            };
        }

        private ICollection<Genre> GetOrCreateGenres(JToken genresToken)
        {
            var genres = new List<Genre>();
            foreach (var genreToken in genresToken)
            {
                var genreName = genreToken["name"]?.ToString();
                if (!string.IsNullOrEmpty(genreName))
                {
                    var genre = _context.Genres.FirstOrDefault(g => g.Name == genreName);
                    if (genre == null)
                    {
                        genre = new Genre { Name = genreName };
                        _context.Genres.Add(genre);
                    }
                    genres.Add(genre);
                }
            }
            return genres;
        }
    }
}