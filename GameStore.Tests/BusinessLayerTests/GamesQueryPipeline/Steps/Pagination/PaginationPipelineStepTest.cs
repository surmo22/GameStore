using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Pagination;
using GameStore.Domain.Entities.CoreEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Pagination;

public class PaginationPipelineStepTests
{
    [Fact]
    public void Paginate_WithValidPageCountAndPage_ReturnsCorrectSubset()
    {
        var request = new GameFilterRequest { PageCount = "2", Page = 2 };
        var step = new PaginationPipelineStep(request);

        var data = Enumerable.Range(1, 10).AsQueryable();
        var result = step.Paginate(data).ToList();

        Assert.Equal(new List<int> { 1, 2, 3, 4 }, result); // pageSize * page = 4 items (2*2)
    }

    [Fact]
    public void Paginate_WithPageCountAll_ReturnsAll()
    {
        var request = new GameFilterRequest { PageCount = "all", Page = 1 };
        var step = new PaginationPipelineStep(request);

        var data = Enumerable.Range(1, 5).AsQueryable();
        var result = step.Paginate(data).ToList();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Paginate_WithEmptyPageCount_ReturnsAll()
    {
        var request = new GameFilterRequest { PageCount = "", Page = 1 };
        var step = new PaginationPipelineStep(request);

        var data = Enumerable.Range(1, 3).AsQueryable();
        var result = step.Paginate(data).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void RepaginateGames_WithValidPageCountAndPage_ReturnsCorrectPage()
    {
        var request = new GameFilterRequest { PageCount = "3", Page = 2 };
        var step = new PaginationPipelineStep(request);

        var games = Enumerable.Range(1, 10)
            .Select(i => new Game { Name = $"Game {i}" });

        var result = step.RepaginateGames(games).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("Game 4", result[0].Name);
        Assert.Equal("Game 6", result[2].Name);
    }

    [Fact]
    public void RepaginateGames_WithAll_ReturnsAll()
    {
        var request = new GameFilterRequest { PageCount = "all", Page = 1 };
        var step = new PaginationPipelineStep(request);

        var games = Enumerable.Range(1, 4).Select(i => new Game { Name = $"Game {i}" });

        var result = step.RepaginateGames(games).ToList();

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void RepaginateGames_WithEmptyPageCount_ReturnsAll()
    {
        var request = new GameFilterRequest { PageCount = "", Page = 1 };
        var step = new PaginationPipelineStep(request);

        var games = Enumerable.Range(1, 3).Select(i => new Game { Name = $"Game {i}" });

        var result = step.RepaginateGames(games).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void RepaginateGames_Page1_ReturnsFirstPage()
    {
        var request = new GameFilterRequest { PageCount = "2", Page = 1 };
        var step = new PaginationPipelineStep(request);

        var games = Enumerable.Range(1, 5).Select(i => new Game { Name = $"Game {i}" });

        var result = step.RepaginateGames(games).ToList();

        Assert.Equal("Game 1", result[0].Name);
        Assert.Equal("Game 2", result[1].Name);
    }
}
