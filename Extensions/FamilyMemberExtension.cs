using wealthify.DTOs.FamilyMember;
using wealthify.Entity;

namespace wealthify.Extensions;

public static class FamilyMemberExtension
{
    extension(FamilyMember entity)
    {
        public FamilyMemberDto ToDto()
        {
            return new FamilyMemberDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}
