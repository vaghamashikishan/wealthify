using wealthify.Entity;

namespace wealthify.Repositories.Interfaces;

public interface IFamilyMemberRepository
{
    Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task<FamilyMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FamilyMember> CreateFamilyMemberAsync(FamilyMember familyMember, CancellationToken cancellationToken = default);
}
