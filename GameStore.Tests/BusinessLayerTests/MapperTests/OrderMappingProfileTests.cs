using AutoMapper;
using GameStore.BLL.AutoMapper;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.MapperTests;

public class OrderMappingProfileTests
{
    private MapperConfiguration _configuration;

    [Fact]
    public void GameMappingProfile_ShouldHaveValidMappings()
    {
        // Arrange
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<OrderMapperProfile>();
        });

        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }
}