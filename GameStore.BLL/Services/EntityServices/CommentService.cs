using AutoMapper;
using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.CommentServices;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Utils;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.EntityServices;

public class CommentService(
    IUnitOfWork             unitOfWork,
    ICommentActionService   commentActionService,
    IMapper                 mapper,
    ILogger<CommentService> logger,
    IGuidProvider           guidProvider) : ICommentService
{
    public async Task AddCommentAsync(Guid user, AddCommentRequestDto addCommentRequestDto, string gameKey,
        CancellationToken cancellationToken)
    {
        var comment = mapper.Map<Comment>(addCommentRequestDto);
        var game = await unitOfWork.Games.GetGameByKeyAsync(gameKey, cancellationToken) ?? throw new KeyNotFoundException($"Game with key {gameKey} was not found.");
        
        comment.Id = guidProvider.NewGuid();
        comment.GameId = game.Id;
        comment.UserId = user;

        await commentActionService.ApplyActionToCommentAsync(comment, addCommentRequestDto.Action, cancellationToken);

        await unitOfWork.Comments.AddCommentAsync(comment, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        logger.LogInformation($"AddComment with id {comment.Id} was added.");
    }

    public async Task<IEnumerable<GetCommentDto>> GetCommentsByGameKeyAsync(string gameKey, CancellationToken cancellationToken)
    {
        var comments = await unitOfWork.Comments.GetCommentsByGameKeyAsync(gameKey, cancellationToken);
        return mapper.Map<IEnumerable<GetCommentDto>>(comments);
    }

    public async Task<GetCommentDto> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var comment = await unitOfWork.Comments.GetCommentByIdAsync(id, cancellationToken);
        return mapper.Map<GetCommentDto>(comment);
    }

    public async Task DeleteCommentAsync(Guid id, CancellationToken cancellationToken)
    {
        var comment = await unitOfWork.Comments.GetCommentByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Comment with id {id} was not found.");

        // Recursively update the content of the comment and its child comments
        DeleteQuoteFromChildComments(comment, QuoteFormatter.FormatQuote(comment.Name, comment.Body));

        comment.Body = CommentConstants.Deleted;
        unitOfWork.Comments.UpdateComment(comment);
        await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogInformation($"Comment with id {comment.Id} were deleted.");
    }

    private static void DeleteQuoteFromChildComments(Comment comment, string quote)
    {
        // Remove from body quote of parent comments
        comment.Body = comment.Body.Replace(
            quote,
            CommentConstants.Deleted);

        // go recursively through child comments
        foreach (var childComment in comment.ChildComments)
        {
            DeleteQuoteFromChildComments(childComment, quote);
        }
    }
}