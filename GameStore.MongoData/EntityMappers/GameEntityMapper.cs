using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.MongoData.EntityMappers;

public static class GameEntityMapper
{
    public static IEnumerable<Game> MapGamesByKey(IEnumerable<Game> sqlGames, IEnumerable<Game> mongoGames,
        IList<Game> deletedGames)
    {
        var games = new List<Game>(sqlGames);

        foreach (var mongoGame in mongoGames)
        {
            var exists = deletedGames.Any(g => g.Key == mongoGame.Key || g.Id == mongoGame.Id);
            exists = exists || games.Any(g => g.Key == mongoGame.Key || g.Id == mongoGame.Id);
            if (!exists)
            {
                games.Add(mongoGame);
            }
        }

        return games;
    }
}