using AutoMapper;
using GameStore.BLL.AutoMapper;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.MapperTests;

public class GenreMappingProfileTests
{
    private MapperConfiguration _configuration;

    [Fact]
    public void GenreMappingProfile_ShouldHaveValidMappings()
    {
        // Arrange
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenreMappingProfile>();
        });

        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }
}