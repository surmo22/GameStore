using AutoMapper;
using GameStore.BLL.AutoMapper;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.MapperTests;

public class ShipperMappingProfileTests
{
    private MapperConfiguration _configuration;

    [Fact]
    public void PublisherMappingProfile_ShouldHaveValidMappings()
    {
        // Arrange
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ShipperMappingProfile>();
        });

        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }
    
}