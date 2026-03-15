using wealthify.DTOs.InvestmentType;
using wealthify.Entity;
using wealthify.Exceptions;
using wealthify.Extensions;
using wealthify.Repositories.Interfaces;
using wealthify.Services.Interfaces;

namespace wealthify.Services;

public class InvestmentTypeService(IInvestmentTypeRepository repository) : IInvestmentTypeService
{
    public async Task<InvestmentTypeDto> CreateInvestmentTypeAsync(CreateInvestmentTypeDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedName = dto.Name.Trim();
        if (await repository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            throw new ConflictException($"Investment type '{normalizedName}' already exists.");
        }

        var investmentType = new InvestmentType
        {
            Name = normalizedName,
            CreatedAt = DateTime.UtcNow
        };

        var createdInvestmentType = await repository.CreateInvestmentTypeAsync(investmentType, cancellationToken);
        return createdInvestmentType.ToDto();
    }

    public async Task<InvestmentTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var investmentType = await repository.GetByIdAsync(id, cancellationToken);
        return investmentType?.ToDto();
    }
}
