using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.CommentServices;

public interface ICommentActionStrategy
{
    public Task ExecuteAsync(string action, Comment newComment, CancellationToken cancellationToken);

    bool CanExecute(string action);
}
