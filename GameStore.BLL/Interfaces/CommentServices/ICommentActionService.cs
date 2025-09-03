using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.CommentServices;

public interface ICommentActionService
{
    Task ApplyActionToCommentAsync(Comment comment, string action, CancellationToken token);
}