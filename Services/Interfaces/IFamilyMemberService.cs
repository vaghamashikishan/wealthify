using wealthify.DTOs.FamilyMember;

namespace wealthify.Services.Interfaces;

public interface IFamilyMemberService
{
    Task<FamilyMemberDto> CreateFamilyMemberAsync(CreateFamilyMemberDto dto, CancellationToken cancellationToken = default);
    Task<FamilyMemberDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
