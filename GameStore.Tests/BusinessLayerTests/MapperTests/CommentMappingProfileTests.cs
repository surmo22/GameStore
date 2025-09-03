using AutoMapper;
using GameStore.BLL.AutoMapper;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.MapperTests;

public class CommentMappingProfileTests
{
    private MapperConfiguration _configuration;

    [Fact]
    public void GameMappingProfile_ShouldHaveValidMappings()
    {
        // Arrange
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CommentMappingProfile>();
        });

        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }
}