using AutoMapper;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.Interfaces.GameDependencyResolvers;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.MongoData.MongoRepositories.GameDependencyResolvers;

public class GenreResolver(NorthwindMongoContext context, GameStoreContext sqlContext, IMapper mapper) : IMongoGameDependencyResolver
{
    public async Task ResolveDependency(Game game, MongoGame mongoGame, CancellationToken cancellationToken)
    {
        if (mongoGame.CategoryId > 0)
        {
            var mongoGenre = await context.Genres.AsQueryable()
                .Where(g => g.CategoryId == mongoGame.CategoryId)
                .FirstOrDefaultAsync(cancellationToken);
            var genre = mapper.Map<Genre>(mongoGenre);
            var sqlGenre = await sqlContext.Genres.FirstOrDefaultAsync(g => genre.Id == g.Id, cancellationToken);
            game.Genres.Add(sqlGenre ?? genre);
        }
    }
}