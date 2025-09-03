using AutoMapper;
using GameStore.BLL.AutoMapper;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.MapperTests;

public class UserMappingProfileTests
{
    private MapperConfiguration _configuration;

    [Fact]
    public void PublisherMappingProfile_ShouldHaveValidMappings()
    {
        // Arrange
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
        });

        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }
}