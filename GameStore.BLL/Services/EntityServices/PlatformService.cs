using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.EntityServices;

public class PlatformService(IGuidProvider guidProvider, IUnitOfWork unitOfWork, IMapper mapper) : IPlatformService
{
    public async Task<PlatformDto> CreatePlatformAsync(PlatformCreateDto platformCreateDto, CancellationToken cancellationToken)
    {
        var platform = mapper.Map<Platform>(platformCreateDto);
        platform.Id = guidProvider.NewGuid();

        await unitOfWork.Platforms.AddPlatformAsync(platform, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return mapper.Map<PlatformDto>(platform);
    }

    public async Task DeletePlatformAsync(Guid id, CancellationToken cancellationToken)
    {
        await EnsurePlatformExistsAsync(id, cancellationToken);

        await unitOfWork.Platforms.DeletePlatformAsync(id, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<PlatformDto>> GetAllPlatformsAsync(CancellationToken cancellationToken)
    {
        var platforms = await unitOfWork.Platforms.GetAllPlatformsAsync(cancellationToken);
        return mapper.Map<IEnumerable<PlatformDto>>(platforms);
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPlatformIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var platform = await unitOfWork.Platforms.GetPlatformByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"No Platform was found with Id {id}");
        var games = platform.Games.Select(mapper.Map<GameDto>);
        return games;
    }

    public async Task<PlatformDto?> GetPlatformByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var platform = await unitOfWork.Platforms.GetPlatformByIdAsync(id, cancellationToken);
        return mapper.Map<PlatformDto>(platform);
    }

    public async Task UpdatePlatformAsync(PlatformDto platformCreateDto, CancellationToken cancellationToken)
    {
        var platform = mapper.Map<Platform>(platformCreateDto);
        await EnsurePlatformExistsAsync(platform.Id, cancellationToken);

        unitOfWork.Platforms.UpdatePlatformAsync(platform);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<ICollection<Platform>> GetPlatformsByIdsAsync(List<Guid>? platformIds, CancellationToken cancellationToken)
    {
        var platforms = new List<Platform>();
        foreach (var id in platformIds)
        {
            var platform = await unitOfWork.Platforms.GetPlatformByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Platform with id {id} was not found");
            platforms.Add(platform);
        }

        return platforms;
    }

    private async Task EnsurePlatformExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.Platforms.PlatformExistsAsync(id, cancellationToken))
        {
            throw new KeyNotFoundException($"No Platform was found with ID {id}");
        }
    }
}
