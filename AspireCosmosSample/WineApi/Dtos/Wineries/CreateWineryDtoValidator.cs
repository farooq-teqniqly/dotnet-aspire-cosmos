using FluentValidation;

namespace WineApi.Dtos.Wineries
{
    /// <summary>
    /// Validator for <see cref="CreateWineryDto"/>.
    /// </summary>
    public sealed class CreateWineryDtoValidator : AbstractValidator<CreateWineryDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWineryDtoValidator"/> class.
        /// </summary>
        public CreateWineryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Must(name => !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 3)
                .WithMessage("'Name' must be at least 3 characters long (excluding whitespace).")
                .Must(name => !string.IsNullOrWhiteSpace(name) && name.Trim().Length <= 256)
                .WithMessage("'Name' must be 256 characters or fewer (excluding whitespace).");
        }
    }
}
