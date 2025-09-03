using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Interfaces.GameServices.ImageServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Interfaces.Security;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.EntityServices;

public class GameService(IUnitOfWork unitOfWork,
    IGameSetupService gameSetupService,
    IMapper mapper,
    IGameCountService gameCountService,
    IGamePipelineFactory pipelineFactory,
    ICurrentUserService currentUserService,
    IGameImageService gameImageService) : IGameService
{
    public async Task<GameDto> CreateGameAsync(GameCreateRequestDto gameCreateDto, CancellationToken cancellationToken)
    {
        var game = mapper.Map<Game>(gameCreateDto.Game);
        
        await gameSetupService.BuildGameAsync(game, gameCreateDto, cancellationToken);
        await unitOfWork.Games.AddGameAsync(game, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        gameCountService.IncrementTotalGamesCount();
        return mapper.Map<GameDto>(game);
    }

    public async Task<GameDto> UpdateGameAsync(GameUpdateRequestDto gameUpdateDto, CancellationToken cancellationToken)
    {
        var existingGame = await unitOfWork.Games.GetGameByIdAsync(gameUpdateDto.Game.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"No game was found with Id {gameUpdateDto.Game.Id}");
        
        if (existingGame.IsDeleted && !await currentUserService
                .HasPermissionToAsync(nameof(UserPermissionTypes.EditDeletedGames), cancellationToken))
        {
            throw new UnauthorizedAccessException("You don't have permission to edit deleted games.");
        }
        
        await gameImageService.DeleteImageAsync(existingGame.ImageUrl!, cancellationToken);
        
        var game = mapper.Map(gameUpdateDto.Game, existingGame);
        
        await gameSetupService.BuildGameAsync(game, gameUpdateDto, cancellationToken);
        await unitOfWork.Games.UpdateGameAsync(game, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return mapper.Map<GameDto>(game);
    }

    public async Task DeleteGameAsync(string key, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetGameByKeyAsync(key, cancellationToken) 
                   ?? throw new KeyNotFoundException($"No game was found with Key {key}");

        await gameImageService.DeleteImageAsync(game.ImageUrl, cancellationToken);

        await unitOfWork.Games.DeleteGameAsync(game.Id, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        gameCountService.DecrementTotalGamesCount();
    }

    public async Task<GameDto?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetGameByIdAsync(id, cancellationToken);
        if (game is { IsDeleted: true } && !await currentUserService
                .HasPermissionToAsync(nameof(UserPermissionTypes.ViewDeletedGames), cancellationToken))
        {
            throw new UnauthorizedAccessException("You don't have permission to view deleted games.");
        }
        return mapper.Map<GameDto>(game);
    }

    public async Task<PaginatedList<GameDto>> GetAllGamesAsync(
        GameFilterRequest filterRequest,
        CancellationToken cancellationToken)
    {
        var hasPermissionToViewDeletedGames = await currentUserService
            .HasPermissionToAsync(nameof(UserPermissionTypes.ViewDeletedGames), cancellationToken);
        
        var gamePipeline = pipelineFactory.Create(filterRequest, hasPermissionToViewDeletedGames);
        
        var games = await unitOfWork.Games.GetAllGamesAsync(gamePipeline, cancellationToken);
        var mappedGames = mapper.Map<IEnumerable<GameDto>>(games.Games).ToList();
        var pageSize = GetPageSize(filterRequest);

        var paginatedGames = new PaginatedList<GameDto>()
        {
            Games = mappedGames,
            TotalPages = (int)Math.Ceiling((double)games.Count / pageSize),
            CurrentPage = filterRequest.Page,
        };
        return paginatedGames;
    }

    public async Task<(byte[] Content, string ContentType)> GetImageByGameKeyAsync(string key,
        CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetGameByKeyAsync(key, cancellationToken) ??
                   throw new KeyNotFoundException($"No Game was found with Key {key}");
        return await gameImageService.GetImage(game.ImageUrl, cancellationToken);
    }


    public async Task<IEnumerable<GenreDto>> GetGenresByGameKeyAsync(string gameKey, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetGameByKeyAsync(gameKey, cancellationToken) 
                   ?? throw new KeyNotFoundException($"No Game was found with Key {gameKey}");
        var genres = game.Genres.Select(mapper.Map<GenreDto>);
        return genres;
    }

    public async Task<IEnumerable<PlatformDto>> GetPlatformsByGameKeyAsync(string gameKey, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetGameByKeyAsync(gameKey, cancellationToken) 
                   ?? throw new KeyNotFoundException($"No Game was found with Key {gameKey}");
        var platforms = game.Platforms.Select(mapper.Map<PlatformDto>);
        return platforms;
    }

    public async Task<PublisherDto> GetPublisherByGameKeyAsync(string gameKey, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetGameByKeyAsync(gameKey, cancellationToken) 
                   ?? throw new KeyNotFoundException($"No Game was found with Key {gameKey}");
        var publisher = mapper.Map<PublisherDto>(game.Publisher);
        return publisher;
    }

    public async Task<GameDto?> GetGameByKeyAsync(string gameKey, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetGameByKeyAsync(gameKey, cancellationToken);
        if (game is { IsDeleted: true } && !await currentUserService
                .HasPermissionToAsync(nameof(UserPermissionTypes.ViewDeletedGames), cancellationToken))
        {
            throw new UnauthorizedAccessException("You don't have permission to view deleted games.");
        }

        await unitOfWork.Games.IncrementViewCountAsync(game.Id, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return mapper.Map<GameDto>(game);
    }

    public async Task UpdateGameStockAsync(Guid id, int delta, CancellationToken cancellationToken)
    {
        await unitOfWork.Games.UpdateUnitsInStockAsync(id, delta, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<IList<GameDto>> GetAllGamesWithoutFilters(CancellationToken cancellationToken)
    {
        var pipeline = pipelineFactory.Create(new GameFilterRequest(), true);
        var games = await unitOfWork.Games.GetAllGamesAsync(pipeline, cancellationToken);

        return mapper.Map<IList<GameDto>>(games);
    }
    private static int GetPageSize(GameFilterRequest filterRequest)
    {
        int pageSize;
        if (filterRequest.PageCount == "all" || string.IsNullOrEmpty(filterRequest.PageCount))
        {
            pageSize = 1;
        }
        else
        {
            pageSize = int.Parse(filterRequest.PageCount);
        }

        return pageSize;
    }
}
