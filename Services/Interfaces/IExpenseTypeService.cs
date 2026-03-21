using wealthify.DTOs.ExpenseType;

namespace wealthify.Services.Interfaces;

public interface IExpenseTypeService
{
    Task<ExpenseTypeDto> CreateExpenseTypeAsync(CreateExpenseTypeDto dto, CancellationToken cancellationToken = default);
    Task<List<ExpenseTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ExpenseTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ExpenseTypeDto?> UpdateExpenseTypeAsync(UpdateExpenseTypeDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}