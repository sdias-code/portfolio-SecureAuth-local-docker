using FluentValidation;
using SecureAuth.Application.Auth.Commands.Login;

namespace SecureAuth.Application.Auth.Validators
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(150).WithMessage("Email muito longo");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
                .MaximumLength(100).WithMessage("Senha muito longa");
        }
    }
}