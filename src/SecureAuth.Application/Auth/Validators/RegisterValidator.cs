namespace SecureAuth.Application.Auth.Validators
{
    using FluentValidation;
    using SecureAuth.Application.Auth.Commands.Register;

    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}
