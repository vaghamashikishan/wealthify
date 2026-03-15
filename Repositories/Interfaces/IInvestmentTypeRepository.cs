using wealthify.Entity;

namespace wealthify.Repositories.Interfaces;

public interface IInvestmentTypeRepository
{
    Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task<InvestmentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InvestmentType> CreateInvestmentTypeAsync(InvestmentType investmentType, CancellationToken cancellationToken = default);
}
