using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.EntityMappers;
using GameStore.MongoData.Interfaces;
using GameStore.MongoData.Interfaces.GameDependencyResolvers;
using GameStore.MongoData.Interfaces.MongoRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.MongoData.SharedRepositories;

public class GameRepository(IGameRepository sqlGameRepository,
    IMongoGameRepository mongoGameRepository,
    IMongoGameDependenciesResolver resolver,
    IDatabaseMigrator migrator,
    IMapper mapper,
    GameStoreContext context) : IGameRepository
{
    public async Task<GameList<Game>> GetAllGamesAsync(IGamePipeline gamePipeline,
        CancellationToken cancellationToken)
    {
        var sqlGamesTask = sqlGameRepository.GetAllGamesAsync(gamePipeline, cancellationToken);
        var mongoGamesTask = mongoGameRepository.GetAllGamesAsync(gamePipeline, cancellationToken);
        
        await Task.WhenAll(sqlGamesTask, mongoGamesTask);

        var sqlGames = await sqlGamesTask;
        var mongoGames = await mongoGamesTask;
        // deleted games are needed to determine whether we should include mongo game in a merged result
        var deletedGames = await context.Games.AsNoTracking().Where(g => g.IsDeleted).ToListAsync(cancellationToken);
        
        var mappedMongoGames = mapper.Map<IEnumerable<Game>>(mongoGames.Games);
        
        var resultSet =  GameEntityMapper.MapGamesByKey(sqlGames.Games, mappedMongoGames, deletedGames);
        resultSet = gamePipeline.ResortAndRepaginate(resultSet);
        
        return new GameList<Game>
        {
            Games = resultSet,
            Count = sqlGames.Count + mongoGames.Count,
        };
    }

    public async Task<Game?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var sqlGame = await sqlGameRepository.GetGameByIdAsync(id, cancellationToken);
        if (sqlGame is not null)
        {
            return sqlGame;
        }
        
        var mongoGame = await mongoGameRepository.GetGameByIdAsync(id, cancellationToken);
        var gameWithDependencies = await ResolveDependencies(mongoGame, cancellationToken);
        return gameWithDependencies;

    }

    public async Task<Game?> GetGameByKeyAsync(string key, CancellationToken cancellationToken)
    {
        var sqlGame = await sqlGameRepository.GetGameByKeyAsync(key, cancellationToken);
        if (sqlGame is not null)
        {
            return sqlGame;
        }
        
        var mongoGame = await mongoGameRepository.GetGameByKeyAsync(key, cancellationToken);
        var gameWithDependencies = await ResolveDependencies(mongoGame, cancellationToken);
        return gameWithDependencies;
    }

    public async Task AddGameAsync(Game game, CancellationToken cancellationToken)
    {
        await sqlGameRepository.AddGameAsync(game, cancellationToken);
    }

    public async Task UpdateGameAsync(Game game, CancellationToken cancellationToken)
    {
        if (await sqlGameRepository.GameExistsAsync(game.Id, cancellationToken))
        {
            await migrator.MigrateGenresAsync(game.Genres, cancellationToken);
            await migrator.MigratePublisherAsync(game.Publisher, cancellationToken);
            await sqlGameRepository.UpdateGameAsync(game, cancellationToken);
            return;
        }
        
        var mongoGame = await mongoGameRepository.GetGameByIdAsync(game.Id, cancellationToken) ?? throw new KeyNotFoundException("Game not found");
        var mappedGame = mapper.Map<Game>(mongoGame);
        
        await migrator.MigrateGameAsync(game, mappedGame, cancellationToken);
    }

    public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await sqlGameRepository.GameExistsAsync(id, cancellationToken))
        {
            await sqlGameRepository.DeleteGameAsync(id, cancellationToken);
            return;
        }
        
        var mongoGame = await mongoGameRepository.GetGameByIdAsync(id, cancellationToken);
        var mappedGame = await ResolveDependencies(mongoGame, cancellationToken);
        mappedGame.IsDeleted = true;
        await sqlGameRepository.AddGameAsync(mappedGame, cancellationToken);
    }

    public async Task<bool> GameExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await sqlGameRepository.GameExistsAsync(id, cancellationToken) ||
               await mongoGameRepository.GameExistsAsync(id, cancellationToken);
    }

    public async Task UpdateUnitsInStockAsync(Guid id, int delta, CancellationToken cancellationToken)
    {
        await sqlGameRepository.UpdateUnitsInStockAsync(id, delta, cancellationToken);

        if (await mongoGameRepository.GameExistsAsync(id, cancellationToken))
        {
            await mongoGameRepository.UpdateUnitsInStockAsync(id, delta, cancellationToken);
        }
    }

    public async Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await sqlGameRepository.GameExistsAsync(id, cancellationToken))
        {
            await sqlGameRepository.IncrementViewCountAsync(id, cancellationToken);
            return;
        }
        
        await mongoGameRepository.IncrementViewCountAsync(id, cancellationToken);
    }

    private async Task<Game> ResolveDependencies(MongoGame? mongoGame, CancellationToken cancellationToken)
    {
        if (mongoGame is null)
        {
            return null;
        }

        var mappedMongoGame = mapper.Map<Game>(mongoGame);
        var gameWithDependencies = await resolver.ResolveGameDependenciesAsync(mappedMongoGame, mongoGame, cancellationToken);
        return gameWithDependencies;
    }
}