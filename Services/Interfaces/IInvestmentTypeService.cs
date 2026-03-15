using wealthify.DTOs.InvestmentType;

namespace wealthify.Services.Interfaces;

public interface IInvestmentTypeService
{
    Task<InvestmentTypeDto> CreateInvestmentTypeAsync(CreateInvestmentTypeDto dto, CancellationToken cancellationToken = default);
    Task<InvestmentTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
