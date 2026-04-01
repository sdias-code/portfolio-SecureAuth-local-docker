using MediatR;
using Microsoft.EntityFrameworkCore;
using SecureAuth.Application.Common.Interfaces;
using SecureAuth.Application.Auth.DTOs;

namespace SecureAuth.Application.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, UserResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetCurrentUserHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<UserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == _currentUser.UserId)
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    Email = x.Email
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException();

            return user;
        }
    }
}