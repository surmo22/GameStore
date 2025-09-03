using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.EntityServices;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests;

public class PublisherServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly PublisherService _publisherService;

    public PublisherServiceTests()
    {
        _guidProvider.Setup(g => g.NewGuid()).Returns(Guid.NewGuid());
        _publisherService = new PublisherService(_guidProvider.Object, _unitOfWorkMock.Object, _mapperMock.Object);
        Mock<IPublisherRepository> publisherRepositoryMock = new();
        _unitOfWorkMock.Setup(uow => uow.Publishers).Returns(publisherRepositoryMock.Object);
    }

    // Add tests here
    [Fact]
    public async Task CreatePublisherAsync_ShouldCreatePublisher()
    {
        // Arrange
        var publisherCreateDto = new PublisherCreateDto { CompanyName = "test1" };

        _unitOfWorkMock.Setup(uow =>
            uow.Publishers.AddPublisherAsync(It.IsAny<Publisher>(), It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(uow =>
            uow.CompleteAsync(It.IsAny<CancellationToken>()));
        _mapperMock.Setup(m =>
            m.Map<Publisher>(It.IsAny<object>())).Returns(new Publisher { Id = Guid.NewGuid(), CompanyName = "test1" });
        _mapperMock.Setup(m => m.Map<PublisherDto>(It.IsAny<object>())).Returns(new PublisherDto { CompanyName = "test1" });

        // Act
        var result = await _publisherService.CreatePublisherAsync(publisherCreateDto, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Publishers.AddPublisherAsync(It.IsAny<Publisher>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeletePublisherAsync_ShouldDeletePublisher()
    {
        // Arrange
        var publisherId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.Publishers.PublisherExistsAsync(publisherId, CancellationToken.None)).ReturnsAsync(true);

        // Act
        await _publisherService.DeletePublisherAsync(publisherId, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Publishers.DeletePublisherAsync(publisherId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetAllPublishersAsync_ShouldReturnAllPublishers()
    {
        // Arrange
        var publishers = new List<Publisher> { new(), new(), new() };
        var publishersDto = new List<PublisherDto> { new(), new(), new() };
        _unitOfWorkMock.Setup(uow => uow.Publishers.GetAllPublishersAsync(CancellationToken.None))
            .ReturnsAsync(publishers);
        _mapperMock.Setup(m => m.Map<IEnumerable<PublisherDto>>(publishers))
            .Returns(publishersDto);

        // Act
        var result = await _publisherService.GetAllPublishersAsync(CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetPublisherByIdAsync_ShouldReturnPublisher()
    {
        // Arrange
        var publisherId = Guid.NewGuid();
        var publisher = new Publisher { Id = publisherId };
        var publisherDto = new PublisherDto { Id = publisherId };

        _unitOfWorkMock.Setup(uow => uow.Publishers.GetPublisherByIdAsync(publisherId, CancellationToken.None))
            .ReturnsAsync(publisher);
        _mapperMock.Setup(m => m.Map<PublisherDto>(publisher))
            .Returns(publisherDto);

        // Act
        var result = await _publisherService.GetPublisherByIdAsync(publisherId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisherDto, result);
    }

    [Fact]
    public async Task GetPublisherByIdAsync_ThrowsKeyNotFoundException_WhenPublisherNotFound()
    {
        // Arrange
        var publisherId = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Publishers.GetPublisherByIdAsync(publisherId, CancellationToken.None))
            .ReturnsAsync((Publisher)null!);

        // Act && Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _publisherService.GetPublisherByIdAsync(publisherId, CancellationToken.None));
    }

    [Fact]
    public async Task UpdatePublisherAsync_ShouldUpdatePublisher()
    {
        // Arrange
        var publisherDto = new PublisherDto { Id = Guid.NewGuid() };
        var publisher = new Publisher { Id = publisherDto.Id };

        _mapperMock.Setup(m => m.Map<Publisher>(publisherDto)).Returns(publisher);
        _unitOfWorkMock.Setup(uow => uow.Publishers.PublisherExistsAsync(publisher.Id, CancellationToken.None)).ReturnsAsync(true);

        // Act
        await _publisherService.UpdatePublisherAsync(publisherDto, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Publishers.UpdatePublisher(publisher, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdatePublisherAsync_ThrowsKeyNotFoundException_WhenPublisherNotFound()
    {
        // Arrange
        var publisherDto = new PublisherDto { Id = Guid.NewGuid() };
        var publisher = new Publisher { Id = publisherDto.Id };

        _mapperMock.Setup(m => m.Map<Publisher>(publisherDto)).Returns(publisher);
        _unitOfWorkMock.Setup(uow => uow.Publishers.PublisherExistsAsync(publisher.Id, CancellationToken.None)).ReturnsAsync(false);

        // Act && Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _publisherService.UpdatePublisherAsync(publisherDto, CancellationToken.None));
    }

    [Fact]
    public async Task GetGamesByPublisherCompanyNameAsync_ShouldReturnGames()
    {
        // Arrange
        var companyName = "test";
        var games = new List<Game> { new(), new(), new() };
        var gameDtos = new List<GameDto> { new(), new(), new() };
        var publisher = new Publisher { CompanyName = companyName, Games = games, };

        _unitOfWorkMock.Setup(uow => uow.Publishers.GetPublisherByCompanyNameAsync(companyName, CancellationToken.None))
            .ReturnsAsync(publisher);
        _mapperMock.Setup(m => m.Map<IEnumerable<GameDto>>(games))
            .Returns(gameDtos);

        // Act
        var result = await _publisherService.GetGamesByPublisherCompanyNameAsync(companyName, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetGamesByPublisherIdAsync_ThrowsKeyNotFoundException_WhenPublisherNotFound()
    {
        // Arrange
        var companyName = "test";
        _unitOfWorkMock.Setup(uow => uow.Publishers.GetPublisherByCompanyNameAsync(companyName, CancellationToken.None))
            .ReturnsAsync((Publisher)null!);

        // Act && Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _publisherService.GetGamesByPublisherCompanyNameAsync(companyName, CancellationToken.None));
    }

    [Fact]
    public async Task GetPublisherByCompanyNameAsync_ShouldReturnPublisher()
    {
        // Arrange
        var companyName = "test";
        var publisher = new Publisher { CompanyName = companyName };
        var publisherDto = new PublisherDto { CompanyName = companyName };

        _unitOfWorkMock.Setup(uow => uow.Publishers.GetPublisherByCompanyNameAsync(companyName, CancellationToken.None))
            .ReturnsAsync(publisher);
        _mapperMock.Setup(m => m.Map<PublisherDto>(publisher))
            .Returns(publisherDto);

        // Act
        var result = await _publisherService.GetPublisherByCompanyNameAsync(companyName, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisherDto, result);
    }
    
    [Fact]
    public async Task GetPublisherByIdNotMappedAsync_ShouldReturnPublisher()
    {
        // Arrange
        var companyName = Guid.NewGuid();
        var publisher = new Publisher();

        _unitOfWorkMock.Setup(uow => uow.Publishers.GetPublisherByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(publisher);

        // Act
        var result = await _publisherService.GetPublisherByIdNotMappedAsync(companyName, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisher, result);
    }

    [Fact]
    public async Task GetPublisherByCompanyNameAsync_ThrowsKeyNotFoundException_WhenPublisherNotFound()
    {
        // Arrange
        var companyName = "test";
        _unitOfWorkMock.Setup(uow => uow.Publishers.GetPublisherByCompanyNameAsync(companyName, CancellationToken.None))
            .ReturnsAsync((Publisher)null!);

        // Act && Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _publisherService.GetPublisherByCompanyNameAsync(companyName, CancellationToken.None));
    }
}