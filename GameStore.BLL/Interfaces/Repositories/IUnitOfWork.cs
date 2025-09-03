namespace GameStore.BLL.Interfaces.Repositories;

public interface IUnitOfWork
{
    IGameRepository Games { get; }

    IGenreRepository Genres { get; }

    IPlatformRepository Platforms { get; }

    IPublisherRepository Publishers { get; }

    IOrderRepository Orders { get; }

    ICommentRepository Comments { get; }

    IUserBanRepository UserBans { get; }

    Task CompleteAsync(CancellationToken cancellationToken);
}
