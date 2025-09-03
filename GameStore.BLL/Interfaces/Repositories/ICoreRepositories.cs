namespace GameStore.BLL.Interfaces.Repositories;

public interface ICoreRepositories
{
    IGameRepository Games { get; }

    IGenreRepository Genres { get; }

    IPlatformRepository Platforms { get; }

    IPublisherRepository Publishers { get; }
}