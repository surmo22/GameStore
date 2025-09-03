using AutoMapper;
using GameStore.BLL.AutoMapper;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.MapperTests;

public class PublisherMappingProfileTests
{
    private MapperConfiguration _configuration;

    [Fact]
    public void PublisherMappingProfile_ShouldHaveValidMappings()
    {
        // Arrange
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PublisherMappingProfile>();
        });

        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }
}