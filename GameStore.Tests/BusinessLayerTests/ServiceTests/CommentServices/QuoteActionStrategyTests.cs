using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.CommentServices.CommentSetupStrategies;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.UserEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.CommentServices;

public class QuoteActionStrategyTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly QuoteActionStrategy _strategy;

    public QuoteActionStrategyTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _strategy = new QuoteActionStrategy(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReplyActionWithParentId_ShouldFormatCommentBodyWithQuote()
    {
        // Arrange
        var parentComment = new Comment
        {
            Id = Guid.NewGuid(),
            User = new User(){ UserName = "Parent Commenter"},
            Body = "Parent comment text.",
        };
        var newComment = new Comment
        {
            ParentId = parentComment.Id,
            Body = "New comment text.",
        };
        _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(parentComment);

        // Act
        await _strategy.ExecuteAsync(CommentConstants.Reply, newComment, CancellationToken.None);

        // Assert
        var expectedBody = $"Parent Commenter said: \"Parent comment text.\", New comment text.";
        Assert.Equal(expectedBody, newComment.Body);
    }

    [Fact]
    public async Task ExecuteAsync_ReplyActionWithNonExistentParentComment_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var newComment = new Comment
        {
            ParentId = Guid.NewGuid(),
            Body = "New comment text.",
        };
        _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment)null!); // Simulate a not found comment.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _strategy.ExecuteAsync(CommentConstants.Reply, newComment, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_NonReplyAction_ShouldNotModifyCommentBody()
    {
        // Arrange
        var newComment = new Comment
        {
            ParentId = Guid.NewGuid(),
            Body = "Original comment body.",
        };

        // Act
        await _strategy.ExecuteAsync("NonReplyAction", newComment, CancellationToken.None);

        // Assert
        Assert.Equal("Original comment body.", newComment.Body);
    }

    [Fact]
    public void CanExecute_QuoteAction_ShouldReturnTrue()
    {
        // Act
        var result = _strategy.CanExecute(CommentConstants.Quote);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_NonQuoteAction_ShouldReturnFalse()
    {
        // Act
        var result = _strategy.CanExecute("SomeOtherAction");

        // Assert
        Assert.False(result);
    }
}
