using System;
using wealthify.Entity;

namespace wealthify.Repositories.Interfaces;

public interface IAuthRepository
{
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> FindByEmail(string email, CancellationToken cancellationToken = default);
}
