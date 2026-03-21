using Microsoft.EntityFrameworkCore;
using Npgsql;
using wealthify.Database;
using wealthify.Entity;
using wealthify.Exceptions;
using wealthify.Repositories.Interfaces;

namespace wealthify.Repositories;

public class ExpenseTypeRepository(ApplicationDbContext context) : IExpenseTypeRepository
{
    public async Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return await context.ExpenseTypes
            .AnyAsync(et => et.ExpenseTypeName == normalizedName, cancellationToken);
    }

    public async Task<List<ExpenseType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ExpenseTypes
            .AsNoTracking()
            .OrderBy(et => et.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ExpenseType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.ExpenseTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(et => et.Id == id, cancellationToken);
    }

    public async Task<ExpenseType> CreateExpenseTypeAsync(ExpenseType expenseType, CancellationToken cancellationToken = default)
    {
        context.ExpenseTypes.Add(expenseType);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException($"Expense type '{expenseType.ExpenseTypeName}' already exists.");
        }

        return expenseType;
    }

    public async Task<ExpenseType> UpdateExpenseTypeAsync(ExpenseType expenseType, CancellationToken cancellationToken = default)
    {
        context.ExpenseTypes.Update(expenseType);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException($"Expense type '{expenseType.ExpenseTypeName}' already exists.");
        }

        return expenseType;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var rows = await context.ExpenseTypes
            .Where(et => et.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return rows > 0;
    }
}