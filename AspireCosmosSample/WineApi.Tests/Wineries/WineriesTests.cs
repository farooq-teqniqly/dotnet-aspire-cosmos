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
        var createWineryDto = new CreateWineryDto { Name = "DeLille Cellars" };

        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(Routes.Wineries.Create, createWineryDto);
        var wineryDto = await response.Content.ReadFromJsonAsync<WineryDto>();

        // Assert
        Assert.NotNull(wineryDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.EndsWith(
            $"{Routes.Wineries.Create}/{wineryDto.Id}",
            response.Headers.Location!.ToString(),
            StringComparison.OrdinalIgnoreCase
        );
    }
}
