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

        /// <summary>
        /// Initializes a new instance of the <see cref="WineriesController"/> class.
        /// </summary>
        /// <param name="wineryRepository">The repository for managing winery data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wineryRepository"/> is null.</exception>
        public WineriesController(IWineryRepository wineryRepository)
        {
            ArgumentNullException.ThrowIfNull(wineryRepository);

            _wineryRepository = wineryRepository;
        }

        /// <summary>
        /// Creates a new winery.
        /// </summary>
        /// <param name="createWineryDto">The data required to create a new winery.</param>
        /// <param name="validator">The validator for the create winery DTO.</param>
        /// <returns>A response containing the ID of the newly created winery.</returns>
        /// <response code="201">Returns the newly created winery's ID.</response>
        /// <response code="400">If the request body is invalid.</response>
        /// <response code="409">If a winery with the same name already exists.</response>
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
#pragma warning disable CA1062
                .CreateWineryAsync(createWineryDto.Name, HttpContext.RequestAborted)
#pragma warning restore CA1062
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

        /// <summary>
        /// Retrieves a winery by its ID.
        /// </summary>
        /// <param name="wineryId">The unique identifier of the winery to retrieve.</param>
        /// <returns>The winery details if found.</returns>
        /// <response code="200">Returns the requested winery.</response>
        /// <response code="404">If the winery with the specified ID is not found.</response>
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
