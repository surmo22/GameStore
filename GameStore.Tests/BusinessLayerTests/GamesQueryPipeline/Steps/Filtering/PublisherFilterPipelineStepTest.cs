using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Filtering;

public class PublisherFilterPipelineStepTests
{
    [Fact]
    public void Process_GameQuery_WithValidPublisher_FiltersCorrectly()
    {
        var publisherId = Guid.NewGuid();
        var filterRequest = new GameFilterRequest { Publisher = publisherId };
        var step = new PublisherFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { PublisherId = publisherId },
            new() { PublisherId = Guid.NewGuid() }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
        Assert.Equal(publisherId, result[0].PublisherId);
    }

    [Fact]
    public void Process_GameQuery_EmptyPublisher_ReturnsUnfiltered()
    {
        var filterRequest = new GameFilterRequest { Publisher = Guid.Empty };
        var step = new PublisherFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { PublisherId = Guid.NewGuid() }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
    }

    [Fact]
    public void Process_GameQuery_NullPublisher_ReturnsUnfiltered()
    {
        var filterRequest = new GameFilterRequest { Publisher = null };
        var step = new PublisherFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { PublisherId = Guid.NewGuid() }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
    }

    [Fact]
    public void Process_MongoGameQuery_WithValidPublisher_FiltersCorrectly()
    {
        var publisherId = 1;
        var publisherGuid = IntToGuidConverter.Convert(publisherId);
        var filterRequest = new GameFilterRequest { Publisher = publisherGuid };
        var step = new PublisherFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { SupplierId = publisherId },
            new() { SupplierId = 12 }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Single(result);
        Assert.Equal(publisherId, result[0].SupplierId);
    }

    [Fact]
    public void Process_MongoGameQuery_NullPublisher_ReturnsUnfiltered()
    {
        var filterRequest = new GameFilterRequest { Publisher = null };
        var step = new PublisherFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { SupplierId = 1 }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Single(result);
    }

    [Fact]
    public void Process_MongoGameQuery_EmptyPublisher_ReturnsUnfiltered()
    {
        var filterRequest = new GameFilterRequest { Publisher = Guid.Empty };
        var step = new PublisherFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { SupplierId = 1 }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Single(result);
    }
}
