using System.Net;
using System.Net.Http.Json;
using WineApi.Dtos.Wineries;

namespace WineApi.Tests.Wineries
{
    public sealed class CreateWineryValidationTests : IClassFixture<WineApiWebApplicationFactory>
    {
        private readonly WineApiWebApplicationFactory _factory;

        public CreateWineryValidationTests(WineApiWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public static TheoryData<string> InvalidNameLengthsAboveMaximum()
        {
            var data = new TheoryData<string>();

            // Test lengths from 257 to 260 characters (above maximum of 256)
            for (var length = 257; length <= 260; length++)
            {
                data.Add(new string('A', length));
            }

            return data;
        }

        public static TheoryData<string> InvalidNameLengthsBelowMinimum()
        {
            var data = new TheoryData<string>();

            // Test all lengths from 0 to 2 characters (below minimum of 3)
            for (var length = 0; length <= 2; length++)
            {
                data.Add(new string('A', length));
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(InvalidNameLengthsAboveMaximum))]
        public async Task CreateWinery_When_Name_Length_Above_Maximum_Should_Return_BadRequest(
            string name
        )
        {
            // Arrange
            var createWineryDto = new CreateWineryDto { Name = name };
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync(Routes.Wineries.Create, createWineryDto);
            var errorContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(
                "must be 256 characters or fewer",
                errorContent,
                StringComparison.OrdinalIgnoreCase
            );
        }

        [Theory]
        [MemberData(nameof(InvalidNameLengthsBelowMinimum))]
        public async Task CreateWinery_When_Name_Length_Below_Minimum_Should_Return_BadRequest(
            string name
        )
        {
            // Arrange
            var createWineryDto = new CreateWineryDto { Name = name };
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync(Routes.Wineries.Create, createWineryDto);
            var errorContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(
                "must be at least 3 characters long",
                errorContent,
                StringComparison.OrdinalIgnoreCase
            );
        }
    }
}
