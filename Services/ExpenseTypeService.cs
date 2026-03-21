using wealthify.DTOs.ExpenseType;
using wealthify.Entity;
using wealthify.Exceptions;
using wealthify.Extensions;
using wealthify.Repositories.Interfaces;
using wealthify.Services.Interfaces;

namespace wealthify.Services;

public class ExpenseTypeService(IExpenseTypeRepository repository) : IExpenseTypeService
{
    public async Task<ExpenseTypeDto> CreateExpenseTypeAsync(CreateExpenseTypeDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedName = dto.ExpenseTypeName.Trim().ToLower();
        if (await repository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            throw new ConflictException($"Expense type '{normalizedName}' already exists.");
        }

        var expenseType = new ExpenseType
        {
            ExpenseTypeName = normalizedName
        };

        var createdExpenseType = await repository.CreateExpenseTypeAsync(expenseType, cancellationToken);
        return createdExpenseType.ToDto();
    }

    public async Task<List<ExpenseTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var expenseTypes = await repository.GetAllAsync(cancellationToken);
        return [.. expenseTypes.Select(expenseType => expenseType.ToDto())];
    }

    public async Task<ExpenseTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var expenseType = await repository.GetByIdAsync(id, cancellationToken);
        return expenseType?.ToDto();
    }

    public async Task<ExpenseTypeDto?> UpdateExpenseTypeAsync(UpdateExpenseTypeDto dto, CancellationToken cancellationToken = default)
    {
        var existingExpenseType = await repository.GetByIdAsync(dto.Id, cancellationToken);
        if (existingExpenseType is null)
        {
            return null;
        }

        var normalizedName = dto.ExpenseTypeName.Trim().ToLower();
        if (!string.Equals(existingExpenseType.ExpenseTypeName, normalizedName, StringComparison.Ordinal) &&
            await repository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            throw new ConflictException($"Expense type '{normalizedName}' already exists.");
        }

        var expenseTypeToUpdate = new ExpenseType
        {
            Id = existingExpenseType.Id,
            ExpenseTypeName = normalizedName,
            CreatedAt = existingExpenseType.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updatedExpenseType = await repository.UpdateExpenseTypeAsync(expenseTypeToUpdate, cancellationToken);
        return updatedExpenseType.ToDto();
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.DeleteByIdAsync(id, cancellationToken);
    }
}