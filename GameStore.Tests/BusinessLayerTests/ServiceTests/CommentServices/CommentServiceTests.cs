using AutoMapper;
using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.CommentServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.EntityServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.CommentServices;

public class CommentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICommentActionService> _mockCommentActionService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
        _mockUnitOfWork = new();
        _mockCommentActionService = new();
        _mockMapper = new();
        Mock<ILogger<CommentService>> mockLogger = new();
        _commentService = new(
            _mockUnitOfWork.Object,
            _mockCommentActionService.Object,
            _mockMapper.Object,
            mockLogger.Object,
            _guidProvider.Object);
    }

    // Test methods will go here
    [Fact]
    public async Task AddCommentAsync_ValidRequest_AddsComment()
    {
        // Arrange
        var addCommentRequestDto = new AddCommentRequestDto
        {
            // Initialize properties as needed
        };
        var gameKey = "valid-game-key";
        var cancellationToken = CancellationToken.None;

        var game = new Game { Id = Guid.NewGuid(), Key = gameKey };
        var comment = new Comment { Id = Guid.NewGuid(), GameId = game.Id };

        _mockUnitOfWork.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, cancellationToken))
            .ReturnsAsync(game);
        _mockMapper.Setup(m => m.Map<Comment>(addCommentRequestDto))
            .Returns(comment);
        _mockCommentActionService.Setup(cas => cas.ApplyActionToCommentAsync(comment, addCommentRequestDto.Action, cancellationToken))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.Comments.AddCommentAsync(comment, cancellationToken))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.CompleteAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _commentService.AddCommentAsync(Guid.NewGuid(), addCommentRequestDto, gameKey, cancellationToken);

        // Assert
        _mockUnitOfWork.Verify(uow => uow.Games.GetGameByKeyAsync(gameKey, cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<Comment>(addCommentRequestDto), Times.Once);
        _mockCommentActionService.Verify(cas => cas.ApplyActionToCommentAsync(comment, addCommentRequestDto.Action, cancellationToken), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.Comments.AddCommentAsync(comment, cancellationToken), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_GameNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var addCommentRequestDto = new AddCommentRequestDto
        {
            // Initialize properties as needed
        };
        var gameKey = "invalid-game-key";
        var cancellationToken = CancellationToken.None;

        _mockUnitOfWork.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, cancellationToken))
            .ReturnsAsync((Game)null!);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _commentService.AddCommentAsync(Guid.NewGuid(), addCommentRequestDto, gameKey, cancellationToken));

        _mockUnitOfWork.Verify(uow => uow.Games.GetGameByKeyAsync(gameKey, cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<Comment>(It.IsAny<AddCommentRequestDto>()), Times.Once);
        _mockCommentActionService.Verify(cas => cas.ApplyActionToCommentAsync(It.IsAny<Comment>(), It.IsAny<string>(), cancellationToken), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.Comments.AddCommentAsync(It.IsAny<Comment>(), cancellationToken), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task GetCommentsByGameKeyAsync_ReturnsMappedComments()
    {
        // Arrange
        var gameKey = "test-game-key";
        var cancellationToken = CancellationToken.None;

        var comments = new List<Comment>
        {
            new() { Id = Guid.NewGuid(), Body = "Comment 1" },
            new() { Id = Guid.NewGuid(), Body = "Comment 2" },
        };

        var commentDtos = new List<GetCommentDto>
        {
            new() { Id = comments[0].Id, Body = "Comment 1" },
            new() { Id = comments[1].Id, Body = "Comment 2" },
        };

        _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentsByGameKeyAsync(gameKey, cancellationToken))
            .ReturnsAsync(comments);

        _mockMapper.Setup(m => m.Map<IEnumerable<GetCommentDto>>(comments))
            .Returns(commentDtos);

        // Act
        var result = (await _commentService.GetCommentsByGameKeyAsync(gameKey, cancellationToken)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Comment 1", result.First().Body);
        Assert.Equal("Comment 2", result.Last().Body);
    }

    [Fact]
    public async Task GetCommentByIdAsync_ReturnsMappedComment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var comment = new Comment { Id = commentId, Body = "Test Comment" };
        var commentDto = new GetCommentDto { Id = commentId, Body = "Test Comment" };

        _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(commentId, cancellationToken))
            .ReturnsAsync(comment);

        _mockMapper.Setup(m => m.Map<GetCommentDto>(comment))
            .Returns(commentDto);

        // Act
        var result = await _commentService.GetCommentByIdAsync(commentId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.Equal("Test Comment", result.Body);
    }

    [Fact]
    public async Task DeleteCommentAsync_DeletesCommentAndLogsInformation()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var comment = new Comment
        {
            Id = commentId,
            Body = "Test Comment",
            User = new User(){ UserName = "test1"},
            ChildComments =
            [
                new()
                {
                    Id = Guid.NewGuid(), Body = "Test Comment 2", User = new User{ UserName = "test"}, ChildComments = [],
                }
            ],
        };

        _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(commentId, cancellationToken))
            .ReturnsAsync(comment);

        // Act
        await _commentService.DeleteCommentAsync(commentId, cancellationToken);

        // Assert
        Assert.Equal(CommentConstants.Deleted, comment.Body);
        _mockUnitOfWork.Verify(uow => uow.Comments.UpdateComment(comment), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_CommentNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(commentId, cancellationToken))
            .ReturnsAsync((Comment)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _commentService.DeleteCommentAsync(commentId, cancellationToken));
        Assert.Equal($"Comment with id {commentId} was not found.", exception.Message);
    }
}