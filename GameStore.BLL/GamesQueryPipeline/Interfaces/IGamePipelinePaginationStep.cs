using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.GamesQueryPipeline.Interfaces;

public interface IGamePipelinePaginationStep
{
    public IQueryable<T> Paginate<T>(IQueryable<T> query);
    IEnumerable<Game> RepaginateGames(IEnumerable<Game> games);
}