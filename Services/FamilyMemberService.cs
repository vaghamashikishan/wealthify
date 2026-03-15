using wealthify.DTOs.FamilyMember;
using wealthify.Entity;
using wealthify.Exceptions;
using wealthify.Extensions;
using wealthify.Repositories.Interfaces;
using wealthify.Services.Interfaces;

namespace wealthify.Services;

public class FamilyMemberService(IFamilyMemberRepository repository) : IFamilyMemberService
{
    public async Task<FamilyMemberDto> CreateFamilyMemberAsync(CreateFamilyMemberDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedName = dto.Name.Trim().ToLower();
        if (await repository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            throw new ConflictException($"Family member '{normalizedName}' already exists.");
        }

        var familyMember = new FamilyMember
        {
            Name = normalizedName
        };

        var createdFamilyMember = await repository.CreateFamilyMemberAsync(familyMember, cancellationToken);
        return createdFamilyMember.ToDto();
    }

    public async Task<FamilyMemberDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var familyMember = await repository.GetByIdAsync(id, cancellationToken);
        return familyMember?.ToDto();
    }
}
