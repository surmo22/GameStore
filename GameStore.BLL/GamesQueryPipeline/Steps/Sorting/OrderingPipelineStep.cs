using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Sorting;

public class OrderingPipelineStep(GameFilterRequest request) : IGamePipelineOrderingStep
{
    public IQueryable<Game> OrderGames(IQueryable<Game> games)
    {
        if (!request.Sort.HasValue)
        {
            return games;
        }

        games = request.Sort.Value switch
        {
            SortingOptions.MostPopular => games.OrderByDescending(g => g.ViewCount),
            SortingOptions.MostCommented => games.OrderByDescending(g => g.Comments.Count),
            SortingOptions.PriceAscending => games.OrderBy(g => g.Price),
            SortingOptions.PriceDescending => games.OrderByDescending(g => g.Price),
            SortingOptions.New => games.OrderByDescending(g => g.CreationDate),
            _ => throw new InvalidOperationException("Invalid sorting option")
        };

        return games;
    }

    public IQueryable<MongoGame> OrderGames(IQueryable<MongoGame> games)
    {
        if (!request.Sort.HasValue)
        {
            return games;
        }
        
        games = request.Sort.Value switch
        {
            SortingOptions.MostPopular => games.OrderBy(g => g.ViewCount),
            SortingOptions.MostCommented => games, // no comments in mongo
            SortingOptions.PriceAscending => games.OrderBy(g => g.UnitPrice),
            SortingOptions.PriceDescending => games.OrderByDescending(g => g.UnitPrice),
            SortingOptions.New => games, // no creation date in mongo
            _ => throw new InvalidOperationException("Invalid sorting option")
        };

        return games;
    }
}