using FluentValidation;
using SecureAuth.Application.Auth.Commands.Refresh;

namespace SecureAuth.Application.Auth.Validators
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token é obrigatório")
                .MinimumLength(10).WithMessage("Refresh token inválido")
                .MaximumLength(500).WithMessage("Refresh token inválido");
        }
    }
}