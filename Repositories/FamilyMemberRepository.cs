using Microsoft.EntityFrameworkCore;
using Npgsql;
using wealthify.Database;
using wealthify.Entity;
using wealthify.Exceptions;
using wealthify.Repositories.Interfaces;

namespace wealthify.Repositories;

public class FamilyMemberRepository(ApplicationDbContext context) : IFamilyMemberRepository
{
    public async Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return await context.FamilyMembers
            .AnyAsync(fm => fm.Name == normalizedName, cancellationToken);
    }

    public async Task<FamilyMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.FamilyMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(fm => fm.Id == id, cancellationToken);
    }

    public async Task<FamilyMember> CreateFamilyMemberAsync(FamilyMember familyMember, CancellationToken cancellationToken = default)
    {
        context.FamilyMembers.Add(familyMember);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException($"Family member '{familyMember.Name}' already exists.");
        }

        return familyMember;
    }
}
