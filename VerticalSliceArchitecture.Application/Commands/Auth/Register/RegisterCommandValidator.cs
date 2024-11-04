using FluentValidation;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{

    public RegisterCommandValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email invalid");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must have at least 8 characters");
    }
}