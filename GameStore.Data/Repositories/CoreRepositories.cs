using GameStore.BLL.Interfaces.Repositories;

namespace GameStore.Data.Repositories;

public class CoreRepositories(
    IGameRepository gameRepository,
    IGenreRepository genreRepository,
    IPlatformRepository platformRepository,
    IPublisherRepository publisherRepository) : ICoreRepositories
{
    public IGameRepository Games => gameRepository;

    public IGenreRepository Genres => genreRepository;

    public IPlatformRepository Platforms => platformRepository;

    public IPublisherRepository Publishers => publisherRepository;
}