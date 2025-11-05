using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WineApi.Dtos.Wineries;
using WineApi.Repositories;

namespace WineApi.Controllers
{
    [ApiController]
    [Route("wineries")]
    public sealed class WineriesController : ControllerBase
    {
        private readonly IWineryRepository _wineryRepository;

        public WineriesController(IWineryRepository wineryRepository)
        {
            ArgumentNullException.ThrowIfNull(wineryRepository);

            _wineryRepository = wineryRepository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateWineryResponseDto>> CreateWinery(
            [FromBody] CreateWineryDto createWineryDto,
            IValidator<CreateWineryDto> validator
        )
        {
            await validator.ValidateAndThrowAsync(createWineryDto).ConfigureAwait(false);

            var createWineryResult = await _wineryRepository
                .CreateWineryAsync(createWineryDto.Name, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            if (createWineryResult.IsSuccess)
            {
                var wineryId = createWineryResult.GetValue();

                return CreatedAtAction(
                    nameof(GetWinery),
                    new { wineryId },
                    new CreateWineryResponseDto(wineryId)
                );
            }

            return Problem(
                createWineryResult.GetError().Message,
                statusCode: StatusCodes.Status409Conflict
            );
        }

        [HttpGet("{wineryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WineryDto>> GetWinery(string wineryId)
        {
            var getWineryResult = await _wineryRepository
                .GetWineryAsync(wineryId, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            if (getWineryResult.IsSuccess)
            {
                var winery = getWineryResult.GetValue();

                return Ok(
                    new WineryDto(winery.Id, winery.Name, winery.CreatedAtUtc, winery.UpdatedAtUtc)
                );
            }

            return Problem(
                getWineryResult.GetError().Message,
                statusCode: StatusCodes.Status404NotFound
            );
        }
    }
}
