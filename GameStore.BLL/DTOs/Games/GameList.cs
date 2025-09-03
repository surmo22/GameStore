namespace GameStore.BLL.DTOs.Games;

public class GameList<T> where T: class
{
    public IEnumerable<T> Games { get; set; }
    
    public int Count { get; set; }
}