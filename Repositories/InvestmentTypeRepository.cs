using Microsoft.EntityFrameworkCore;
using wealthify.Database;
using wealthify.Entity;
using wealthify.Repositories.Interfaces;

namespace wealthify.Repositories;

public class InvestmentTypeRepository(ApplicationDbContext context) : IInvestmentTypeRepository
{
    public async Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        var loweredName = normalizedName.ToLower();
        return await context.InvestmentTypes
            .AnyAsync(it => it.Name.ToLower() == loweredName, cancellationToken);
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
        await context.SaveChangesAsync(cancellationToken);
        return investmentType;
    }
}
