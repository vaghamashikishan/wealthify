using System;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using wealthify.Database;
using wealthify.Entity;
using wealthify.Exceptions;
using wealthify.Repositories.Interfaces;

namespace wealthify.Repositories;

public class AuthRepository(ApplicationDbContext dbContext) : IAuthRepository
{
    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            dbContext.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException($"'{user.Email}' already exists.");
        }
    }

    public async Task<User?> FindByEmail(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }
}
