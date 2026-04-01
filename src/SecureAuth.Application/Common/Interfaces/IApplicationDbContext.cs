using Microsoft.EntityFrameworkCore;
using SecureAuth.Domain.Entities;

namespace SecureAuth.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
    }
}