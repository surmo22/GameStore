using GameStore.BLL.Interfaces.CommentServices;
using GameStore.BLL.Services.CommentServices;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.CommentServices;

public class CommentActionServiceTests
{
    private readonly Mock<ICommentActionStrategy> _mockStrategy;
    private readonly CommentActionService _commentActionService;

    public CommentActionServiceTests()
    {
        Mock<ILogger<CommentActionService>> mockLogger = new Mock<ILogger<CommentActionService>>();
        _mockStrategy = new Mock<ICommentActionStrategy>();
        var strategies = new List<ICommentActionStrategy> { _mockStrategy.Object };
        _commentActionService = new CommentActionService(strategies, mockLogger.Object);
    }

    [Fact]
    public async Task ApplyActionToCommentAsync_CommentWithoutParent_DoesNotApplyAction()
    {
        // Arrange
        var comment = new Comment { Id = Guid.NewGuid(), ParentId = null };
        var action = "SomeAction";
        var cancellationToken = CancellationToken.None;

        // Act
        await _commentActionService.ApplyActionToCommentAsync(comment, action, cancellationToken);

        // Assert
        _mockStrategy.Verify(s => s.CanExecute(It.IsAny<string>()), Times.Never);
        _mockStrategy.Verify(s => s.ExecuteAsync(It.IsAny<string>(), It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApplyActionToCommentAsync_ValidActionWithStrategy_ExecutesStrategy()
    {
        // Arrange
        var comment = new Comment { Id = Guid.NewGuid(), ParentId = Guid.NewGuid() };
        var action = "ValidAction";
        var cancellationToken = CancellationToken.None;

        _mockStrategy.Setup(s => s.CanExecute(action)).Returns(true);

        // Act
        await _commentActionService.ApplyActionToCommentAsync(comment, action, cancellationToken);

        // Assert
        _mockStrategy.Verify(s => s.ExecuteAsync(action, comment, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ApplyActionToCommentAsync_InvalidAction_LogsWarning()
    {
        // Arrange
        var comment = new Comment { Id = Guid.NewGuid(), ParentId = Guid.NewGuid() };
        var action = "InvalidAction";
        var cancellationToken = CancellationToken.None;

        _mockStrategy.Setup(s => s.CanExecute(action)).Returns(false);

        // Act
        await _commentActionService.ApplyActionToCommentAsync(comment, action, cancellationToken);

        // Assert
        _mockStrategy.Verify(s => s.ExecuteAsync(It.IsAny<string>(), It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}