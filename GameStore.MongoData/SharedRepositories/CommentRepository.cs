using GameStore.BLL.Interfaces.Repositories;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.MongoData.Interfaces;

namespace GameStore.MongoData.SharedRepositories;

public class CommentRepository(ICommentRepository innerRepository,
    IGameRepository gameRepository,
    IDatabaseMigrator migrator) : ICommentRepository
{
    public async Task AddCommentAsync(Comment comment, CancellationToken token)
    {
        var game = await gameRepository.GetGameByIdAsync(comment.GameId, token);

        if (game is not null)
        {
            await migrator.MigrateGameAsync(game, game, token);
        }
        
        await innerRepository.AddCommentAsync(comment, token);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByGameKeyAsync(string gameKey, CancellationToken token)
    {
        return await innerRepository.GetCommentsByGameKeyAsync(gameKey, token);
    }

    public async Task<Comment> GetCommentByIdAsync(Guid id, CancellationToken token)
    {
        return await innerRepository.GetCommentByIdAsync(id, token);
    }

    public void UpdateComment(Comment comment)
    {
        innerRepository.UpdateComment(comment);
    }
}