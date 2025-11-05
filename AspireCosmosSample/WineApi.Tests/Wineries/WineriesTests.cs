using System.Net;
using System.Net.Http.Json;
using WineApi.Dtos.Wineries;

[assembly: CLSCompliant(false)]

namespace WineApi.Tests.Wineries;

public sealed class WineriesTests : IClassFixture<WineApiWebApplicationFactory>
{
    private readonly WineApiWebApplicationFactory _factory;

    public WineriesTests(WineApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateWinery_When_Given_Valid_Parameters_ShouldSucceed()
    {
        // Arrange
        var wineryName = Guid.NewGuid().ToString();
        var createWineryDto = new CreateWineryDto(wineryName);

        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(Routes.Wineries.Create, createWineryDto);
        var wineryResponseDto = await response.Content.ReadFromJsonAsync<CreateWineryResponseDto>();

        // Assert
        Assert.NotNull(wineryResponseDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.EndsWith(
            $"{Routes.Wineries.Create}/{wineryResponseDto.Id}",
            response.Headers.Location!.ToString(),
            StringComparison.OrdinalIgnoreCase
        );
    }

    [Fact]
    public async Task CreateWinery_When_Name_Is_Duplicate_ShouldFail()
    {
        // Arrange
        var wineryName = Guid.NewGuid().ToString();
        var createWineryDto = new CreateWineryDto(wineryName);

        var client = _factory.CreateClient();
        await client.PostAsJsonAsync(Routes.Wineries.Create, createWineryDto);

        // Act
        var response = await client.PostAsJsonAsync(Routes.Wineries.Create, createWineryDto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
