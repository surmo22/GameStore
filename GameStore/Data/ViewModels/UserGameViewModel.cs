namespace GameStore.Data.ViewModels
{
    public class UserGameViewModel
    {
        public required Game Game { get; set; }
        
        public required IEnumerable<Key> Keys { get; set; }
    }
}
