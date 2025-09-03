using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment, CancellationToken token);

    Task<IEnumerable<Comment>> GetCommentsByGameKeyAsync(string gameKey, CancellationToken token);

    Task<Comment> GetCommentByIdAsync(Guid id, CancellationToken token);

    void UpdateComment(Comment comment);
}