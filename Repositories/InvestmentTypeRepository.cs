using Microsoft.EntityFrameworkCore;
using Npgsql;
using wealthify.Exceptions;
using wealthify.Database;
using wealthify.Entity;
using wealthify.Repositories.Interfaces;

namespace wealthify.Repositories;

public class InvestmentTypeRepository(ApplicationDbContext context) : IInvestmentTypeRepository
{
    public async Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        var loweredName = normalizedName;
        return await context.InvestmentTypes
            .AnyAsync(it => it.Name == loweredName, cancellationToken);
    }

    public async Task<InvestmentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.InvestmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(it => it.Id == id, cancellationToken);
    }

    public async Task<InvestmentType> CreateInvestmentTypeAsync(InvestmentType investmentType, CancellationToken cancellationToken = default)
    {
        context.InvestmentTypes.Add(investmentType);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException($"Investment type '{investmentType.Name}' already exists.");
        }

        return investmentType;
    }
}
