using FluentValidation;

namespace WineApi.Dtos.Wineries
{
    public sealed class CreateWineryDtoValidator : AbstractValidator<CreateWineryDto>
    {
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
