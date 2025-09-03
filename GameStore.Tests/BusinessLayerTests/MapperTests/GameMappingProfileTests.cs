using AutoMapper;
using GameStore.BLL.AutoMapper;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.MapperTests;

public class GameMappingProfileTests
{
    private MapperConfiguration _configuration;

    [Fact]
    public void GameMappingProfile_ShouldHaveValidMappings()
    {
        // Arrange
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GameMappingProfile>();
        });

        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }
}
