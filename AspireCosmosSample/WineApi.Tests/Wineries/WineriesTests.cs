using System.Net;
using System.Net.Http.Json;
using WineApi.Dtos.Wineries;
using WineApi.Entities;

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

    [Fact]
    public async Task GetWinery_When_Winery_Exists_ShouldSucceed()
    {
        // Arrange
        var wineryName = Guid.NewGuid().ToString();
        var createWineryDto = new CreateWineryDto(wineryName);

        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync(Routes.Wineries.Create, createWineryDto);
        var wineryResponseDto =
            await createResponse.Content.ReadFromJsonAsync<CreateWineryResponseDto>();

        Assert.NotNull(wineryResponseDto);

        var wineryId = wineryResponseDto.Id;

        // Act
        var getResponse = await client.GetAsync(
            new Uri(Routes.Wineries.GetById(wineryId), UriKind.Relative)
        );

        var wineryDto = await getResponse.Content.ReadFromJsonAsync<WineryDto>();

        // Assert
        Assert.NotNull(wineryDto);
        Assert.Equal(wineryName, wineryDto.Name);
        Assert.StartsWith(EntityIdPrefixes.WineryPrefix, wineryDto.Id, StringComparison.Ordinal);
        Assert.True(wineryDto.CreatedAtUtc < DateTimeOffset.UtcNow);
        Assert.Null(wineryDto.UpdatedAtUtc);
    }

    [Fact]
    public async Task GetWinery_When_Winery_Does_Not_Exist_ShouldFail()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var getResponse = await client.GetAsync(
            new Uri(Routes.Wineries.GetById(Guid.NewGuid().ToString()), UriKind.Relative)
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
