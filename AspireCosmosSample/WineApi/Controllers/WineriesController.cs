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
        public async Task<ActionResult<CreateWineryResponseDto>> CreateWinery(
            [FromBody] CreateWineryDto createWineryDto,
            IValidator<CreateWineryDto> validator
        )
        {
            await validator.ValidateAndThrowAsync(createWineryDto).ConfigureAwait(false);

            var wineryId = await _wineryRepository
                .CreateWineryAsync(createWineryDto.Name, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            return CreatedAtAction(
                nameof(GetWinery),
                new { wineryId },
                new CreateWineryResponseDto(wineryId)
            );
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
