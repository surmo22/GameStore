using GameStore.BLL.Interfaces.CommentServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.CommentServices.CommentSetupStrategies;

public class ReplyActionStrategy(IUnitOfWork unitOfWork) : ICommentActionStrategy
{
    public async Task ExecuteAsync(string action, Comment newComment, CancellationToken cancellationToken)
    {
        if (action == CommentConstants.Reply && newComment.ParentId.HasValue)
        {
            var parentComment = await unitOfWork.Comments.GetCommentByIdAsync((Guid)newComment.ParentId, cancellationToken)
                                ?? throw new KeyNotFoundException($"Comment with id {newComment.ParentId} is not found");
            newComment.Body = parentComment.Name + ", " + newComment.Body;
        }
    }

    public bool CanExecute(string action)
    {
        return action == CommentConstants.Reply;
    }
}