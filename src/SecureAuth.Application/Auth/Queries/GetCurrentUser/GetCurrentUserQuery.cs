using MediatR;
using SecureAuth.Application.Auth.DTOs;

namespace SecureAuth.Application.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<UserResponse>
    {       
    }
}