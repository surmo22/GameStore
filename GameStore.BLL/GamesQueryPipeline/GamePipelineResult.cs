namespace GameStore.BLL.GamesQueryPipeline;

public class GamePipelineResult<T>
{
    public IQueryable<T> PaginatedQuery { get; set; } = null!;
    
    public Task<int> TotalCountTask { get; set; } = null!;
}