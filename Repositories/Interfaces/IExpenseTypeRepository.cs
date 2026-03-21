using wealthify.Entity;

namespace wealthify.Repositories.Interfaces;

public interface IExpenseTypeRepository
{
    Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task<List<ExpenseType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ExpenseType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ExpenseType> CreateExpenseTypeAsync(ExpenseType expenseType, CancellationToken cancellationToken = default);
    Task<ExpenseType> UpdateExpenseTypeAsync(ExpenseType expenseType, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}