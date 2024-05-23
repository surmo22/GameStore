namespace GameStore.Data.ViewModels
{
    public class GamesViewModel
    {
        public List<Game>? Games { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
