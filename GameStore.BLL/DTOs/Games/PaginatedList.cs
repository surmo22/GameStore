namespace GameStore.BLL.DTOs.Games;

public class PaginatedList<T>
{
    public IEnumerable<T> Games { get; set; }

    public int TotalPages { get; set; } = 10;

    public int CurrentPage { get; set; } = 40;
}