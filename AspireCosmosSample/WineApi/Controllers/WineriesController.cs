using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WineApi.Dtos.Wineries;
using WineApi.Entities;

namespace WineApi.Controllers
{
    [ApiController]
    [Route("wineries")]
    public sealed class WineriesController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WineryDto>> CreateWinery(
            [FromBody] CreateWineryDto createWineryDto,
            IValidator<CreateWineryDto> validator
        )
        {
            await validator.ValidateAndThrowAsync(createWineryDto).ConfigureAwait(false);

            var winery = new Winery
            {
                Id = $"wn_{Guid.CreateVersion7()}",
#pragma warning disable CA1062
                Name = createWineryDto.Name.Trim(),
#pragma warning restore CA1062
            };

            var wineryDto = new WineryDto(
                winery.Id,
                winery.Name,
                winery.CreatedAtUtc,
                winery.UpdatedAtUtc
            );

            await Task.CompletedTask.ConfigureAwait(false);
            return CreatedAtAction(nameof(GetWinery), new { wineryId = wineryDto.Id }, wineryDto);
        }

        [HttpGet("{wineryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetWinery(string wineryId)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return NoContent();
        }
    }
}
