using System.Net.Http.Json;
using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.DTOs.Publisher;
using GameStore.Common.Constants;
using GameStore.Common.Utils;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.UserEntities;
using GameStore.Domain.MongoEntities;
using GameStore.IntegrationTests.Helpers;
using GameStore.IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;

namespace GameStore.IntegrationTests.Tests;

public class GamesTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;

    private readonly GameStoreContext _context;
    
    private readonly NorthwindMongoContext _mongoContext;

    private static Guid _publisherId;
    
    private static Guid _genreId;
    
    private static Guid _platformId;
    
    public GamesTests(DatabaseFixture fixture)
    {
        var factory = new GameStoreWebApplicationFactory(fixture);
        _client = factory.CreateClient();
        _context = fixture.GameStoreContext;
        _mongoContext = fixture.NorthwindContext;
        if (_publisherId == Guid.Empty)
        {
            SetupPublisher();
        }

        if (_genreId == Guid.Empty)
        {
            SetupGenre();
        }

        if (_platformId == Guid.Empty)
        {
            SetupPlatform();
        }

        Authenticator.AuthenticateClientAsync(_client, factory.Services).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task CreateGame_CreatesGame()
    {
        // Arrange
        await _mongoContext.Genres.InsertOneAsync(new MongoGenre()
        {
            CategoryId = 11,
            CategoryName = "Giga test mongo",
            Description = "Giga test mongo",
            Picture = "foo",
        });
        var gameCreateRequest = new GameCreateRequestDto()
        {
            Game = new GameCreateDto()
            {
                Key = "testgame",
                Name = "testgame",
                Description = "testgame",
                Price = 10,
                UnitInStock = 100,
                Discount = 0,
            },

            Genres = [_genreId, IntToGuidConverter.Convert(11)],
            Platforms = [_platformId],
            PublisherId = _publisherId,
            Image = "",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("games", gameCreateRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var game = await response.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(game);
    }

    [Fact]
    public async Task DeleteGame_DeletesGame()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        _context.Games.Add(new Game
        {
            Id = gameId,
            Key = "testgame1",
            Name = "testgame1",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.DeleteAsync($"games/testgame1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.IsDeleted));
    }
    
    [Fact]
    public async Task DeleteGame_DeletesGameFromMongo()
    {
        // Arrange
        await _mongoContext.Publishers.InsertOneAsync(new MongoPublisher()
        {
            SupplierId = 1111,
            ContactName = "test222",
            CompanyName = "best company inttheworld2"
        });
        await _mongoContext.Games.InsertOneAsync(new MongoGame()
        {
            ProductKey = "DeleteGame_DeletesGameFromMongo1",
            ProductName = "DeleteGame_DeletesGameFromMong1o",
            ProductId = 125,
            SupplierId = 1111,
        });
        
        // Act
        var response = await _client.DeleteAsync($"games/DeleteGame_DeletesGameFromMongo1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(await _context.Games.FirstOrDefaultAsync(g => g.Key == "DeleteGame_DeletesGameFromMongo1" && g.IsDeleted));
    }

    [Fact]
    public async Task GetAllGames_ReturnsAllGames()
    {
        // Arrange
        _context.Games.Add(new Game
        {
            Id = Guid.NewGuid(),
            Key = "Getalll",
            Name = "Getalll",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync("games?minPrice=0&maxPrice=100&pageCount=10&page=1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<PaginatedList<GameDto>>();
        Assert.NotNull(games);
        Assert.NotEmpty(games.Games);
    }
    
    [Fact]
    public async Task GetAllGamesWithFilter_ReturnsAllGames()
    {
        // Arrange
        var platfromId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        _context.Games.Add(new Game
        {
            Id = Guid.NewGuid(),
            Key = "GetalllWithFilter",
            Name = "GetalllWithFilter",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
            Platforms = new List<Platform>()
            {
                new()
                {
                    Id = platfromId,
                    Type = "getallwithfilters",
                }
            },
            Genres = new List<Genre>()
            {
                new()
                {
                    Id = genreId,
                    Name = "getallwithfilters",
                }
            },
            CreationDate = DateTime.Now,
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync(
                $"games?name=Getalll&minPrice=0" +
                      $"&maxPrice=100&platforms={platfromId}&genres={genreId}" +
                      $"&publisher={_publisherId}" +
                      $"&DatePublishing=Last Week" +
                      $"&page=1&pageCount=10" +
                      $"&Sort=New");

        
        // Assert
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<PaginatedList<GameDto>>();
        Assert.NotNull(games);
        Assert.NotEmpty(games.Games);
        Assert.Contains(games.Games, g => g.Name == "GetalllWithFilter");
    }

    [Fact]
    public async Task GetGameByKey_ReturnsGame()
    {
        // Arrange
        await _mongoContext.Publishers.InsertOneAsync(new MongoPublisher()
        {
            SupplierId = 1,
            ContactName = "test22",
        });
        await _mongoContext.Games.InsertOneAsync(new MongoGame()
        {
            ProductKey = "getgamebykey",
            ProductName = "getgamebykey",
            ProductId = 11,
            SupplierId = 1,
        });
        
        // Act
        var response = await _client.GetAsync("games/getgamebykey");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(games);
    }
    
    [Fact]
    public async Task GetGameByKey_ReturnsGameFromMongo()
    {
        // Arrange
        _context.Games.Add(new Game
        {
            Id = Guid.NewGuid(),
            Key = "GetGameByKeyReturnsGame",
            Name = "GetGameByKeyReturnsGame",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync("games/GetGameByKeyReturnsGame");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(games);
    }

    [Fact]
    public async Task GetGenresByGameKey_ReturnsGenres()
    {
        // Arrange
        _context.Games.Add(new Game()
        {
            Id = Guid.NewGuid(),
            Key = "GetGenresByGameKeyReturnsGenres",
            Name = "GetGenresByGameKeyReturnsGenres",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
            Genres = new List<Genre>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "testgenre12345325",
                }
            }
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"games/GetGenresByGameKeyReturnsGenres/genres");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        Assert.NotNull(genres);
        Assert.NotEmpty(genres);
    }
    
    [Fact]
    public async Task GetPublisherByGameKey_ReturnsPublisher()
    {
        // Arrange
        _context.Games.Add(new Game
        {
            Id = Guid.NewGuid(),
            Key = "GetPublisherByGameKey_ReturnsPublisher",
            Name = "GetPublisherByGameKey_ReturnsPublisher",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"games/GetPublisherByGameKey_ReturnsPublisher/publisher");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var publisher = await response.Content.ReadFromJsonAsync<PublisherDto>();
        Assert.NotNull(publisher);
    }

    [Fact]
    public async Task GetPlatformsByGameKey_ReturnsPlatforms()
    {
        // Arrange
        _context.Games.Add(new Game
        {
            Id = Guid.NewGuid(),
            Key = "key123456",
            Name = "GetPlatformsByGameKeyReturnsPlatforms",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
            Platforms = new List<Platform>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Type = "testplatform",
                }
            }
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"games/key123456/platforms");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var platforms = await response.Content.ReadFromJsonAsync<List<PlatformDto>>();
        Assert.NotNull(platforms);
        Assert.NotEmpty(platforms);
    }

    [Fact]
    public async Task DownloadGame_ReturnsGameFile()
    {
        // Arrange
        var gameKey = _context.Games.FirstOrDefault().Key;
        
        // Act
        var response = await _client.GetAsync($"games/{gameKey}/file");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var publisher = await response.Content.ReadAsStreamAsync();
        Assert.NotNull(publisher);
    }
    
    [Fact]
    public async Task GetGameById_ReturnsGame()
    {
        // Arrange
        var gameId = _context.Games.FirstOrDefault().Id;
        
        // Act
        var response = await _client.GetAsync($"games/find/{gameId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var publisher = await response.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(publisher);
    }

    [Fact]
    public async Task UpdateGame_UpdatesGame()
    {
        // Arrange
        var game = _context.Games.FirstOrDefault();
        game.Name = "Updated new game";
        var gameUpdateRequestDto = new GameUpdateRequestDto()
        {
            Game = new()
            {
                Id = game.Id,
                Key = "Updated new game",
                Name = "Updated new game",
                Description = "Updated new game",
                Price = 10,
                UnitInStock = 100,
            },
            Genres = [_genreId],
            Platforms = [_platformId],
            PublisherId = _publisherId,
            Image = ""
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"games/", gameUpdateRequestDto);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Games.FirstOrDefault(g => g.Name == "Updated new game"));
    }

    [Fact]
    public async Task BuyGameAdds_GameToCart()
    {
        // Arrange
        var game = _context.Games.FirstOrDefault();
        
        // Act
        var response = await _client.PostAsync($"games/{game.Key}/buy", null!);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Items.Any(x => x.ProductId == game.Id)));

    }

    [Fact]
    public async Task AddComment_AddsCommentToGame()
    {
        // Arrange
        var game = _context.Games.FirstOrDefault();
        var addCommentRequest = new AddCommentRequestDto()
        {
            Action = "",
            AddComment = new()
            {
                Body = "teest",
            }
        };
        
        // Act
        var response = await _client.PostAsJsonAsync($"games/{game.Key}/comments", addCommentRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Comments.FirstOrDefaultAsync(c => c.Body == "teest"));
    }

    [Fact]
    public async Task GetCommentsByGameKey_ReturnsComments()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        _context.Games.Add(new Game()
        {
            Id = gameId,
            Key = "GetCommentsByGameKeyReturnsComments",
            Name = "GetCommentsByGameKeyReturnsComments",
            Description = "testgame1",
            Price = 10,
            UnitInStock = 100,
            PublisherId = _publisherId,
        });
        _context.Comments.Add(new Comment()
        {
            Body = "teest",
            GameId = gameId,
            Id = Guid.NewGuid(),
            User= new User
            {
                UserName = "test"
            }
        });
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"games/GetCommentsByGameKeyReturnsComments/comments");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var comments = await response.Content.ReadFromJsonAsync<List<AddCommentDto>>();
        Assert.NotNull(comments);
        Assert.NotEmpty(comments);
    }
    
    [Fact]
    public async Task DeleteComment_DeletesComment()
    {
        // Arrange
        var game = _context.Games.FirstOrDefault();
        var commentId = Guid.NewGuid();
        var commentToInsert = new Comment()
        {
            Body = "teest",
            GameId = game.Id,
            Id = commentId,
            User= new User
            {
                UserName = "testuser"
            }
        };
        _context.Comments.Add(commentToInsert);
        await _context.SaveChangesAsync();
        _context.Entry(commentToInsert).State = EntityState.Detached;
        // Act
        var response = await _client.DeleteAsync($"games/{game.Key}/comments/{commentId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        Assert.NotNull(comment);
        Assert.Contains(CommentConstants.Deleted, comment.Body);
    }
    
    private void SetupPublisher()
    {
        _publisherId = Guid.NewGuid();
        _context.Publishers.Add(new Publisher()
        {
            Id = _publisherId,
            CompanyName = "very cool name",
        });
        _context.SaveChanges();
    }

    private void SetupGenre()
    {
        _genreId = Guid.NewGuid();
        _context.Genres.Add(new Genre()
        {
            Id = _genreId,
            Name = "my genre",
        });
        _context.SaveChanges();
    }

    private void SetupPlatform()
    {
        _platformId = Guid.NewGuid();
        _context.Platforms.Add(new Platform()
        {
            Id = _platformId,
            Type = "test",
        });
        _context.SaveChanges();
    }
}