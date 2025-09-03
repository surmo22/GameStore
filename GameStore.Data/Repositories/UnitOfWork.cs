using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;

namespace GameStore.Data.Repositories;

public class UnitOfWork(
    GameStoreContext context,
    ICoreRepositories coreRepositories,
    IOrderRepository orderRepository,
    ICommentRepository commentRepository,
    IUserBanRepository userBanRepository) : IUnitOfWork
{
    public IGameRepository Games => coreRepositories.Games;

    public IGenreRepository Genres => coreRepositories.Genres;

    public IPlatformRepository Platforms => coreRepositories.Platforms;

    public IPublisherRepository Publishers => coreRepositories.Publishers;

    public IOrderRepository Orders => orderRepository;

    public ICommentRepository Comments => commentRepository;

    public IUserBanRepository UserBans => userBanRepository;

    public async Task CompleteAsync(CancellationToken cancellationToken)
    {
        var debugView = context.ChangeTracker.DebugView.ShortView;
        Console.WriteLine(debugView);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}