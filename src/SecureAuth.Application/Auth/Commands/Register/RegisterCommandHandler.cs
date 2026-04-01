namespace SecureAuth.Application.Auth.Commands.Register
{
    using MediatR;
    using SecureAuth.Domain.Entities;
    using SecureAuth.Domain.Exceptions;
    using SecureAuth.Domain.Repositories;
    using SecureAuth.Application.Security.Interfaces;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var exists = await _userRepository.ExistsByEmailAsync(request.Email);

            if (exists)
                throw new BusinessException("Email já cadastrado");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password)
            };

            await _userRepository.AddAsync(user);

            return Unit.Value;
        }
    }
}
