using System.Net.Http.Json;
using GameStore.BLL.DTOs.Orders;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.DTOs.PaymentMethods;
using GameStore.Common.Constants;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using GameStore.Domain.Entities.UserEntities;
using GameStore.IntegrationTests.Helpers;
using GameStore.IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;

namespace GameStore.IntegrationTests.Tests;

public class OrdersTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;

    private readonly GameStoreContext _context;

    private static Guid _publisherId;

    private static Guid _gameId;

    private static Guid _userId;
    
    private static IServiceProvider _serviceProvider;

    public OrdersTests(DatabaseFixture dbFixture)
    {
        var factory = new GameStoreWebApplicationFactory(dbFixture);
        _client = factory.CreateClient();
        _context = dbFixture.GameStoreContext;
        if (_publisherId == Guid.Empty)
        {
            SetupPublisher();
        }

        if (_gameId == Guid.Empty)
        {
            SetupGame();
        }
        
        _userId = Authenticator.AuthenticateClientAsync(_client, factory.Services).GetAwaiter().GetResult();
        _serviceProvider = factory.Services;
    }

    [Fact]
    public async Task GetPaidAndCancelledOrders_ReturnsOrders()
    {
        // Arrange
        await _context.Orders.ExecuteDeleteAsync();
        AddOrder(_userId);
        
        // Act
        var response = await _client.GetAsync("orders/history");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        Assert.NotNull(orders);
        Assert.NotEmpty(orders);
    }

    [Fact]
    public async Task GetOrderById_ReturnsOrder()
    {
        // Arrange
        await _context.Orders.ExecuteDeleteAsync();
        var orderId = AddOrder(_userId);
        
        // Act
        var response = await _client.GetAsync($"orders/{orderId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(order);
        Assert.Equal(orderId, order.Id);
    }

    [Fact]
    public async Task GetOrderDetails_ReturnsOrderDetails()
    {
        // Arrange
        await _context.Orders.ExecuteDeleteAsync();
        var orderId = AddOrder(_userId);
        
        // Act
        var response = await _client.GetAsync($"orders/{orderId}/details");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var details = await response.Content.ReadFromJsonAsync<List<OrderGameDto>>();
        Assert.NotNull(details);
        Assert.NotEmpty(details);
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsPaymentMethods()
    {
        // Arrange
        await _context.PaymentMethods.AddRangeAsync([
            new()
            {
                Id = Guid.NewGuid(),
                Description = "Test Payment Method",
                ImageUrl = "Test Payment Method",
                Title = "test",
            }
        ]);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync("orders/payment-methods");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var paymentMethods = await response.Content.ReadFromJsonAsync<PaymentMethodList>();
        Assert.NotNull(paymentMethods);
        Assert.NotEmpty(paymentMethods.PaymentMethods);
    }

    [Fact]
    public async Task PayForOrder_PaysOrder()
    {
        // Arrange
        var user = new User()
        {
            UserName = "GetCart",
            Id = Guid.NewGuid(),
        };
        await Authenticator.SeedUserAsync(user, _client, _serviceProvider);
        
        AddOrder(user.Id, false);
        var paymentRequest = new PaymentRequest()
        {
            Method = "IBox terminal",
        };

        // Act
        var response = await _client.PostAsJsonAsync("orders/payment", paymentRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(response);
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

    private void SetupGame()
    {
        _gameId = Guid.NewGuid();
        _context.Games.Add(new Game
        {
            Id = _gameId,
            Name = "test",
            Description = "test",
            PublisherId = _publisherId,
            Key = "test",
            UnitInStock = 100,
        });
        _context.SaveChanges();
    }
    
    private Guid AddOrder(Guid userId, bool isPaid = true)
    {
        var orderId = Guid.NewGuid();
        
        _context.Orders.Add(new Order()
        {
            CustomerId = userId,
            Date = DateTime.Now,
            Id = orderId,
            Status = isPaid ? OrderStatuses.Paid : OrderStatuses.Open,
            Items = new List<OrderGame>()
            {
                new()
                {
                    OrderId = orderId,
                    ProductId = _gameId,
                    Price = 1,
                    Quantity = 1,
                }
            }
        });
        _context.SaveChanges();
        return orderId;
    }
}