using GameStore.BLL.Interfaces.CommentServices;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.CommentServices;

public class CommentActionService(IEnumerable<ICommentActionStrategy> strategies, ILogger<CommentActionService> logger) : ICommentActionService
{
    public async Task ApplyActionToCommentAsync(Comment comment, string action, CancellationToken token)
    {
        if (comment.ParentId is null)
        {
            return;
        }

        var strategy = strategies.FirstOrDefault(s => s.CanExecute(action));
        if (strategy is not null)
        {
            await strategy.ExecuteAsync(action, comment, token);
            return;
        }

        logger.LogWarning($"Invalid action for comment with id {comment.Id} was provided, action: {action}");
    }
}