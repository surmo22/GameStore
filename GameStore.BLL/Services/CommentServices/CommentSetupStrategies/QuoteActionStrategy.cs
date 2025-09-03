using GameStore.BLL.Interfaces.CommentServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Utils;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.CommentServices.CommentSetupStrategies;

public class QuoteActionStrategy(IUnitOfWork unitOfWork) : ICommentActionStrategy
{
    public async Task ExecuteAsync(string action, Comment newComment, CancellationToken cancellationToken)
    {
        if (action == CommentConstants.Reply && newComment.ParentId.HasValue)
        {
            var parentComment = await unitOfWork.Comments.GetCommentByIdAsync((Guid)newComment.ParentId, cancellationToken)
                                ?? throw new KeyNotFoundException($"Comment with id {newComment.ParentId} is not found");
            newComment.Body = QuoteFormatter.FormatQuote(parentComment.Name, parentComment.Body) + ", " + newComment.Body;
        }
    }

    public bool CanExecute(string action)
    {
        return action == CommentConstants.Quote;
    }
}